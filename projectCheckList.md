# Introduction #

Projects should be clean and neat, certainly before they are released. But what does that mean, "clean and neat"? Here is a checklist (continuously growing).


# Subversion #

  * At the top level, there **must** be svn:ignore svn properties set for
    * eclipse project files (`.project`, `.classpath`, `.settings`)
    * the Maven 2 `target` directory
  * All text files **must** have keyword substitution enabled for `$Date$`, `$Revision$` and `$HeadURL$`.


# Maven 2 #

  * Projects _should_ have a Maven 2 POM
  * The POM **must** contain all relevant developer and contributor information, which is probably a delta with its parent pom
  * For Java artifacts (jar, ear, war, ...) the POM **must** be copied into `META-INF`.


# Changes #

  * All projects **must** keep a changes file, that documents the changes from version to version. The changes file is part of the Maven 2 documentation site, and is built from src/changes/changes.xml
  * A separate release notes document may be added to the documentation site and the distribution when there are important things to say to people who used a previous version


# Dependencies #

  * ppwcode projects may never contain dependencies directly. The Maven 2 dependencies mechanism **must** be used for dependencies
  * Dependencies in the Maven 2 POM **must** not be too strict. Use the `scope` attribute as intended.
  * Dependencies in the Maven 2 POM _should_ not have a version (there are exceptions). Versions for dependencies should be listed in the `depedencyManagement` section of a parent POM. In the case of Java ppwcode projects, this is done in the `ppwcode-ppwcode-library-parent-pom`.


# License #

  * All projects **must** be released under the Apache License v2
  * All projects **must** have a `LICENSE` file in their root. A copy of this file can be found in the root of the repo or at http://www.apache.org/licenses/LICENSE-2.0.txt.
  * All projects _should_ have a `NOTICE` file in their root, covering attributions. An example of a notice file can be found at http://www.apache.org/licenses/example-NOTICE.txt. Sadly, this highly overlaps with the developers and contributers section of the Maven 2 POM. The following text might suffice:
```
This product is developed by PeopleWare n.v. and partners.
The developers and contributors are mentioned exhaustively in the file pom.xml
and at the project documentation site (Project Team).
```
  * The following notice **must** appear in **every text file of the project**:
```
   Copyright [inceptionYear] PeopleWare n.v.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
```
> This is applied to text files in different ways, depending on the target technology:
    * [Java source code](licenseNoticeJavaSource.md)
    * [Java properties files](licenseNoticeJavaProperties.md)
    * [XML files](licenseNoticeXml.md)
    * [CSS files](licenseNoticeCss.md)
    * [HTML files](licenseNoticeHtml.md)
  * The `[inceptionYear]` **must** be the year the particular project has started, not the ppwcode project as a whole (2008) or the original ppw-libraries project (2004)
  * A CLA **must** be in the possession of PeopleWare for all developers and contributors found in the POM and in the Subversion history.
  * For Java artifacts (jar, ear, war, ...) the `LICENSE` and `NOTICE` files **must** be copied into `META-INF`.


# Documentation #

  * For Java projects, there **must** be an `overview.html`, and there _should_ be a `package.html` for each package.

# Before you release #

  * All `MUDO`'s **must** be resolved
    * If a `MUDO` is not resolved, it **must** be changed to a `TODO` (and if that is possible, it means that it wasn't a `MUDO` to begin with)
  * All `TODO`'s _should_ be resolved
  * For Java projects, the Checkstyle report (with the ppwcode settings) _should_ be empty.
  * The project **must** be listed in the Issue labels (so that people can post issues against it). This list is edited in the Administration / Issues section of Google Code.

# Releases #

  * Releases **must** follow the appropriate versioningScheme.
  * Releases **must** be tagged in the Subversion repository.
  * Releases **must** be announced at the Announcement mailing list.
  * Releases are only available via the Maven 2 repository at http://www.ppwcode.org/java/maven2repo/


# References on project check lists #
http://incubator.apache.org/guides/releasemanagement.html is an interesting document on this topic. The document is being updated continuously, it seems.