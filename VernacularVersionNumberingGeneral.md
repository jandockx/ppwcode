# Introduction #

This page describes the general version numbering vernacular. This vernacular is to be applied if there is no more specialized version numbering vernacular that applies.


# Release version numbers #

In general, we follow the wel known, 3-number, dotted, nested notation  `n.m.k` for version numbering.

  * `n` is called the **major version number**
  * `m' is called the **minor version number**
  * `k` is called the **micro version number**

This schema **must** be applied to

  * pom's
  * resource artifacts


## Micro version number ##

The micro version number is reserved for **bugfixes**. If version `n.m.k` does not work, has an error of some sort, version `n.m.k+1` should have fixed the error and do work.

The micro version numbering for a give minor version number starts at 0.

_NOTE: an alternative is to consider this an "implementation version", and use a build number instead_



## Minor version number ##

Minor version numbers are reserved for evolution of the artifact. Certainly working agile, a first release does not have all the bells and whistles you would like the product to have. It is important to release early. It does paint the general intention and applicability of the product, and promises enhancements.

Newer minor versions of the product will add more "features", whatever that might mean in your technology, however without "breaking compatibility", whatever that might mean in your technology. This means that, in all possible circumstances, if a user replaces a version `n.m.k` with a version `n.m+1.k`, his product will continue working without any change. Possibly, the user now will gain some or all of the new features automatically, possibly he now has the possibility to start using the new features.

When the minor version number changes, the micro version number is reset to 0.
The minor version numbering for a give major version number starts at 0.



## Major version number ##

Major version number changes signal incompatibility. This may occur either

  * because we want to add new features, but it turns out to be impossible to have a user replace an older version with the new version and have his product to continue working without any change, at least in one possible circumstance; or
  * because we saw that we were completely wrong, and wanted to do a complete rewrite, using a completely different approach; or
  * ...

If the user is using a version `n.m.k`, he should expect the need for a total rewrite or at least major changes in his project before he can start using version `n+1.m.k`.

When the major version number changes, the minor version number is reset to 0.
The major version number starts at 1. There are no "0" major versions.


## Border cases ##

The one dire point is that one small change we make, with enormous perceived benefits for the user, based on version `n.m.k`, that happens to require the slightest change in the user setup to make his product work with the new version.

This surely is not a complete rewrite, a different approach nor a new architecture. The intention surely is evolutionary. However, the fact of the matter is, that compatibility is broken. We would like, as developers, to see this as a minor update (`n.m+1.0`). Because compatibility is broken, the rules say it is a new major version (`n+1.0.0`) however.

First of all, in this case, you should work to provide a default or something for the new feature, to really keep compatibility. But the semantics of minor and major version numbers are pretty clear, black or white: if there is an incompatibility, this is called a new major version, whatever the _intention_ of the new version is. The latter is important. Version numbers are an important communication between the developer of a product, and its user. And it that, _one-way_ (!) communication, the intention is not that relevant. In a communication like this, we need to give the needs of the receiver of the communication, his good understanding, precedence over our intention. For the user, a new major version is a red flag: "if I change to this new version, it will take me some time", while a new version within the same major version is a big green flag: "I can change to this new version without any concern and zero time". Break the promise of the big green flag once, and the user will never be able to trust the flag again. Furthermore, the difference between zero time, and half an hour (to read the documentation, and the little change needed) is infinite (try a division by zero sometime, using floating point arithmetic that is :-) ). **You must never betray the signal of an unchanged major version**. Not even for marketing reasons.



# Pre-release version numbers #

It is often sensible to freeze development of an artifact in a final phase before a release, and go through some iterations of acceptance testing. The versions that are tested here are most often called _alpha_ and _beta_ versions.

_Alpha_ versions are considered teaser versions, versions to show the user or client already something, to give him ease of mind. Alpha versions are not intended to be bugfree. In this vernacular, we will not use alpha versions. A _development version_ (see below) can be used for this purpose.

_Beta_ versions are used in the final stretch toward a release. Evolutions in consecutive beta versions are bugfixes, never new features. The reason why we want to introduce beta version numbers, is to keep the semantics of a "`n.m.0`" release as "stable".

When we get ready for a new release that is not a bugfix itself, you always should build a beta version first, and give it to the user to test. Because evolutions in the beta versions are bugfixes of a new major or minor versions, there is no need for a micro version in a beta release version number. In fact, the micro version number is replaced by the beta version number. So, the form of a beta release version number is `n.m beta-k`. **Please really use the word _beta_, and not just the letter _b_, as this has proven to be confusing in some circumstances** (sometimes, also letters are used as meaningful version numbering parts, e.g., in the JDK). If it is possible to use the lower case Greek letter beta, please do. An eszet (ß) as an approximation for a lower case beta is not a good idea.



# Development version numbers #

During development, we often create intermediate versions. For these versions, we simply use a time based notation: `dYYYYMMDD-hhmm`, where `d` stands for "development version". There is no need to prepend this notation with a dot-version, since very often, during development, _you don't know yet what the new version will be_, notably, you don't know yet whether the new version will be compatible with the previous version or not.