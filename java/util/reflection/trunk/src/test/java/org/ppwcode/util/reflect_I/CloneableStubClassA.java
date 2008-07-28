/*<license>
Copyright 2004 - $Date: 2008-07-28 14:47:41 +0200 (Mon, 28 Jul 2008) $ by PeopleWare n.v..

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

package org.ppwcode.util.reflect_I;


import static org.ppwcode.vernacular.exception_II.ProgrammingErrors.unexpectedException;


public class CloneableStubClassA implements Cloneable {

  @Override
  public final CloneableStubClassA clone() {
    CloneableStubClassA result = null;
    try {
      result = (CloneableStubClassA)super.clone();
    }
    catch (CloneNotSupportedException exc) {
      unexpectedException(exc, "we are Cloneable");
    }
    return result;
  }

}

