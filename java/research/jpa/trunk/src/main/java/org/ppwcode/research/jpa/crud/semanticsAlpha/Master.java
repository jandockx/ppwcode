/*<license>
Copyright 2008 - $Date: 2008-09-29 16:35:07 +0200 (Mon, 29 Sep 2008) $ by PeopleWare n.v.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
</license>*/

package org.ppwcode.research.jpa.crud.semanticsAlpha;


import static org.ppwcode.metainfo_I.License.Type.APACHE_V2;
import static org.ppwcode.vernacular.exception_II.ProgrammingErrorHelpers.preArgumentNotNull;

import java.util.HashSet;
import java.util.Set;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.FetchType;
import javax.persistence.OneToMany;
import javax.persistence.Table;

import org.ppwcode.metainfo_I.Copyright;
import org.ppwcode.metainfo_I.License;
import org.ppwcode.metainfo_I.vcs.SvnInfo;
import org.ppwcode.vernacular.persistence_III.AbstractIntegerIdVersionedPersistentBean;
import org.toryt.annotations_I.Basic;
import org.toryt.annotations_I.Expression;
import org.toryt.annotations_I.Invars;
import org.toryt.annotations_I.MethodContract;


/**
 * master
 */
@Entity
@Table(name="org_ppwcode_research_jpa_crud_semanticsalpha_master")
@Copyright("2008 - $Date: 2008-09-29 18:21:16 +0200 (Mon, 29 Sep 2008) $, PeopleWare n.v.")
@License(APACHE_V2)
@SvnInfo(revision = "$Revision: 2727 $",
         date     = "$Date: 2008-09-29 18:21:16 +0200 (Mon, 29 Sep 2008) $")
public class Master extends AbstractIntegerIdVersionedPersistentBean {


  /*<property name="name">
  -------------------------------------------------------------------------*/

  /**
   * Can be {@code null}.
   */
  public final String getName() {
    return $name;
  }

  public final void setName(String s) {
    $name = s;
  }

  @Column(name="name")
  private String $name;

  /*</property>*/



  /*<property name="details">
  -------------------------------------------------------------------------*/

  @MethodContract(pre  = {@Expression("_c != null"),
                          @Expression("_c.master == this")},
                  post = @Expression("details.contains(_c)"))
  final void addDetail(Detail c) {
    assert preArgumentNotNull(c, "c");
    $details.add(c);
  }

  @MethodContract(post = @Expression("! details.contains(_c)"))
  final void removeDetail(Detail c) {
    $details.remove(c);
  }

  @Basic(init   = @Expression("details.isEmpty()"),
         invars = {@Expression("details != null"),
                   @Expression("! details.contains(null)"),
                   @Expression("for (Detail c : details) { c.master == this }")})
  final public Set<Detail> getDetails() {
    return $details == null ? null : new HashSet<Detail>($details);
  }

  @OneToMany(mappedBy = "$master", cascade = {}, fetch=FetchType.LAZY)
  @Invars({@Expression("$details != null"),
           @Expression("! $details.contains(null)"),
           @Expression("for (Detail c : $details) { c.master == this }")})
  // hibernate and JPA do not allow final for the set.
  Set<Detail> $details = new HashSet<Detail>(); // package accessible for some tests
  // set is null after deserialization if not initialized before serialization

  /*</property>*/

}

