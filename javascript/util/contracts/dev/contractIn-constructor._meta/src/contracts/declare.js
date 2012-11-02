define(["dojo/_base/declare"], function(dojoDeclare) {

  var _invariantPropertyName = "invariants";
  // hardcoded name of type invariant property

  var _contractMethodImplName = "impl";
  var _contractMethodPreName = "pre";
  var _contractMethodPostName = "post";
  var _contractMethodExcpName = "excp";
  var _contractInMetaName = "contract";

  var _callWithContractsMethodName_Pre = "cPre";
  var _callWithContractsMethodName_PrePost = "cPrePost";
  var _callWithContractsMethodName_PrePostInvar = "cPrePostInvar";

  function _errorMsgContractMethod(propName) {
    return "ContractMethod '" + propName + "' not well-formed: ";
  }

  function _crackParameters(className, superclass, props) {
    // summary:
    //    Returns an object with properties "className", "superclass" and "props",
    //    taking into account that in the arguments className is optional; that
    //    superclass may be null, a (constructor) Function, or an Array of
    //    (constructor) Functions; that props is an optional object.
    // returns: Object
    // description:
    //    When there is no className, the first argument is intended to be
    //    superclass, and the second argument is intended to be props.
    //    The resulting object is then
    //    {"className" : "", "superclass" : arguments[0], "props" : arguments[1]}.
    //    When there is a className, the resulting object is
    //    {"className" : arguments[0], "superclass" : arguments[1], "props" : arguments[2]}.

    // copied from dojo/_base/declare.js - declare - first 6 lines of code

    if(typeof className != "string"){
      props = superclass;
      superclass = className;
      className = "";
    }
    props = props || {};

    return {"className" : className, "superclass" : superclass, "props" : props};
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

  function _isFunctionOrArray(candidate) {
    return _isFunction(candidate) || _isArray(candidate);
  }

  function _checkInvariantsWellFormed(candidateInvariants) {
    // summary:
    //    Void method, that throws an error if the candidateInvariants are not
    //    well-formed invariants.

    // TODO
  }

  function _checkPresWellFormed(candidatePres) {
    // summary:
    //    Void method, that throws an error if the candidatePres are not
    //    well-formed preconditions.
    //    Preconditions must have the same argument names as the impl.

    // TODO
  }

  function _checkPostsWellFormed(candidatePosts) {
    // summary:
    //    Void method, that throws an error if the candidatePosts are not
    //    well-formed postconditions.
    //    Postconditions must have the same argument names as the impl, and have one
    //    extra (last) argument, the result.

    // TODO
  }

  function _checkExcpsWellFormed(candidateExcps) {
    // summary:
    //    Void method, that throws an error if the candidateExcps are not
    //    well-formed exception conditions.
    //    Exception conditions must have the same argument names as the impl, and have one
    //    extra (last) argument, the exception, which is not null.

    // TODO
  }

  function _isContractMethod(candidateCm, propName) {
    // summary:
    //    Boolean method that returns true if candidateCm is of (duck) type
    //    ContractMethod. If it is close, but not well-formed, we throw an
    //    error.
    // description:
    //    This method returns true if:
    //    - candidateCm is an Object, i.e., not a function or an Array
    //    - candidateCm has one of the properties impl, pre or post
    //    It then is an error when
    //    - candidateCm does not have all 3 properties impl, pre, post
    //    - impl is not a Function
    //    - _checkPresWellFormed(candidateCm.pre) fails
    //    - _checkPostsWellFormed(candidateCm.post) fails

    var result = candidateCm && (! _isFunctionOrArray(candidateCm)) &&
                   (candidateCm.hasOwnProperty(_contractMethodImplName) ||
                     candidateCm.hasOwnProperty(_contractMethodPreName) ||
                     candidateCm.hasOwnProperty(_contractMethodPostName) ||
                     candidateCm.hasOwnProperty(_contractMethodExcpName));

    if (result) {
      if (! candidateCm.hasOwnProperty(_contractMethodPreName)) {
        throw new SyntaxError(_errorMsgContractMethod(propName) + _contractMethodPreName + " not defined");
      }
      if (! candidateCm.hasOwnProperty(_contractMethodImplName)) {
        throw new SyntaxError(_errorMsgContractMethod(propName) + _contractMethodImplName + " not defined");
      }
      if (! candidateCm.hasOwnProperty(_contractMethodPostName)) {
        throw new SyntaxError(_errorMsgContractMethod(propName) + _contractMethodPostName+ " not defined");
      }
      if (! candidateCm.hasOwnProperty(_contractMethodExcpName)) {
        throw new SyntaxError(_errorMsgContractMethod(propName) + _contractMethodExcpName+ " not defined");
      }
      if (! _isArray(candidateCm[_contractMethodPreName])) {
        throw new SyntaxError(_errorMsgContractMethod(propName) + _contractMethodPreName+ " not an Array");
      }
      if (! _isFunction(candidateCm[_contractMethodImplName])) {
        throw new SyntaxError(_errorMsgContractMethod(propName) + _contractMethodImplName+ " not a Function");
      }
      if (! _isArray(candidateCm[_contractMethodPostName])) {
        throw new SyntaxError(_errorMsgContractMethod(propName) + _contractMethodPostName+ " not an Array");
      }
      if (! _isArray(candidateCm[_contractMethodExcpName])) {
        throw new SyntaxError(_errorMsgContractMethod(propName) + _contractMethodExcpName+ " not an Array");
      }

      _checkPresWellFormed(candidateCm[_contractMethodPreName]);
      _checkPostsWellFormed(candidateCm[_contractMethodPostName]);
    }

    return result;
  }

  function methodContractOf(/*Object*/ o, /*Function*/ f) {
    var result = null;
    var contract = o.constructor._meta.contract;
    if (contract) {
      var methodNames = Object.getOwnPropertyNames(contract);
      var methodContractSingleton = methodNames.filter(function(cm) {
        return contract[cm][_contractMethodImplName] === f;
      });
      if (methodContractSingleton.length === 1) {
        result = contract[methodContractSingleton[0]];
      }
    }
    if (!result) {
      throw "No contract found in " + o + " for " + f;
    }
    return result;
  }

  function argumentsToArray(argsThing) {
    var result = [];
    var i;
    for (i = 0; i < argsThing.length; i++) {
      result.push(argsThing[i]);
    }
    return result;
  }

  function checkConditions(conditionTypeText, conditions, subject, args, extraArg) {
    var extendedArgs = args.slice(0);
    extendedArgs.push(extraArg);
    var errors = [];
    conditions.forEach(function(condition) {
      var conditionResult;
      try {
        conditionResult = condition.apply(subject, extendedArgs);
        if (!conditionResult ) {
          errors.push(condition);
        }
      }
      catch (e) {
        errors.push({
          condition : condition,
          exc       : e
        });
      }
    });
    if (errors.length > 0) {
      var error = {
        msg    : conditionTypeText + "s failed",
        errors : errors,
        toString : function() {
          return this.msg + " [" + this.errors + "]";
        }
      };
      throw error;
    }
  }

  function _functionWithPre(method) {
    var theThis = this;

    return function() {
      var args = argumentsToArray(arguments);

      checkConditions("Preconditions", methodContractOf(theThis, method)[_contractMethodPreName], theThis, args, null);
      return method.apply(theThis, args);
    };
  }

  function _functionWithPrePost(method) {
    var theThis = this;

    return function() {
      var args = argumentsToArray(arguments);

      checkConditions("Preconditions", methodContractOf(theThis, method)[_contractMethodPreName], theThis, args, null);
      var result;
      var nominalEnd = false;
      try {
        result = method.apply(theThis, args);
        nominalEnd = true;
      }
      catch (methodExc) {
        // this is exceptional (non-nominal), yet defined behavior
        checkConditions("Exception condition", methodContractOf(theThis, method)[_contractMethodExcpName], theThis, args, methodExc);
      }
      if (nominalEnd) {
        checkConditions("Postcondition", methodContractOf(theThis, method)[_contractMethodPostName], theThis, args, result);
      }
      return result;
    };
  }

  function _functionWithPrePostInvar(method) {
    var theThis = this;

    var contract = theThis.constructor._meta.contract;
    if (!contract) {
      throw "No contract found in " + theThis;
    }

    return function() {
      var args = argumentsToArray(arguments);

      checkConditions("Preconditions", methodContractOf(theThis, method)[_contractMethodPreName], theThis, args, null);
      var result;
      var nominalEnd = false;
      try {
        result = method.apply(theThis, args);
        nominalEnd = true;
      }
      catch (methodExc) {
        // this is exceptional (non-nominal), yet defined behavior
        checkConditions("Invariant", contract[_invariantPropertyName], theThis, [], null);
        checkConditions("Exception condition", methodContractOf(theThis, method)[_contractMethodExcpName], theThis, args, methodExc);
      }
      if (nominalEnd) {
        checkConditions("Invariant", contract[_invariantPropertyName], theThis, [], null);
        checkConditions("Postcondition", methodContractOf(theThis, method)[_contractMethodPostName], theThis, args, result);
      }
      return result;
    };
  }

  function ppwcodeDeclare(className, superclass, props) {
    var trueArgs = _crackParameters(className, superclass, props);
    /* we normalize props, so that we are sure that
     * - a property "invariants" contains sensible invariants
     * - if the value of a property with name pn of props (props[pn]) is an object cm that
     *   has (duck) type ContractMethod,
     *   -- the actual function (cm.impl) is a function, and associated to the property name
     *      (in stead of cm)
     *   -- the preconditions (cm.pre) are sensible preconditions, and associated to the
     *      pre property of the method in props (props[pn].pre)
     *   -- the postconditions (cm.post) are sensible postconditions, and associated to the
     *      post property of the method in props (props[pn].post)
     */
    var trueProps = trueArgs.props;
    var classHasContracts = trueProps.hasOwnProperty(_invariantPropertyName);
    if (classHasContracts) {
      var contract = {};
      var propNames = Object.getOwnPropertyNames(trueProps);
      propNames.forEach(function(propName) {
        var propValue = trueProps[propName];
        if (propName === _invariantPropertyName) {
          _checkInvariantsWellFormed(propValue);
          contract[_invariantPropertyName] = propValue;
          delete trueProps[_invariantPropertyName];
        }
        else if (_isContractMethod(propValue, propName)) {
          // remember the contract, make the methode the actual method in trueProps
          // this also works for the constructor
          var actualMethod = propValue[_contractMethodImplName];
          contract[propName] = {};
          contract[propName][_contractMethodPreName] = propValue[_contractMethodPreName];
          contract[propName][_contractMethodPostName] = propValue[_contractMethodPostName];
          contract[propName][_contractMethodExcpName] = propValue[_contractMethodExcpName];
          contract[propName][_contractMethodImplName] = actualMethod;
          trueProps[propName] = actualMethod;
        }
      });
      // and we add the contract-verification methods
      trueProps[_callWithContractsMethodName_Pre] = _functionWithPre;
      trueProps[_callWithContractsMethodName_PrePost] = _functionWithPrePost;
      trueProps[_callWithContractsMethodName_PrePostInvar] = _functionWithPrePostInvar;
    }
    var result = dojoDeclare(trueArgs.className, trueArgs.superclass, trueProps);
    if (classHasContracts) {
      // finally, add the contract to _meta
      result._meta[_contractInMetaName] = contract;
    }

    return result;
  }

  return ppwcodeDeclare;

});
