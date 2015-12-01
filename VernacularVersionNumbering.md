# Introduction #

Version numbering is pretty important, in the communication from the developers to the users of an artifact. And it is not so straightforward as it might seem. Most version numbering schemes follow a dotted, nested notation, in essence generating a tree of versions.

What is confusing however is what the different numbers mean. Also, a version numbering scheme should be consistent in time. It is quite annoying when, during the lifetime of a product, the version numbering scheme has to be changed. This will happen nonetheless, but the annoyance we encountered in the past might be of a benefit to you. The annoyance we encountered in the past makes us at least careful, so that we want to think about it beforehand, and make explicit a number of things.


# General #

In general, most people follow a dotted, nested notation, like `n.m.k` for version numbering. With 3 numbers, we talk about a **major version number** (`n`), a **minor version number** (`m`), and a **micro version number** (`k`). There is however no well-known consensus about the  semantices, the sense or nonsense of these numbers.

In the vernacular, we try to define such semantics. They differ however given the type of artifact.

  * [General](VernacularVersionNumberingGeneral.md) (apply to **pom**, **resource artifacts**), ...)
  * [Library](VernacularVersionNumberingLibrary.md)
  * Application
    * EJB jar, service, webservice application, back-end application
    * web application
    * desktop application