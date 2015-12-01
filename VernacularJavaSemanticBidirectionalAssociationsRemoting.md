**This page was used as a first approach, in an discussion. The conclusion is different, and this page is not a correct rendering of what we will do. A later version might be written, or not.**

# Introduction #

The context of this discussion is how to deal with bidirectional relationships in a 2-faces context, with persistence on one side, and use of the semantic objects outside of the persistence context on the other side.

The problem is the same in the context of EJB2, HIbernate, JPA, or any other persistence technology (and EJB3 / JPA does not solve it). The problem is the same in the context of any use of the semantic objects out side the persistence context. In particular, when the semantic objects are transported from one system to another, via serialization, XML or other marshalling, JSON conversion, with RMI, web services, dwr, or any other technology.

There is no good solution to date that we can think of. In this text, we take down different possible approaches, and discuss their benefits and negative points. Finally, we choose one approach, which does not solve the problem completely, as our vernacular.


# The example #

We will use an example in this text to make things more clear. We will consider a bidirectional one-to-many relation between enterprises and contracts (one enterprise has 0-n contracts, a contract has 0-1 enterprise). Although a civilized contract cannot exist without an enterprise, it can exist in RAM as a non-civil contract. This is necessary for a number of reasons, amongst others, because we need a contract object a priori during creation, to set an enterprise on (creation is a slow, user interactive proces). A normal Java implementation would be:

```
public class Enterprise {

  /*<property name="contracts">
  -------------------------------------------------------------------------*/

  @MethodContract(pre  = {@Expression("_c != null"),
                          @Expression("_c.enterprise == this")},
                  post = @Expression("contracts.contains(_c)"))
  final void addContract(Contract c) {
    ProgrammingErrors.preArgumentNotNull(c, "c");
    $contracts.add(c);
  }

  @MethodContract(post = @Expression("! contracts.contains(_c)"))
  final void removeContract(Contract c) {
    $contracts.remove(c);
  }

  @Basic(init   = @Expression("contracts.isEmpty()"),
         invars = {@Expression("contracts != null"),
                   @Expression("! contracts.contains(null)"),
                   @Expression("for (Contract c : contracts) { c.enterprise == this }")})
  final public Set<Contract> getContracts() {
    return new HashSet<Contract>($contracts);
  }

  @Invars({@Expression("$contracts != null"),
           @Expression("! $contracts.contains(null)"),
           @Expression("for (Contract c : $contracts) { c.enterprise == this }")})
  private Set<Contract> $contracts = new HashSet<Contract>();

  /*</property>*/

}
```
```
public class Contract {

  /*<property name="Enterprise">
  -------------------------------------------------------------------------*/

  @Basic(init   = @Expression("enterprise == null"),
         invars = @Expression("enterprise != null ? enterprise.contracts.contains(this)"))
  public final Enterprise getEnterprise() {
    return $enterprise;
  }

  @MethodContract(post = {@Expression("enterprise == _enterprise"),
                          @Expression("'enterprise != null ? ! 'enterprise.contracts.contains(this)"),
                          @Expression("_enterprise != null ? _enterprise.contracts.contains(this)")})
  public final void setEnterprise(Enterprise enterprise) {
    if ($enterprise != enterprise) {
      if ($enterprise != null) {
        $enterprise.removeContract(this);
      }
      $enterprise = enterprise;
      if ($enterprise != null) {
        $enterprise.addContract(this);
      }
    }
  }

  private Enterprise $enterprise;

  /*</property>*/

}
```


# Idea 1: only use unidirectional associations #

Bidirectional associations are, in general needed for our semantics. One possible solution of the problem, is to not work with bidirectional associations, but replace them with unidirectional relations, as follows:

```
public class Enterprise {

  // NOP

}
```
```
public class Contract {

  /*<property name="Enterprise">
  -------------------------------------------------------------------------*/

  @Basic(init = @Expression("enterprise == null"))
  public final Enterprise getEnterprise() {
    return $enterprise;
  }

  @MethodContract(post = @Expression("enterprise == _enterprise"))
  public final void setEnterprise(Enterprise enterprise) {
    $enterprise = enterprise;
  }

  private Enterprise $enterprise;

  /*</property>*/

}
```

To get the contracts for a given enterprise, we would then rely on business logic, in a separate class, something of the form:
```
  public interface EnterpriseDao {

    Set<Contract> contractsFor(Enterprise enterprise);

  }
```

The implementation depends on the use of the code. In a context where civilized enterprises and contracts are persisted to a RDBMS, this method could be implemented using SQL from the database, or using a named query when Hibernate or JPA are choosen.

