# Introduction #

Subversion is a beautiful tool. You can do al kinds of things with it. But, we need some consistency. In this document we describe how we want to use Subversion in ppwcode.

First of all, we will follow our own [Subversion vernacular](VernacularSubversion.md). On this page, we only mention things special for this project.

# Subversion directory structure #

Our [Subversion vernacular](VernacularSubversion.md) says it is a good idea to use a deep nested directory structure, instead of a flat list of projects. In ppwcode, the following structure should be used:

## Nominal structure ##

On the first level, we discern between different targets. For libraries, these targets are the technologies (Java, .net, ...). Then, within such a target directory, further structuring depends on the vernaculars of those targets. On this level you also find the general projects `licenses`, `logo`, `website` and `wiki` (which must be here to work with Google Code).

## Special subtrees ##

Special subtrees are available in the svn repository with names starting with "`_`".

  * `_purgatory`: here old versions of projects of ours reside (which were imported at the start of ppwcode from the old CVS repository) for reactivation; this directory will normally go away after a while
  * `_tmp`: just what it says: a home for temporary storage; please clean up after yourself
  * `_sandbox`: where new projects live, before they are mature enough to know there way in the main structure
  * `_renditions`: where the above directories are intended mainly for developers and contributors, this one is for everybody; it contains different "renditions" of this repository, different ways of presenting it, for different audiences or tools



# Tags and Branches #

Tags and branches will be done as described in the [Subversion vernacular](VernacularSubversion.md).