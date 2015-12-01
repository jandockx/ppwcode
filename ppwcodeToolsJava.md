# Introduction #

This page discusses tooling used for ppwcode Java projects. The tools used for Java projects are an extension of the [general tools used](ppwcodeTools.md).


# Details #

## Build ##

ppwcode projects use [Maven 2](http://maven.apache.org/) as the main development and build tool. You'd best install Maven 2 before you checkout the ppwcode code. It's easier that way :-).

## IDE ##

You can use any IDE you want. It doesn't matter, since we standardize on [Maven 2](http://maven.apache.org/). Yet, be aware, that no IDE specific files should be add to svn (unless there is a very good reason, which is discussed at the developer list first).

## Other ##

A lot of other tools are used ([PMD](http://pmd.sourceforge.net/), [Checkstyle](http://checkstyle.sourceforge.net/), ...), but these are not discussed here, since they are defined and configured in Maven 2 parent poms which we use. See there for more information.