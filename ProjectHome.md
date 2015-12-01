This project contains several distinct libraries, in different technologies, developed by [PeopleWare](http://www.peopleware.be) and its clients. There are generally 3 types of libraries in this project.

  * Libraries whose prime intention is to document and possibly enforce particular coding styles and idioms. Most technologies offer much leeway in choosing a personal style. This is our choice. Think convention over configuration.
  * Libraries whose prime intention is to glue together existing libraries and frameworks. For Java, think using Hibernate with Struts, or Hibernate with JSF, or JPA with dojo. For .net, think combining ASP and LINQ ...
  * Utility libraries. Stuff that should have been in the Java API, or .net, or ... to begin with, but wasn't for some reason. Think Commons. If some larger library would want to accept the code, it would move there.

# Choice annoys #
Most general libraries out there offer the possibility for customization, offer multiple choices, multiple ways of doing things, etc. The idea behind that is to make the library useful to as many people and projects as possible. That is a sensible way to do things for general libraries.

But in the end, somebody has to make those choices, and commit to a given approach. The libraries in this project are geared to that end: take away the need to choose, make (and possibly enforce) well-thought out choices for you. This is done in the context of real-life projects we encounter.

Our business mainly concerns information-handling administrative applications. The choices we make might not apply to you, but they do apply to a great number of projects.

# Code needed in the field #
During most of our projects, these libraries are used and refined. Code is developed in a first project on a need-basis, with generalization in mind. On later projects, the code is refined and generalized as needed.

Since [PeopleWare](http://www.peopleware.be) does projects for different clients, the effort in developing these general libraries is shared. A later client profits from the efforts done for an earlier client, but in return accepts that evolutions in the libraries are used for future clients. Often, we also work together with developers that are employees of our client, and they also contribute to this code collection.

# Open Source #
By putting this code under a liberal Open Source license, we avoid difficult intellectual rights issues. Furthermore, we believe that our choices, which have grown out of real-project needs and concerns, will also appeal to other developers and organizations out there. If you want to join the effort, please contact us.

At certain times, parts of the code of this project will be offered to other Open Source projects for inclusion.

# Project web site #
http://www.ppwcode.org/

&lt;wiki:gadget url="http://www.ohloh.net/p/15560/widgets/project\_basic\_stats.xml" height="220" border="1"/&gt;