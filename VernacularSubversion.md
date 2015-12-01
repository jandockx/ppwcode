# Introduction #

Subversion is a beautiful tool. You can do al kinds of things with it. But, we need some consistency. In this document we describe how we want to use Subversion (and why).


# Subprojects #

While it was already possible in CVS to use deep directory structures, for a number of reasons it was practical to have different projects as peers on the top level of a repository. Subversion however much more resembles an ordinary file system, where directory structures are really that.

Therefor, we use deep structures to nest projects in a context of related projects.

Most often, different projects or products in a project or product family can be grouped differently according to different criteria. A negative point about any tree structure is that you can only sensibly apply one criterion for grouping (and possibly repeated groupings of secondary criteria at deeper levels).

Which criterion to use, in what order, depends a lot on your particular circumstances.
[ppwcodeSubversion](ppwcodeSubversion.md) explains the criteria used in ppwcode. But this is, by nature, a technical project family. Most of the uses of this vernacular happen in a business context, and not in a technical context. In a business context, we can highly advise to have your Subversion directory structure follow the organizational structure of your enterprise as the first grouping criterion, and not, e.g., the technologies used in a particular project. Technologies are secondary in this context, and not few projects or sub-families of related projects have subprojects in different technologies. It does not make much sense to have those parts in far of branches of the Subversion tree.




# Tags and Branches #

