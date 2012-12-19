/*
 Copyright 2012 - $Date $ by PeopleWare n.v.

 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at

 http://www.apache.org/licenses/LICENSE-2.0

 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
 */

define(["dojo/main", "ppwcode/contracts/doh", "../ArraySet"],
  function(dojo, doh, ArraySet) {

    doh.register(ArraySet.prototype.declaredClass, [

      function testConstructor1a() {
        var subject = new ArraySet();
        doh.invars(subject);
        doh.is(Object, subject.getElementType());
        doh.is(0, subject.getSize());
        doh.t(subject.getEquivalence());
      }

    ]);

  }
);