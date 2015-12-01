# Introduction #

The Apache License v2 is applied to pieces of text by adding a notice at the top of the file. This page describes how this is done exactly for HTML files.

# Details #

For HTML files, you **must** use the following comment, immediately following the root tag (if you add comments outside the root tag, your file is no longer well-formed XML):
```
  <!--
  Copyright [inceptionYear] - $Date$ by PeopleWare n.v.

  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at

      http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
  -->
```

Furthermore, you **must** add a footer, with the anchors adapted to refer correctly to the 'LICENSE' and 'NOTICE' files:
```
<div class="footer">
  <p>Copyright PeopleWare n.v., [inceptionYear] - $Date$.
    Released under the <a href="LICENSE">Apache License, v2</a>.
    See <a href="NOTICE">NOTICE</a> for attribution.</p>
  <p>$HeadURL$, $Revision$</p>
</div>
```