This might work, but now we have lost a lot, which can not easily be replaced. A lot of the semantics of enterprises and contracts is in their structure, but an equal amount of semantics is expressed in extra invariants, which result in _validation code_. In our vernacular, this validation code turns up in the setter (in this case `Contract.setEnterprise(Enterprise)` and in the `Contract.wildExceptions` and `Enterprise.wildExceptions` methods, which test civility.

Whenever an invariant expresses limitations on the set of contracts for an enterprise, we are in trouble. Imagine, e.g., a business rule that says that contracts, which have a start and an end date, for a given enterprise, may not overlap. In the original code, this invariant would be expressed in Enterprise, and be of the form:
```
for (Contract c1 : contracts) {for (Contract c2: contracts) {c1 != c2 ?? noOverlap(c1, c2)}}
```
This invariant would be enforced by adding validation code to `Contract.setEnterprise(Enterprise)`:
```
  @MethodContract(post = {@Expression("enterprise == _enterprise"),
                          @Expression("'enterprise != null ? ! 'enterprise.contracts.contains(this)"),
                          @Expression("_enterprise != null ? _enterprise.contracts.contains(this)")})
  public final void setEnterprise(Enterprise enterprise) throws ContractOverlapException {
    if ($enterprise != enterprise) {
      if (enterprise != null) {
        if (hasOverlap(enterprise.getContracts()) {
          throw new ContractOverlapException(this, enterprise);
        }
      }
      if ($enterprise != null) {
        $enterprise.removeContract(this);
      }
      $enterprise = enterprise;
      if ($enterprise != null) {
        $enterprise.addContract(this);
      }
    }
  }

  private boolean hasOverlap(Set<Contract> contracts) {
    for (Contract c : contracts) {
      if (this.overlaps(c) {
        return true;
      }
    }
    return false;
  }

  ...
```

Furthermore, the contract period can probably be changed, and thus we need validation code in these setters to (to make sure a contract period of a contract of a given enterprise is not changed incorrectly to overlap with another contract of that given enterprise, after the relationship is set up). (Note that here an invariant from class Enterprise is enforced in another class, Contract, which is less then desirable).

In the version with a unidirectional association, there is no way we can express this invariant in an encapsulated way.

We could add an invariant instead on Enterprise of the form:
```
for (Contract c1) {for (Contract c2) {c1.enterprise = this && c2.enterprise == this && c1 != c2 ?? noOverlap(c1, c2)}}
```

This is correct in the contract paradigm. The problem is that we have no way to implement this in a rational way. From the side of the enterprise, it is not possible in any way, as before. But now it is not even possible from the side of the Contract, _because we have no Java-programmatic way to get the set of contracts of a given enterprise_. There is not even a set of all contracts, we could filter for the contracts of the given enterprise.

Using the method `EnterpriseDao.contractsFor(Enterprise)` is a possibility. Since in this case the validation code is in Contract, a contract instance needs a way to get at the correct EnterpriseDao. But since a contract does have a reference to the related Enterprise, we could possibly store the reference to the EnterpriseDao in each enterprise instance. An enterprise would then have an additional method `getEnterpriseDao()`. But in that case, we could also implement the `getContracts()` method (which is the only public method for the contracts relationship in Enterprise) as:
```
  public Set<Contract> getContracts() {
    return getEnterpriseDao().contractsFor(this);
  }
```
There is a caveat here, though. In, e.g., a JPA context, `myContract.setEnterprise(myEnterprise)` would **not** result in `myEnterprise.getContracts().contains(myContract)`! At least not until the current JPA transaction would be committed! _OR WOULD IT: IF WE ARE ATTACHED, JPA TAKES CARE OF THIS???_

The remaining problem is how to make each Enterprise instance aware of the related Dao? With the above pattern, we would need to inject the DAO in each entity in some way (but we surely do not want the enterprise dao instance to be persisted itself, or even serialized, so it is not a real property!). This problem is probably even harder in other contexts. Imagine the enterprise being serialized, send over the wire, and deserialized. There, it needs some enterprise DAO too, but surely another implementation is needed there than the JPA implementation we used on the server.

Perhaps the solution lies here in a DAO manager:

```
  public interface DaoManager {
    
    Dao daoFor(Class<?> persistentBeanType);
 
  }
```

With this being a singleton in each context (JPA on the backend server, something else on the web tier, and possibly even a JavaScript version in the browser), no injection in the classes is needed.
```
  public Set<Contract> getContracts() {
    return ((EnterpriseDao)DaoManager.getInstance().daoFor(Enterprise.class)).contractsFor(this);
  }
```

Actually, we can use this solution without making Enterprise dirty with this code. But such code would in any case appear in the Contracts class for validation reasons.

We are not happy with this code, for several reasons.
First of all, it introduces a circular dependency between the DAO and the semantic classes, which is known bad practice. The semantic classes (Enterprise, Contract) can, in no context whatsoever, be used without the DAO. Secondly, every context would need a good implementation of the DAO. Third, the contracts we get returned in this way are related to an Entrprise object, but that would often be a _different_ Enterprise object than now (i.e., when we are using it detached from a JPA context that resolves this for us; writing some resolver like that for remote use ourselfs would be very difficult.


# Remote we do want unidirectionality #

For remote work, we actually do not want bidirectionality, for several reasons:
  * We don not always want to carry all to-many relationships over the wire. Please let us get those to-many instances we want, don't force them upon us.
  * If you push all to-many instances over the wire for all to-many relationships always, you will, with each call, almost get the entire database over the wire. Normally, almost all objects in a business system are related to each other.
  * Validation: we have validation in our bidirectional version of `Contract.setEnterprise(Enterprise)`. Frankly, for reasons of performance, we do not want that type of validation to occur locally. Semantically, it would be okay, but it will be repeated on the server anyway. This would mean 2 database hits and a lot of network traffic for no good reason.
  * ...

We seem to have 2 different needs for Enterprises and Contracts: a bidirectional version on the server, and a, more controllable unidirectional version remotely.