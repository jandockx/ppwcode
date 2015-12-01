# Introduction #

At times, certainly during initial development, we need to release the entire ppwcode library structure as a whole. Since projects are interdependent, this has to be done in a certain order. The dependencies are a graph. Releases have to be done in an order that respects the graph (dependent before supplier), because dependents have to refer to the correct version of the supplier, and the dependents can only be built when that supplier version is available.

The project tree does not entirely represent all dependencies. There are some interdependencies between branches (notably ppwcode-vernacular-exception is also used in ppwcode-util). Below is a list that fullfils the requirements. For global builds, the list should be followed. Furthermore, if a new version is released, the projects lower in the list might be in need of a new version too. Since however, through dependency management, most versions of ppwcode projects are described in ppwcode-ppwcode-library-parent-pom, we probably need to rebuild that too.

NOTE: there should be a better way to describe this than a list

# Project tree #

  * checkstyle
    * licensetemplates
    * checkstyle
  * maven2
    * library parent pom
    * dependency management
  * ppwcode pom
    * ppwcode library parent pom
  * metainfo
  * util
    * parent pom
    * exception
    * reflection
    * serialization
  * vernacular
    * parent pom
    * exception
    * resourcebundle
    * semantics
    * persistence
    * value
  * value
  * research
    * jpa



# Dependency tree #

  * licensetemplates
  * checkstyle
  * library parent pom
  * dependency management (snapshot)
  * ppwcode library parent pom

  * metainfo
  * (toryt annotations - _outside of ppwcode, but we have "influence" to get a new build :-)_ )

  * util parent pom
  * util exception
  * util reflection
  * util serialization

  * vernacular parent pom
  * vernacular resourcebundle
  * vernacular exception
  * vernacular semantics
  * vernacular persistence (snapshot)
  * vernacular value (snapshot)

  * value (snapshot)

  * research jpa (snapshot)