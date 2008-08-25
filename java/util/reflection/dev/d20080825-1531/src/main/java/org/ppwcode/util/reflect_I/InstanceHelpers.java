/*<license>
Copyright 2004 - $Date: 2008-07-31 01:17:04 +0200 (Thu, 31 Jul 2008) $ by PeopleWare n.v..

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


import static org.ppwcode.metainfo_I.License.Type.APACHE_V2;
import static org.ppwcode.vernacular.exception_II.ProgrammingErrors.unexpectedException;

import org.ppwcode.metainfo_I.Copyright;
import org.ppwcode.metainfo_I.License;
import org.ppwcode.metainfo_I.vcs.SvnInfo;
import org.ppwcode.vernacular.exception_II.ProgrammingErrors;


/**
 * <p>Utility methods for instance reflection. Use these methods if you are interested in the result of reflection,
 *   and not in a particular reason why some reflective inspection might have failed. The ppwcode exception
 *   vernacular is applied.</p>
 *
 * @author    Jan Dockx
 * @author    PeopleWare n.v.
 */
@Copyright("2004 - $Date: 2008-07-31 01:17:04 +0200 (Thu, 31 Jul 2008) $, PeopleWare n.v.")
@License(APACHE_V2)
@SvnInfo(revision = "$Revision: 1978 $",
         date     = "$Date: 2008-07-31 01:17:04 +0200 (Thu, 31 Jul 2008) $")
public class InstanceHelpers {

  private InstanceHelpers() {
    // NOP
  }

  // MUDO tests and contracts
  public static <_Class_> _Class_ newInstance(Class<_Class_> clazz) {
    ProgrammingErrors.preArgumentNotNull(clazz, "clazz");
    _Class_ result = null;
    try {
      result = clazz.newInstance();
    }
    catch (InstantiationException exc) {
      unexpectedException(exc, "trying to instantiage " + clazz + " with default constructor");
    }
    catch (IllegalAccessException exc) {
      unexpectedException(exc, "trying to instantiage " + clazz + " with default constructor");
    }
    catch (ExceptionInInitializerError err) {
      unexpectedException(err, "trying to instantiage " + clazz + " with default constructor");
    }
    catch (SecurityException exc) {
      unexpectedException(exc, "trying to instantiage " + clazz + " with default constructor");
    }
    return result;
  }

}
