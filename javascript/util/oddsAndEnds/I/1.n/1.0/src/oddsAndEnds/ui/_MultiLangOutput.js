define(["dojo/_base/declare",  "./_MultiLangWidget"],
  function(declare, _MultiLangWidget) {

    return declare([_MultiLangWidget], {
      // summary:
      //   This widget shows (not-editable) a `value` in an i18n-ed way,
      //   and for which the representation language can change.
      //   `value` is what is shown. `lang` is the locale, which is set initially and kept in sync
      //   with the closest enclosing `_MultiLangAnchorParent`.
      //   Setting anything re-renders.
      // description:
      //   Missing values are rendered as the `missingLabel`, found by `getLabel`,
      //   if `missingLabel` is filled out. Otherwise `missing` is used.
      //
      //   All locales must be defined as extraLocale in dojoConfig.

      // missing: string
      //   This string is used when there is no value to show, if there is no `missinglabel`.
      missing: "?value?",

      // missingLabel: String
      //   The label of the missing message, defined in an nls bundle.
      //   `getLabel` must be able to find the label.
      missingLabel: null,

      set: function(name, value){
        // summary:
        //		Override and refresh output on value change.
        // name:
        //		The property to set.
        // value:
        //		The value to set in the property.
        this.inherited(arguments);
        if (this._created) {
          this._output();
        }
      },

      _getMissingAttr: function() {
        return this.missingLabel ?
          this.getLabel(this.missingLabel) :
          (this.missing || this.missing === "") ? this.missing : 'N/A';
      },

      _output: function(){
        // summary:
        //		Produce the value-bound output.
        // tags:
        //		protected

        this._c_ABSTRACT();
      }

    });
  }
);
