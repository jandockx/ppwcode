# Introduction #

Java is just a language. A language leaves a very broad spectrum of choices in coding. Our vernaculars try to make a lot of decisions, narrowing down the bore of chosing for developers, and  gently guiding developers to consistency.

For Java code, most of the design-related things are either expressed in framework or library code, and most style-related things are taken down in the checkstyle and other checks and reports.

There are however a number of things that are still open. These will be mentioned in this wiki.



# Test classes #

First of all, whatever rules we whip up, **none of this ever applies to test classes**. You can follow a similar style for test classes if you want, but frankly, we don't care. As long as they work, that's fine.

We are very interested in an automated, contract-based test setup (JML, Toryt, ...?). Other than that tests are difficult, complex and chaotic. We certainly are not going to ask you to write testing code that has a higher quality than the actual code.



# Testing #

That said, it should be clear that every piece of production code in this collection **must** be tested, one way or another, and not a bit, but deep and thorough.



# Collections of utility methods #

Very often we provide, or you have to write, collections of utility methods. The choice to make is often to either provide them as _static methods_ in a class that is merely used as a container for these methods, or to provide them as _dynamic methods_. The first is the traditional Java pattern (see, e.g., java.util.Math). The second approach was introduced for Struts, and we find it in e.g. Apache Commons BeanUtils. The reason for it is that from JSP code you could not initially call static methods (through EL, you can nowadays). The class instance is used as "middleware" (_talk to the instance, 'cause the class ain't listening_). JSF is a fall back: in JSF 1.1, you cannot access static methods from the pages either.

We consider as a given, as part of the definition of a helper method, that it is stateless.

Using static methods gathered in a class is a misuse of the class concept.  Conceptually, these methods are more global methods, which would be defined better at the level of the package for example. Instances of these class are senseless.

Using dynamic methods gives the possibility of dynamic binding, but this writer knows of no examples in practice for helper methods. To reuse code for helper methods, delegation is most often used. One could defend that Formatters or Converters are examples, but in practice people look at utility objects like that differently, this writer feels: helper code ends up in an object if the helper code is stateful. The negative point of using dynamic methods for helper code is that you have to initialize an object before you can call the method, although helper methods are stateless. In all practical examples of dynamic helper methods known to this writer, the only reason for making them dynamic is to be able to use them in JSP code.

The vernacular we apply is to always write stateless helper code in **static methods**, gathered in a class. Potentially, a second class can be added to "dynamify" the methods for use in JSP code (this is the reverse of the technique used in Apache Commons code).

The following rules **must** be followed for helper methods:
  * Helper methods **must** be stateless
  * Helper methods **must** be coded in static methods, gathered in a class for this purpose
  * The name of the class gathering the helper methods **must** end in `...Helper`
  * The class gathering the helper methods **must** be defined `final`
  * The class gathering the helper methods **must** feature one constructor, that has no arguments (default constructor), has an empty body, documented to be empty on purpose, and is declared `private`
  * The name of the helper methods _should_ be short and to the point, and not apply bean property semantics (no `get...` and `set...`)
  * In naming of helper methods, overloading may be applied when suitable (this is a no-no in true object oriented code)