Tags and branches _an sich_ or not a feature of Subversion as they where a feature from CVS. But with the features that do exist in Subversion, notably the "_zeitliche_ copy" ([see Wittgenstein, Tractatus Logico Philosophicus, 6.4.3.1.1](http://www.kfs.org/~jonathan/witt/t64311en.html) :-P) make this, and more, possible in a more transparant way.


## The conventional way ##

The "conventional" way to do tags and branches with Subversion, is to create, under your project root, a `trunk` directoryt, for the, uhm, trunk of development, a `branches` directory for the branches, and a `tags` directory for, you guessed it, the tags:

  * myProjectSvn/
    * trunk/
    * branches/
      * aBranch/
      * anotherBranch/
      * aDevelopmentBranch/
      * 1.0.0PatchBranch/
      * 2.0.0PatchBranch/
      * 2.1.0PatchBranch/
      * 2.2.0PatchBranch/
      * 3.0.0PatchBranch/
    * tags/
      * d1
      * d2
      * d3
      * d4
      * d4bis
      * d5
      * d6
      * ...
      * d1254
      * 1.0.0-alpha-1/
      * 1.0.0-alpha-2/
      * 1.0.0-alpha-3/
      * 1.0.0-alpha-4/
      * 1.0.0-alpha-5/
      * 1.0.0-beta-1/
      * 1.0.0-beta-2/
      * 1.0.0-rc-1/
      * 1.0.0-rc-2/
      * 1.0.0/
      * 1.0.1/
      * 1.0.2/
      * 1.1.0/
      * 1.1.1/
      * 1.1.2/
      * 1.2.0/
      * 2.0.0/
      * 2.0.1/
      * 2.1.0/
      * 2.1.1/
      * 2.1.2/
      * 2.1.3/
      * 2.1.4/
      * 2.1.5/
      * 2.2.0/
      * 2.2.1/
      * 3.0.0/
      * aBranchRoot/
      * anotherBranchRoot/
      * aDevelopmentBranchRoot/
      * 1.0.0PatchBranchRoot/
      * 2.0.0PatchBranchRoot/
      * 2.1.0PatchBranchRoot/
      * 2.2.0PatchBranchRoot/
      * 3.0.0PatchBranchRoot/

Or something like that, probably even much more complex. Note in any case that version numbers do have dots in them, and the `-`-symbol, and start with a number, things that were impossible with CVS.

We contend that working with the tags and branches directory is left-over of the CVS days, and that we can do much better. Convention over configuration, sure, but in practice the "`trunk` - `branches` - `tags`" convention is not very well supported in tools anyway (yes, with the exception of TortoiseSVN).


## The trunk ##

We need a (main) trunk. The (main) trunk is where we do linear development, where we develop new features, where we refactor to new structures, where we go where we intend to go, with foresight.

Note that you might experiment with new features, and you might not want to do that in the trunk, but in a branch. Such experimental branches are covered later.

Note also that in these developments, you often do not yet know what the next version number will be. When developing new features, you might or might not end up with changes that warrant a new major or minor version, or not. You might plan to stay within the same major or minor version, but it is only when you reach a point you want to release, that you can see whether or not you are, e.g., backward compatible. And such concerns are what you base your next version number on. (Unless you have a very high degree of control or discipline, and then nothing here stands in the way of doing that either).


## Nested version tags ##

Now, the first thing that we observe is a hierarchical structure in the version numbers. Whatever your criteria for dot-versions are, you are using a dotted scheme, aren't you? We too. With CVS, the tags and branches name space is flat, and there is nothing you can do about that. With Subversion however, "tags" and "branches" are merely directories, normally a copy-in-time of the trunk. And directory names are relatively free format. So, with this new technology, there is no need to limit ourself to an endless flat list. Instead, we can represent the dot-version hierarchy as a ... hierarchy. This is much easier for developers and users to navigate and understand, and we do not need idiotic prefixes.

And, since next to the trunk (and a `dev` directory, as we will see later), we only have version tags on the top level of our project, there is no need to store the dot-version tags in a separate directory. With the above example, the tree would look like this (the rest of the original tree will dissolve in a moment):

  * myProjectSvn/
    * trunk/
    * 1.n.n/
      * 1.0.n/
        * 1.0.0-alpha/
          * 1.0.0-alpha-1/
          * 1.0.0-alpha-2/
          * 1.0.0-alpha-3/
          * 1.0.0-alpha-4/
          * 1.0.0-alpha-5/
        * 1.0.0-beta/
          * 1.0.0-beta-1/
          * 1.0.0-beta-2/
        * 1.0.0-rc/
          * 1.0.0-rc-1/
          * 1.0.0-rc-2/
        * 1.0.0/
        * 1.0.1/
        * 1.0.2/
      * 1.1.n/
        * 1.1.0/
        * 1.1.1/
        * 1.1.2/
      * 1.2.n/
        * 1.2.0/
    * 2.n.n/
      * 2.0.n/
        * 2.0.0/
        * 2.0.1/
      * 2.1.n/
        * 2.1.0/
        * 2.1.1/
        * 2.1.2/
        * 2.1.3/
        * 2.1.4/
        * 2.1.5/
      * 2.2.n
        * 2.2.0/
        * 2.2.1/
    * 3.n.n.
      * 3.0.n/
        * 3.0.0/
    * branches/
      * aBranch/
      * anotherBranch/
      * aDevelopmentBranch/
      * 1.0.0PatchBranch/
      * 2.0.0PatchBranch/
      * 2.1.0PatchBranch/
      * 2.2.0PatchBranch/
      * 3.0.0PatchBranch/
    * tags/
      * d1
      * d2
      * d3
      * d4
      * d4bis
      * d5
      * d6
      * ...
      * d1254


## Branches and branch root tags ##

Let's turn to branches. With CVS, it was in practice quite important to create a branch at the time your trunk is at the branch point. It was possible, but quite difficult, to create a branch retro-actively. Not so with Subversion. Furthermore, it was also quite important, when branching, to also create a tag at the branch point, i.e., the "root" of the branch (to be able to make sensible diffs when you want to merge a branch into the trunk later on). Not so with Subversion, for the simple reason that a tag is the same as a branch: a copy-in-time of a directory, that knows of itself when it was copied.

When we look at the reason for branching, we find essentially 2 reasons:
  * to evolve the code of an old version, while the trunk already has moved on (maintenance release, bugfix, backporting new features into a backward compatible release, ...)
  * to do extended experiments, try dangerous things, in a sandbox, without interfering with or interference from other developers

Again, these 2 reasons are of a different nature. Here we will talk about **Evolution of an old version (patch branches)**

What happens in practice is that you released, and tagged a version, moved on, and that after some weeks or months, you get a bug report, an enhancement request, or find another reason to do a minor upgrade, on the old code. For a number of reasons you don't want to force your users to use the latest version, but you want to do minimal changes on the old version. Now, with Subversion, if you have a directory that is the copy-in-time of that release, which is since unchanged and should stay unchanged, _it is a breeze to create a new copy of that directory now that you need it_, to work on. And, by the way, for this development, this is really a "trunk".

So, with Subversion, _there is no need to create branches when you release a new version_. The tag suffices. And _there is no need to create a branch root tag either_.

Now, if you store your dot-version tags in a hierachy, it is also clear where this "version trunk" should appear: next to the tag you are copying from. Suppose you have a `major.minor.micro` version number. You will always work _starting from a micro version_, possible 0, since that is what you release. In this kind of work, you know what your intention is: create a bugfix (new micro version, same minor version), or backport a new feature e.g., (new minor version, same major version). (You will, in this mode, never create a new major version).

If your intention is to create a new micro version, you will always want to start from the latest micro version for that minor version. Copy that directory to a `trunk` directory under that minor version, and work there. Or, maybe you did that last time, and there already is a trunk. But in that case, that is exactly where you should continue working. If you started that minor-version-trunk at version n.m.k, you released n.m.k+1, and maybe also, n.m.k+2, and so on. In any case, there is no deeper nesting, so work for all releases later than k for minor version n.m has been done on this trunk, so the trunk, if it exists, is at the latest micro version.

If your intention is to create a new minor version, exactly the same applies, except that you might want to merge changes from the last minor trunk into the major trunk before starting. But let's be frank: new minor versions after the facts are something that rarely happens, if ever. New minor versions most often have new features, and are normally developed in the linear flow of things, in the trunk, like new major versions. A new minor version after the facts is almost always a backport of new features developed and added earlier in a new major version, on the main trunk.

_NOTE: the previous paragraph is not very clear. Proposals?_

For our example, where there were retroactive fixes at some points, but not all, this leads to the following structure:



  * myProjectSvn/
    * trunk/
    * 1.n.n/
      * 1.0.n/
        * 1.0.0-alpha/
          * 1.0.0-alpha-1/
          * 1.0.0-alpha-2/
          * 1.0.0-alpha-3/
          * 1.0.0-alpha-4/
          * 1.0.0-alpha-5/
        * 1.0.0-beta/
          * 1.0.0-beta-1/
          * 1.0.0-beta-2/
        * 1.0.0-rc/
          * 1.0.0-rc-1/
          * 1.0.0-rc-2/
        * 1.0.0/
        * 1.0.1/
        * 1.0.2/
        * trunk
      * 1.1.n/
        * 1.1.0/
        * 1.1.1/
        * 1.1.2/
      * 1.2.n/
        * 1.2.0/
    * 2.n.n/
      * 2.0.n/
        * 2.0.0/
        * 2.0.1/
        * trunk
      * 2.1.n/
        * 2.1.0/
        * 2.1.1/
        * 2.1.2/
        * 2.1.3/
        * 2.1.4/
        * 2.1.5/
        * trunk
      * 2.2.n
        * 2.2.0/
        * 2.2.1/
        * trunk
      * trunk
    * 3.n.n.
      * 3.0.n/
        * 3.0.0/
    * branches/
      * aBranch/
      * anotherBranch/
      * aDevelopmentBranch/
    * tags/
      * d1
      * d2
      * d3
      * d4
      * d4bis
      * d5
      * d6
      * ...
      * d1254

## Experiment branches and development tags ##

How about branches the developer makes, to not interfere with or be interfered by other developers?
And, how about development tags? Normally, you would want to tag your trunk when it is stable during development too, e.g., when you make a snapshot build. This is definitely another "kind" of tag, with another finality and another audience than the dot-version tags. It would make sense to gather them together, these tags, and branches, separate from the others.

Well, given the previous, this is actually simple: these kinds of tags always happen in the context of _a_ trunk. We suggest putting a `dev` directory next to the trunk, and creating the development tags there. Instead of a sequence number, we will use the date-hour format, because this makes it easier to situate the tag in time, and to relate it to Maven-generated snapshots build, that use the same format. With that, our Subversion tree will look like this:

  * myProjectSvn/
    * trunk/
    * dev
      * d20040712-1245
      * d20040712-1618
      * d20040713-2314
      * ...
      * d20041114-0136
    * 1.n.n/
      * 1.0.n/
        * 1.0.0-alpha/
          * 1.0.0-alpha-1/
          * 1.0.0-alpha-2/
          * 1.0.0-alpha-3/
          * 1.0.0-alpha-4/
          * 1.0.0-alpha-5/
        * 1.0.0-beta/
          * 1.0.0-beta-1/
          * 1.0.0-beta-2/
        * 1.0.0-rc/
          * 1.0.0-rc-1/
          * 1.0.0-rc-2/
        * 1.0.0/
        * 1.0.1/
        * 1.0.2/
        * trunk
        * dev
          * d20060113-0734
          * d20060113-0934
          * d20060116-1228
          * d20060201-0714
          * d20060527-1055
          * d20060528-1321
      * 1.1.n/
        * 1.1.0/
        * 1.1.1/
        * 1.1.2/
      * 1.2.n/
        * 1.2.0/
    * 2.n.n/
      * 2.0.n/
        * 2.0.0/
        * 2.0.1/
        * trunk
        * dev
          * ...
      * 2.1.n/
        * 2.1.0/
        * 2.1.1/
        * 2.1.2/
        * 2.1.3/
        * 2.1.4/
        * 2.1.5/
        * trunk
        * dev
          * ...
      * 2.2.n
        * 2.2.0/
        * 2.2.1/
        * trunk
        * dev
          * ...
      * trunk
        * dev
          * ...
    * 3.n.n.
      * 3.0.n/
        * 3.0.0/
    * branches/
      * aBranch/
      * anotherBranch/
      * aDevelopmentBranch/

Note that there is no more `tags`-directory. Note also that, after a while, the development tags loose their meaning. In general, development tags only have relevance for a few days or weeks. Certainly, after a release, the path via development snapshots towards that release is largely irrelevant. On the other hand, there might be many, very many development tags.

Different from CVS, in Subversion, it is also a breeze to clean up. At certain points it might be interesting to just "delete" (not really, it is a version control system) older development tags. That's another good reason to use a date-hour identification instead of a sequence. The tags left after such a delete will look less awkward.

Finally, we are left with the development branches. What better place to put those than also in the `dev` directory? And, you know what, this also solves another problem: very often, during experiments, which might be started by a subteam, you want to branch further (getting in the mood), and you often also want to issue development tags on a development branch. No problem: we can use the same approach as before: we create a directory for the experiment development branch, and work in a `trunk`. That again leaves room voor development tags in the branch directory, (there is no more reason for a `dev` directory: this is all "dev"), next to directories for nested branches.

Thus, we reach or final proposal, in the following tree for our example.


  * myProjectSvn/
    * trunk/
    * dev
      * aDevelopmentBranch/
        * d20040911-1141
        * d20040918-1024
        * d20040922-0852
        * anotherBranch/
          * d20040707-1637
          * trunk
        * trunk
      * aDevelopmentBranchToo/
        * trunk
      * aDevelopmentBranchThree/
        * d20040807-1135
        * d20040813-1452
        * trunk
      * d20040712-1245
      * d20040712-1618
      * d20040713-2314
      * ...
      * d20041114-0136
    * 1.n.n/
      * 1.0.n/
        * 1.0.0-alpha/
          * 1.0.0-alpha-1/
          * 1.0.0-alpha-2/
          * 1.0.0-alpha-3/
          * 1.0.0-alpha-4/
          * 1.0.0-alpha-5/
        * 1.0.0-beta/
          * 1.0.0-beta-1/
          * 1.0.0-beta-2/
        * 1.0.0-rc/
          * 1.0.0-rc-1/
          * 1.0.0-rc-2/
        * 1.0.0/
        * 1.0.1/
        * 1.0.2/
        * trunk
        * dev
          * d20060113-0734
          * d20060113-0934
          * d20060116-1228
          * d20060201-0714
          * d20060527-1055
          * d20060528-1321
          * aBranch/
            * trunk/
            * d20060415-1605
            * d20060416-1526
            * d20060422-0459
            * d20060430-1212
      * 1.1.n/
        * 1.1.0/
        * 1.1.1/
        * 1.1.2/
      * 1.2.n/
        * 1.2.0/
    * 2.n.n/
      * 2.0.n/
        * 2.0.0/
        * 2.0.1/
        * trunk
        * dev
          * ...
      * 2.1.n/
        * 2.1.0/
        * 2.1.1/
        * 2.1.2/
        * 2.1.3/
        * 2.1.4/
        * 2.1.5/
        * trunk
        * dev
          * ...
      * 2.2.n
        * 2.2.0/
        * 2.2.1/
        * trunk
        * dev
          * ...
      * trunk
        * dev
          * ...
    * 3.n.n.
      * 3.0.n/
        * 3.0.0/

Note that, like development tags, you might also delete development branches when they are no longer needed.


## Finally: relax ##

Finally: relax. This is a versioning system so you can always roll back changes. And appart from a "copy-in-time", Subversion features an other enormously important feature that is different from what CVS offers: a "move-in-time". If you are not happy with a given structure, you can simply rename directories or move entire substructures of your repository to another place, without loss of history or functionality. This means that the structure of you Subversion repository can grow organically, without much problem.

The structure above might serve as a starting point, but on the other hand, you might consider it too complex to start with. No problem. Just start, and see where your experience leads you. Our experience led here, and in PeopleWare projects and ppwcode, this is our Subversion structure vernacular.