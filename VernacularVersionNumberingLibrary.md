_NOTE: this page needs work_


# Introduction #

Ppwcode projects are versioned with a mark version, major version and minor version.  This document explains the meaning of each part of a version number.

# Details #

Ppwcode projects are obviously versioned.  The project's version always needs to be configured in the project's POM.  The version number must adhere to the following rules (these rules are of course subject to change).

## Mark Version ##

Each project has a mark version.  The mark version corresponds with the major number in traditional versioning schemes, which typically carry a `<major>.<minor>.<micro>` structure.  In ppwcode projects, the mark version number is part of the maven `artifactId`.  Because it is part of the Artifact ID, it is possible that a project depends on multiple versions of the same package. This may be beneficial with fast evolving software systems.

The mark version is written as a roman numeral.  Examples are bean\_I, persistence\_III, i18n\_IV.

A package MUST receive a new mark version number if the following is true:

  * Methods in the package's public API have disappeared.
  * Method signatures in the package's public API have changed, and change the contract (semantic changes).
  * The contracts of methods in the API have changed intentionally (i.e. the change in the contract is not a bug fix).
  * The signature of methods in the API has become more restrictive, potentially breaking depending software.  E.g. public methods that are made final.

## Major Version ##

A ppwcode has a major version that is configured in the POM's `version`.  The version number consists of a major and a minor number.   Within the same mark version, a package MUST receive a different major version number if the following is true:

  * Methods are deprecated (no changes in signature or contracts).
  * Method signatures in the package's public API have changed, but do not change the contract. For example, (changing the API to reflect evolution in Java version, such as adding Generics)
  * New methods are added.  New contracts are hence added, however, without affecting existing contracts.

A general rule is: changes in the major version are required in modifications of the package will allow depending projects to compile, but possibly with warnings.  There are no semantic modifications made in the new major version.

## Minor Version ##

A ppwcode has a major version that is configured in the POM's `version`.  The version number consists of a major and a minor number.  Within the same mark and major version, a different minor number means:

  * Bug fixes in the API's contract
  * Bug fixes in the implementation
  * Implementation changes (performance, refactoring, ...)

A general rule is: changes in the minor version are required for any modification that does not affect the mark or major version.

In general, a minor version upgrade is highly advised, maybe even mandatory, for users. If a A b.c exists, we will no longer support A b.d, with d < c.

# Open Issues #

  * Do annotations affect the versioning?
  * Do we make any assumptions on binary compatibility?  Do we really need to do that in an open source setting:  if it's not binary compatible, then recompile the source...
  * What if the code does no longer compile with newer java compilers, uses deprecated Java APIs, generates warnings (Absence of Override annotations, not using generics, ...)