define(["dojo/_base/declare", "ppwcode/contracts/_Mixin"],
    function(declare, _ContractMixin) {

      /* TODO This is a first version. There are more performant implementations of sets.
              We "assume" an "interface" here for Sets (see java.util.collections), which
              we have not yet made explicit in this version.
       */

      // see https://developer.mozilla.org/en-US/docs/JavaScript/Reference/Global_Objects/Array/prototype

      // no null allowed


//      var props = {
//        elementType: function() {
//          // Constructor
//        },
//        equivalence: function(/*Object*/ one, /*Object*/ other) {
//          return true; // return boolean
//        },
//        data: []
//      };

      function referenceEquivalence(/*Object*/ one, /*Object*/ other) {
        return one === other;
      }

      var ArraySet = declare("be.ppwcode.util.collections.ArraySet", [_ContractMixin], {
        // summary:
        //   An ArraySet is a Set, which is internally represented as an Array.

        constructor: function(/*props*/ props) {
          // TODO pre: if props.elementType exists, it must be a Constructor
          // TODO pre: if props.equivalence exists, it must be a function that behaves as expected
          // TODO pre: if props.data exists, it must be Collection, Object or an Array

          this._elementType = props && props.elementType ? props.elementType : Object;
          this._equivalence = props && props.equivalence ? props.equivalence : referenceEquivalence;
          // TODO pre: All data of elementType
          this._data = props && props.data ? props.data.slice() : [];
        },

        clone: function() {
          return new ArraySet({
            elementType: this._elementType,
            equivalence: this._equivalence,
            data: this._data
          })
        },

        getElementType: function() {
          return this._elementType;
        },

        isOfElementType: function(/*Object*/ element) {
          this._c_pre(function() {return element;});

          return element.isInstanceOf(this._elementType); // TODO not general enough; requires dojo
        },

        getSize: function() {
          return this._data.length;
        },

        isEmpty: function() {
          return this._data.length <= 0;
        },

        contains: function(/*Object*/ any) {
          var thisSet = this;
          return this._data.some(function(element) {
            return thisSet._equivalence(element, any);
          });
        },

        _iterate: function(/*Function*/ iterator, /*Function*/ callback, /*Object*/ thisArg) {
          this._c_pre(function() {return iterator;});
          // TODO pre iterator is an iterator function ...
          this._c_pre(function() {return callback;});
          // TODO pre callback is a function

          // callback is invoked with three arguments: the element value, the element index, the set being traversed
          var thisSet = this;
          var callbackContext =  thisArg ? thisArg : thisSet;
          var result = iterator.call(
            this._data,
            function(element, index, data) {
              callback.call(callbackContext, element, index, thisSet);
            }
          );
          return result;
        },

        forEach: function(/*Function*/ callback, /*Object*/ thisArg) {
          this._c_pre(function() {return callback;});
          // TODO pre callback is a function

          // callback is invoked with three arguments: the element value, the element index, the set being traversed
          this._iterate(this._data.forEach);
          return this;
        },

        every: function(/*Function*/ callback, /*Object*/ thisArg) {
          this._c_pre(function() {return callback;});
          // TODO pre callback is a function

          // callback is invoked with three arguments: the element value, the element index, the set being traversed
          return this._iterate(this._data.every);
        },

        some: function(/*Function*/ callback, /*Object*/ thisArg) {
          this._c_pre(function() {return callback;});
          // TODO pre callback is a function

          // callback is invoked with three arguments: the element value, the element index, the set being traversed
          return this._iterate(this._data.some);
        },

        filter: function(/*Function*/ callback, /*Object*/ thisArg) {
          this._c_pre(function() {return callback;});
          // TODO pre callback is a function

          // callback is invoked with three arguments: the element value, the element index, the set being traversed
          return this._iterate(this._data.filter);
        },

        map: function(/*Function*/ callback, /*Object*/ thisArg) {
          this._c_pre(function() {return callback;});
          // TODO pre callback is a function

          // callback is invoked with three arguments: the element value, the element index, the set being traversed
          return this._iterate(this._data.map);
        },

        reduce: function(/*Function*/ callback, /*Object*/ initialValue) {
          this._c_pre(function() {return callback;});
          // TODO pre callback is a function

          // callback:
          //   Function to execute on each value in the set, taking four arguments:
          //     previousValue: The value previously returned in the last invocation of the callback,
          //       or initialValue, if supplied. (See below.)
          //     currentValue: The current element being processed in the set.
          //     index: The index of the current element being processed in the set.
          //     set: The set reduce was called upon.

          var thisSet = this;
          var result = this._data.reduce(
            function(previousValue, currentValue, index, data) {
              callback.call(thisSet, previousValue, currentValue, index, thisSet);
            },
            initialValue
          );
          return result;

        },

        toArray: function() {
          return this._data.slice();
        },

        add: function(/*Object*/ element) {
          this._c_pre(function() {return element;});
          this._c_pre(function() {return this._isOfElementType(element);});

          if (! this.contains(element)) {
            this._data.push(element);
          }
        },

//        addAll: function(/*Collection*/ collection) {
//        },
//
//        addAll: function(/*Collection*/ collection, /*Function*/ filter) {
//        },

        remove: function(/*Object*/ element) {
          if (! element) {
            // this is not a precondition; we have fulfilled the postcondition
            return;
          }
          var elementIndex = -1;
          this._data.some(function(someElement, index) {
            if (this._equivalence(element, someElement)) {
              elementIndex = index;
              return true;
            }
            else {
              return false;
            }
          },
          this);
          if (elementIndex > -1) {
            this._data = this._data.splice(elementIndex, 1);
          }
        },

//        removeAll: function(/*Collection*/ collection) {
//        },
//
//        removeAll: function(/*Collection*/ collection, /*Function*/ filter) {
//        },

        clear: function() {
          this._data = [];
        },

        hasSameElements: function(/*Collection*/ collection) {
          this._c_pre(function() {return collection;});
          // TODO is a collection or an Array or an Object; we use every and some

          var allFromHimInMe = collection.every(
            function(cElement) {
              return this.contains(cElement);
            },
            this
          );
          if (! allFromHimInMe) {
            return false;
          }
          else {
            var allFromMeInHim = this._data.every(
              function(myElement) {
                return collection.some(
                  function(cElement) {
                    return this._equivalence(myElement, cElement);
                  },
                  this
                );
              },
              this
            );
            return allFromMeInHim;
          }
        },

        toString: function() {
          return this._data.toString();
        }

      });

      return ArraySet;
    }
);