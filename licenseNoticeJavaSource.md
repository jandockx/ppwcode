# Introduction #

The Apache License v2 is applied to pieces of text by adding a notice at the top of the file. This page describes how this is done exactly for Java source code files.

# Details #

For source code Java files, you **must** the following mantra, at the top of the file (line 1):
```
/*<license>
Copyright [inceptionYear] - $Date$ by PeopleWare n.v..

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
```

Furthermore, the following annotations **must** be added to each class, to carry the copyright and license information into the class file:
```
@Copyright("[inceptionYear] - $Date$, PeopleWare n.v.")
@License(APACHE_V2)
@SvnInfo(revision = "$Revision$",
         date     = "$Date$")
public class _MyClass_ ...
```


It is not necessary to add the `HeadURL` keyword to Java files: a Java source file can easily be identified by the package and class name.