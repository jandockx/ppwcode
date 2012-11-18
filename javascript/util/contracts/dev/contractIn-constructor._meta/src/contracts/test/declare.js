define(["dojo/main", "util/doh/main", "contracts/declare"],
  function(dojo, doh, subjectDeclare) {

    var booleanValue = true;
    var stringValue1 = "A property value";
    var stringValue2 = "Another property value";
    var numberValue1 = 3.14;
    var numberValue2 = Math.sqrt(2);
    var arrayValue = ["string value", -88284.994, null, [4, 5, 9], { propInObjectInArray: true}, function() { return true; }];
    var dateValue = new Date();
    var objectValue = { propInObjectinObject: 8 };
    var functionValue = function() {
      // object must have a property "stringProperty"
      return this.stringProperty;
    };
    var functionValuePost = [
      function(result) { return result === this.stringProperty; }
    ];
    var toStringMethod = function() {
      // object must have a method "functionProperty"
      return this.functionProperty();
    };
    var constructor = function() {
      this.stringProperty = stringValue2;
      this.numberProperty =  numberValue2;
    };
    var constructorPost = [
      function() { return this.nullProperty === null; },
      function() { return this.booleanProperty === booleanValue; },
      function() { return this.stringProperty === stringValue2; },
      function() { return this.numberProperty === numberValue2; },
      function() { return this.arrayProperty === arrayValue; },
      function() { return this.dateProperty === dateValue; },
      function() { return this.objectProperty === objectValue; },
      function() { return this.functionProperty === functionValue; },
      function() { return this.toString === toStringMethod; },
      function() { return this.constructor === constructor; },
      function() { return this.oneMoreMethod === functionValue; }
    ];


    function testResultInstanceProperty(resultInstance, propertyName, expectedValuePrototype, expectedValueInstance) {
      var resultPrototype = Object.getPrototypeOf(resultInstance);
      doh.t(resultPrototype.hasOwnProperty(propertyName));
      doh.is(expectedValuePrototype, resultPrototype[propertyName]);
      doh.is(expectedValueInstance, resultInstance[propertyName]);
    }

    var _objectProto = Object.prototype;
    var _urToStringF = _objectProto.toString;

    function _urToString(o) {
      return _urToStringF.call(o);
    }

    function _isFunction(candidateFunction) {
      return _urToString(candidateFunction) === "[object Function]";
    }

    function _isArray(candidateArray) {
      return _urToString(candidateArray) === "[object Array]";
    }

    function hasContract(instance, methodName, pre, impl, post, exc) {
      doh.t(instance);
      var cstr = instance.constructor;
      doh.t(cstr);
      var meta= cstr._meta;
      doh.t(meta);
      var contract = meta.contract;
      doh.t(contract);
      var invariants = contract.invariants;
      doh.t(invariants);
      doh.t(_isArray(invariants));
      var methodContract = contract[methodName];
      doh.t(methodContract);
      doh.t(! _isArray(methodContract) && ! _isFunction(methodContract));
      doh.t(methodContract.pre);
      doh.t(_isArray(methodContract.pre));
      doh.t(methodContract.impl);
      doh.t(_isFunction(methodContract.impl));
      doh.t(methodContract.post);
      doh.t(_isArray(methodContract.post));
      doh.t(methodContract.excp);
      doh.t(_isArray(methodContract.excp));
    }

    function isString(o) {
      return typeof o === "string" || (typeof o === "object" && o.constructor === String);
    }

    doh.register("be/ppwcode/util/contracts/I/declare", [

      function testDoh() {
        console.log("test ran");
      },

      function stringType() {
        var aString = "this is a test string";
        doh.is("string", typeof aString);
        doh.t(isString(aString));
      },

      function testSimpleDeclare() {
        var Result = subjectDeclare(null, {
          nullProperty : null,
          booleanProperty : booleanValue,
          stringProperty : stringValue1,
          numberProperty : numberValue1,
          arrayProperty : arrayValue,
          dateProperty : dateValue,
          objectProperty : objectValue,
          functionProperty : functionValue,
          toString: toStringMethod,
          constructor: constructor
        });
        var resultInstance = new Result();
        doh.is(Result, resultInstance.constructor);
//        doh.is(Object.getPrototypeOf(Result), Object.getPrototypeOf(resultInstance));
        testResultInstanceProperty(resultInstance, "nullProperty", null, null);
        testResultInstanceProperty(resultInstance, "booleanProperty", booleanValue, booleanValue);
        testResultInstanceProperty(resultInstance, "stringProperty", stringValue1, stringValue2);
        testResultInstanceProperty(resultInstance, "numberProperty", numberValue1, numberValue2);
        testResultInstanceProperty(resultInstance, "arrayProperty", arrayValue, arrayValue);
        testResultInstanceProperty(resultInstance, "dateProperty", dateValue, dateValue);
        testResultInstanceProperty(resultInstance, "objectProperty", objectValue, objectValue);
        testResultInstanceProperty(resultInstance, "functionProperty", functionValue, functionValue);
        testResultInstanceProperty(resultInstance, "toString", toStringMethod, toStringMethod);
        var resultPrototype = Object.getPrototypeOf(resultInstance);
        doh.t(resultPrototype.hasOwnProperty("constructor"));
        doh.is(resultInstance.constructor, resultPrototype.constructor)
      },

      function testContractDeclare() {
        var Result = subjectDeclare(null, {
          invariants : [],
          nullProperty : null,
          booleanProperty : booleanValue,
          stringProperty : stringValue1,
          numberProperty : numberValue1,
          arrayProperty : arrayValue,
          dateProperty : dateValue,
          objectProperty : objectValue,
          functionProperty : functionValue,
          toString : toStringMethod,
          constructor : {
            pre  : [],
            impl : constructor,
            post : [],
            excp : []
          },
          oneMoreMethod : {
            pre  : [],
            impl : functionValue,
            post : [],
            excp : []
          }
        });
        var resultInstance = new Result();
        doh.is(Result, resultInstance.constructor);
//        doh.is(Object.getPrototypeOf(Result), Object.getPrototypeOf(resultInstance));
        testResultInstanceProperty(resultInstance, "nullProperty", null, null);
        testResultInstanceProperty(resultInstance, "booleanProperty", booleanValue, booleanValue);
        testResultInstanceProperty(resultInstance, "stringProperty", stringValue1, stringValue2);
        testResultInstanceProperty(resultInstance, "numberProperty", numberValue1, numberValue2);
        testResultInstanceProperty(resultInstance, "arrayProperty", arrayValue, arrayValue);
        testResultInstanceProperty(resultInstance, "dateProperty", dateValue, dateValue);
        testResultInstanceProperty(resultInstance, "objectProperty", objectValue, objectValue);
        testResultInstanceProperty(resultInstance, "functionProperty", functionValue, functionValue);
        testResultInstanceProperty(resultInstance, "toString", toStringMethod, toStringMethod);
        var resultPrototype = Object.getPrototypeOf(resultInstance);
        doh.t(resultPrototype.hasOwnProperty("constructor"));
        doh.is(resultInstance.constructor, resultPrototype.constructor); // dojo constructor wraps around our function
        testResultInstanceProperty(resultInstance, "oneMoreMethod", functionValue, functionValue);
        hasContract(resultInstance, "constructor", [], constructor, [], []);
        hasContract(resultInstance, "oneMoreMethod", [], functionValue, [], []);
      },

      function testContractDeclareWithConditions() {
        var resultInvariants = [
          function() { return this.nullProperty != undefined; },
          function() { return booleanProperty != undefined; },
          function() { return stringProperty != undefined; },
          function() { return numberProperty != undefined; },
          function() { return arrayProperty != undefined; },
          function() { return dateProperty != undefined; },
          function() { return objectProperty != undefined; },
          function() { return functionProperty != undefined; },
          function() { return toString != undefined; },
          function() { return constructor != undefined; },
          function() { return oneMoreMethod != undefined; }
        ];
        var Result = subjectDeclare(null, {
          invariants : resultInvariants,
          nullProperty : null,
          booleanProperty : booleanValue,
          stringProperty : stringValue1,
          numberProperty : numberValue1,
          arrayProperty : arrayValue,
          dateProperty : dateValue,
          objectProperty : objectValue,
          functionProperty : functionValue,
          toString : toStringMethod,
          constructor : {
            pre  : [],
            impl : constructor,
            post : constructorPost,
            excp : []
          },
          oneMoreMethod : {
            pre  : [],
            impl : functionValue,
            post : functionValuePost,
            excp : []
          }
        });
        var resultInstance = new Result();
        doh.is(Result, resultInstance.constructor);
//        doh.is(Object.getPrototypeOf(Result), Object.getPrototypeOf(resultInstance));
        testResultInstanceProperty(resultInstance, "nullProperty", null, null);
        testResultInstanceProperty(resultInstance, "booleanProperty", booleanValue, booleanValue);
        testResultInstanceProperty(resultInstance, "stringProperty", stringValue1, stringValue2);
        testResultInstanceProperty(resultInstance, "numberProperty", numberValue1, numberValue2);
        testResultInstanceProperty(resultInstance, "arrayProperty", arrayValue, arrayValue);
        testResultInstanceProperty(resultInstance, "dateProperty", dateValue, dateValue);
        testResultInstanceProperty(resultInstance, "objectProperty", objectValue, objectValue);
        testResultInstanceProperty(resultInstance, "functionProperty", functionValue, functionValue);
        testResultInstanceProperty(resultInstance, "toString", toStringMethod, toStringMethod);
        var resultPrototype = Object.getPrototypeOf(resultInstance);
        doh.t(resultPrototype.hasOwnProperty("constructor"));
        doh.is(resultInstance.constructor, resultPrototype.constructor); // dojo constructor wraps around our function
        testResultInstanceProperty(resultInstance, "oneMoreMethod", functionValue, functionValue);
        hasContract(resultInstance, "constructor", [], constructor, constructorPost, []);
        hasContract(resultInstance, "oneMoreMethod", [], functionValue, functionValuePost, []);
        doh.is(resultInvariants, resultInstance.constructor._meta.contract.invariants);
      },

      function realTest() {
        var now = new Date();

        var Person = subjectDeclare(null, {
          invariants : [
            function() { return this.hasOwnProperty("firstName"); },
            function() { return this.firstName; },
            function() { return isString(this.firstName); },
            function() { return this.hasOwnProperty("lastName"); },
            function() { return this.lastName; },
            function() { return isString(this.lastName); },
            function() { return this.hasOwnProperty("dob"); },
            function() { return this.dob; },
            function() { return this.dob instanceof Date; },
            function() { return now > this.dob; },
            function() { return this.age; },
            function() { return this.age instanceof Function; }
          ],
          constructor : {
            pre  : [
              function() { return first },
              function() { return isString(first); },
              function() { return last },
              function() { return isString(last); },
              function() { return dob },
              function() { return dob instanceof Date}
            ],
            impl : function(first, last, dob) {
              if (now <= dob) {
                throw "dob must be in the past (" + dob + ")";
              }
              this.firstName = first;
              this.lastName = last;
              this.dob = dob;
            },
            post : [
              function(first, last, dob) { return this.firstName === first; },
              function(first, last, dob) { return this.lastName === last; },
              function(first, last, dob) { return this.dob === dob; }
            ],
            excp : [
              function(first, last, dob, exc) { return isString(exc); },
              function(first, last, dob, exc) { return exc === "dob must be in the past (" + dob + ")"; },
              function(first, last, dob, exc) { return now <= dob; }
            ]
          },
          age : {
            pre  : [],
            impl : function() {
              return (new Date(now.getTime() - this.dob.getTime())).getFullYear() - 1970;
            },
            post : [
              function(result) {
                return result === (now.getFullYear() - this.dob.getFullYear() +
                ((now.getMonth() < this.dob.getMonth() ||
                  (now.getMonth() === this.dob.getMonth() && now.getDate() < this.dob.getDate())) ? 1 : 0));
              }
            ],
            excp : []
          }
        });
        // var pInstance = new (Object.getPrototypeOf(Person).cPrePostInvar(Person))("Jan", "Dockx", new Date(1966, 10, 3));
        var pInstance = new Person("Jan", "Dockx", new Date(1966, 9, 3));
        var age = pInstance.cPrePostInvar(pInstance.age)();
      }

    ]);

  }
);