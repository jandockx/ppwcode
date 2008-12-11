dojo.provide("org.ppwcode.dojo.dijit.form.PpwCrudForm");

dojo.require("dijit.form.Form");
dojo.require("dijit.form.Button");
dojo.require("dijit.Tooltip");
dojo.require("dojo.i18n");
dojo.requireLocalization("org.ppwcode.dojo.dijit.form", "PpwCrudForm");


dojo.declare(
	"org.ppwcode.dojo.dijit.form.PpwCrudForm",
	[dijit.form.Form],
	{
		// summary:
		//   The PpwCrudForm builds on dijit.form.Form to add Crud functionality.  You create
		//   your forms as dojo enriched HTML forms (a <form dojoType="org.ppwcode.dojo.dijit.form.PpwCrudForm"></form>
		//   with dijit form widgets, such as dijit.form.TextBox in it). The form offers
		//   the same functionality as the dijit.form.Form, adds CRUD viewing modes,
		//   allows to automatically display javascript objects in the form and provides
		//   an easy way to update and create new javascript objects.
        // description:
		//   1) Viewing modes
		//
		//   The form supports 3 viewing modes, View mode, Create mode and Update mode.  Each viewing mode
		//   changes the status of the input fields in the form and displays a number of buttons that apply
		//   for that viewing mode.  In View mode, all fields are disabled (i.e. not editable), and two buttons
		//   are shown: an edit button and an optional delete button (the delete button is enabled by setting
		//   the withDelete attribute of the PpwCrudForm to true).  Clicking the Edit button will put the form
		//   in Update mode.
		//
		//   In Update mode, all form input fields are enabled (editable) and another set of
		//   Buttons are placed under the form:  An Update button and a Cancel button.  Clicking the Update
		//   button will make the buttons disappear and generate the onUpdateModeSaveButtonClick() event,
		//   which can be overridden by the form user, the buttons disappear.  The Cancel button will undo
		//   all changes made and put the form back in View mode.
		//
		//   Create mode will be reset the form fields (the form will be empty) and display a Create button and
		//   a Cancel button.  Clicking the Cancel button does not return to View Mode; it will reset the form
		//   and remove the buttons.  Clicking the create button will make the buttons disappear and generate the
		//   onCreateModeSaveButtonClick() event.
		//
		//
		//   2) Display a javascript object in the form.
		//
		//   Although the view modes can be manipulated explicitly, the main goal of the form is to provide improved
		//   support for Creating, Updating and Deleting of arbitrary javascript Objects.  The core functionality
		//   lies in the event handlers for save and update operations and the ability to display the values of a
		//   particular javascript object in a straightforward way.
		//
		//   The main business operations that a javascript program can call on the PpwCrudForm are createObject() and
		//   displayObject(object).  Calling createObject() puts the form in Create mode.  Calling displayObject will
		//   display the object passed along as a parameter and put the form in View mode.  To be able to display
		//   an object, the PpwCrudForm must know how to map the properties of an object to the fields in the form.
		//   This is done by setting the fromMap (setFormMap(map)) of the PpwCrudForm.  a formMap is array of
		//   mapping objects:
		//
		//   var formmap = [{property: "prop1", fieldId: "textfield1", isId: true},
		//                  {property: "prop2", fieldId: "hiddenfield1},
		//                   ...
		//                  {property: "propN", feieldId: "datefieldN"}];
		//
		//   Each mapping object must contain 2 properties:  a "property" and a "fieldid".  The "property" must be
		//   the name of a property of the object you will be displaying, the fieldid must be the HTML ID of an
		//   input field in the form.  You're obviously not required to display all properties of an object, only
		//   those in the formmap will be displayed.  There is one additional property of the mapping object: "isId".
		//   this marks a property of the object as a property that can be used to identify the property.  the value
		//   of the isId property must always be a boolean.  The form does not use the isId property, but can make
		//   identity information available to create or update handlers that need that information;
		//   the getObjectIdFields() method returns an array of all the property names that were marked as "isId".
		//
		//
		//   3) Creating and updating objects
		//
		//   the onUpdateModeSaveButtonClick and onSaveModeSaveButtonClick event handlers are called when
		//   the Update or Create buttons are clicked.  Both event handlers pass along an event object as
		//   a parameter.  This event object corresponds to the click event that was generated by the browser
		//   when the button was clicked, with one additional property: "formObject".  This property contains
		//   the object that was created or updated in the form.  In case of creation, the object will be created
		//   by calling the function that was passed in the "constructorFunction" attribute of the PpwCrudForm.
		//   (if it's not specified, the object will be created by issuing "new Object()").  Subsequently, all the
		//   values of the input fields will be assigned to the newly created object, adhering to the formMap.
		//   In case of updating, the original object is cloned (using dojo.clone, so no loops in the object
		//   graph allowed), and the form fields are assigned to the appropriate object properties.

		templatePath: dojo.moduleUrl("org", "ppwcode/dojo/dijit/form/templates/PpwCrudForm.html"),
		//override templateString from superclass
		templateString: null,

		constructorFunction: null,

		withDelete: false,

		_viewModeButtonPanel: null,
		_viewModeEditButton: null,
		_viewModeDeleteButton: null,

		_createModeButtonPanel: null,
		_createModeSaveButton: null,
		_createModeCancelButton: null,

		_createFunction: null,

		_updateModeButtonPanel: null,
		_updateModeSaveButton: null,
		_updateModeCancelButton: null,

		_updateFunction: null,

		// contains the mapping between javascript object properties and form fields
		_formmap: null,
		_propertyToDomNodeMap: null,
		_propertyToWidgetMap: null,

		_tooltips: null,

		//the masterview, if there is one.
		_masterview: null,

		_thedisplayobject: null,
		_thebuttoncontainer: null,

		objectName: "",
		
		constructor: function() {
 			this._tooltips = new Object();

			var localizationbundle = dojo.i18n.getLocalization("org.ppwcode.dojo.dijit.form", "PpwCrudForm");
			//View Button Panel
			this._viewModeButtonPanel = dojo.doc.createElement('div');
			dojo.addClass(this._viewModeButtonPanel, "PpwCrudFormButtonPanel");
			//Edit Button
			var tempButton = dojo.doc.createElement('button');
			this._viewModeButtonPanel.appendChild(tempButton);
			this._viewModeEditButton =
				new dijit.form.Button({
					label: localizationbundle.editButtonLabel
				}, tempButton);
			this._viewModeEditButton.startup();

			//Create Button Panel
			this._createModeButtonPanel = dojo.doc.createElement('div');
			dojo.addClass(this._createModeButtonPanel, "PpwCrudFormButtonPanel");
			//Create Button
			tempButton = dojo.doc.createElement('button');
			this._createModeButtonPanel.appendChild(tempButton);
			this._createModeSaveButton =
				new dijit.form.Button({
					label: localizationbundle.createButtonLabel
				}, tempButton);
			this._createModeSaveButton.startup();
			//Cancel button
			tempButton = dojo.doc.createElement('button');
			this._createModeButtonPanel.appendChild(tempButton);
			this._createModeCancelButton =
				new dijit.form.Button({
					label: localizationbundle.cancelButtonLabel
				}, tempButton);
			this._createModeSaveButton.startup();

			//Update Button Panel
			this._updateModeButtonPanel = dojo.doc.createElement('div');
			dojo.addClass(this._updateModeButtonPanel, "PpwCrudFormButtonPanel");
			//Create Button
			tempButton = dojo.doc.createElement('button');
			this._updateModeButtonPanel.appendChild(tempButton);
			this._updateModeSaveButton =
				new dijit.form.Button({
					label: localizationbundle.updateButtonLabel
				}, tempButton);
			this._createModeSaveButton.startup();
			//this._createModeButtonPanel.appendChild(spacer);
			//Cancel button
			tempButton = dojo.doc.createElement('button');
			this._updateModeButtonPanel.appendChild(tempButton);
			this._updateModeCancelButton =
				new dijit.form.Button({
					label: localizationbundle.cancelButtonLabel
				}, tempButton);
			this._updateModeSaveButton.startup();
		},

		postMixInProperties: function(){
			//connect buttons
			this.inherited(arguments);
			dojo.connect(this._viewModeEditButton, "onClick", this, "_onviewmodeeditbuttonclick");
			dojo.connect(this._updateModeSaveButton, "onClick", this, "_onupdatemodesavebuttonclick");
			dojo.connect(this._updateModeCancelButton, "onClick", this, "_onupdatemodecancelbuttonclick");
			dojo.connect(this._createModeSaveButton, "onClick", this, "_oncreatemodesavebuttonclick");
			dojo.connect(this._createModeCancelButton, "onClick", this, "_oncreatemodecancelbuttonclick");

			var localizationbundle = dojo.i18n.getLocalization("org.ppwcode.dojo.dijit.form", "PpwCrudForm");

			if (this.withDelete === true) {
				var tempButton = dojo.doc.createElement('button');
				this._viewModeButtonPanel.appendChild(tempButton);
				this._viewModeDeleteButton =
					new dijit.form.Button({
						label: localizationbundle.deleteButtonLabel
					}, tempButton);
				this._viewModeDeleteButton.startup();
				dojo.connect(this._viewModeDeleteButton, "onClick", this, "_onviewmodedeletebuttonclick");
			}
		},

		onSubmit: function(/*Event*/e){
			//this form should normally never be submitted
			console.log("PpwCrudForm.onSubmit called");
			return false;
		},

		setButtonContainerNode: function(/*String*/btncontainername) {
			this._thebuttoncontainer = dojo.byId(btncontainername);
		},

		_displayButtons: function(panel) {
			if (this._thebuttoncontainer) {
				this._thebuttoncontainer.appendChild(panel);
			} else {
				this.buttonContainerNode.appendChild(panel);
			}
		},

		setFormMap: function(/*Array*/formmap) {
			// summary:
			//   Set a new formmap on the form.  The formmap links input fields
			//   in the form to properties/attributes in the javaobjects that
			//   will be displayed in the form, and resets the form.
			// description.
			//   To be able to display an object, the PpwCrudForm must know how to
			//   map the properties of an object to the fields in the form. This is
			//   done by setting the fromMap (setFormMap(map)) of the PpwCrudForm.
			//   A formMap is array of mapping objects:
			//
			//   var formmap = [{property: "prop1", fieldId: "textfield1", isId: true},
			//                  {property: "prop2", fieldId: "hiddenfield1},
			//                   ...
			//                  {property: "propN", feieldId: "datefieldN"}];
			// formmap: an array of mapping objects.
			this._formmap = formmap;
			this._propertyToDomNodeMap = new Object();
			this._propertyToWidgetMap = new Object();
			for (var i = 0; i < formmap.length; i++) {
				this._propertyToDomNodeMap[formmap[i].fieldid] = dojo.byId(formmap[i].fieldid);
				this._propertyToWidgetMap[formmap[i].fieldid] = dijit.byId(formmap[i].fieldid);
			}
			this.reset();
		},
		/////////////////////////// GUI handling ///////////////////////


		//remove button panel
		_setInitMode: function() {
			if (this._thebuttoncontainer) {
			   dojo.query(".PpwCrudFormButtonPanel", this._thebuttoncontainer).orphan();
			} else {
			   dojo.query(".PpwCrudFormButtonPanel", this.buttonContainerNode).orphan();
			}
			this._disableFormFields(true);
  		},

		_disableFormFields: function(/*boolean*/disabled){
		   for (var i = 0; i < this._formmap.length; i++) {
			   this._propertyToWidgetMap[this._formmap[i].fieldid].setAttribute("disabled", disabled);
		   }
		},

		setViewMode: function() {
			// summary:
			//   Set the form in view mode
			// description:
			//   In view mode, the buttons shown are "Edit" and optionally "Delete".  The fields
			//   are disabled.  Note that the contents of the input fields is not modified.
			this._setInitMode();
			this._displayButtons(this._viewModeButtonPanel);
			this._disableFormFields(true);
		},

		setUpdateMode: function() {
			// summary:
			//   Set the form in update mode.
			// description:
			//   In view mode, the buttons shown are "Update" and "Cancel".  The fields
			//   are disabled.  The contents of the input fields is not modified.
			this._setInitMode();
			this._displayButtons(this._updateModeButtonPanel);
			this._disableFormFields(false);
		},

		setCreateMode: function() {
			// summary:
			//   Set the form in create mode.
			// description:
			//   In view mode, the buttons shown are "Create" and "Cancel".  The fields
			//   are disabled.  The content of the input fields are cleared
			this.reset();
			this._displayButtons(this._createModeButtonPanel);
			this._disableFormFields(false);
		},


		//////////////////////// Button Events /////////////////////////

		_onviewmodeeditbuttonclick: function(/*Event*/e) {
			this.setUpdateMode();
            this.onViewModeEditButtonClick(e);
		},

		_onviewmodedeletebuttonclick: function(/*Event*/e) {
			this._setInitMode();
			e.formObject = dojo.clone(this._thedisplayobject);
			this.onViewModeDeleteButtonClick(e);
		},

		_onupdatemodesavebuttonclick: function(/*Event*/e) {
			this._setInitMode();
			var obj = dojo.clone(this._thedisplayobject);
			e.formObject = this._createOrUpdateObjectFromForm(obj);
			this.onUpdateModeSaveButtonClick(e);
		},

		_onupdatemodecancelbuttonclick: function(/*Event*/e) {
			//reset fields.  createObject does not set
			this.displayObject(this._thedisplayobject);
			this.setViewMode();
            this.onUpdateModeCancelButtonClick(e);
		},

		_oncreatemodesavebuttonclick: function(/*Event*/e) {
			if (this.validate()) {
			  this._setInitMode();
			  //decorate the event with the new object
			  e.formObject = this._createOrUpdateObjectFromForm(new this.constructorFunction());
			  this.onCreateModeSaveButtonClick(e);
			} else {
              dojo.stopEvent(e);
            }
		},

		_oncreatemodecancelbuttonclick: function(/*Event*/e) {
			this.reset();
			this.onCreateModeCancelButtonClick(e)
		},

		_createOrUpdateObjectFromForm: function(/*Object*/obj) {
			for (var i = 0; i < this._formmap.length; i++) {
				//TODO eval("obj." + this._formmap[i].property)
				//obj[this._formmap[i].property] = dijit.byId(this._formmap[i].fieldid).getValue();
				eval("obj." + this._formmap[i].property + " = dijit.byId(this._formmap[i].fieldid).getValue();");
			}
			return obj;
		},

		// resize will only be called when it is an immediate child of a
		// layout widget.  In case of a ContentPane, it must even be the
		// only child...
		resize: function() {
			//console.log("PpwCrudForm: resize()!!");
			for (var props in this._tooltips) {
				this._tooltips[props].hide(this._tooltips[props].__PpwNode);
				this._tooltips[props].show(this._tooltips[props].__PpwMessage, this._tooltips[props].__PpwNode);
			}
 		},

		///////////////////// Public Button Events /////////////////////////

		onUpdateModeSaveButtonClick: function(/*Event*/ e) {
 			// summary:
 			//    override function:  called when the Save button in Update mode is clicked.
 			// description:
 			//    override function:  called when the Save button in Update mode is clicked.
 			// e:
 			//    DOM event
		},

		onCreateModeSaveButtonClick: function(/*Event*/ e) {
 			// summary:
 			//    override function:  called when the Save button is clicked in Create mode.
 			// description:
 			//    override function:  called when the Save button is clicked in Create mode.
 			// e:
 			//    DOM event
		},

		onViewModeDeleteButtonClick: function(/*Event*/ e) {
 			// summary:
 			//    override function:  called when the Delete button is clicked in View mode.
 			// description:
 			//    override function:  called when the Delete button is clicked in View mode.
 			// e:
 			//    DOM event
		},

        onViewModeEditButtonClick: function(/*Event*/ e) {
            // summary:
 			//    override function:  called when the Edit button is clicked in View mode.
 			// description:
 			//    override function:  called when the Edit button is clicked in View mode.
 			// e:
 			//    DOM event
        },

        onUpdateModeCancelButtonClick: function(/*Event*/ e) {
            // summary:
 			//    override function:  called when the Cancel button is clicked in Update mode.
 			// description:
 			//    override function:  called when the Cancel button is clicked in Update mode.
 			// e:
 			//    DOM event
        },

        onCreateModeCancelButtonClick: function(/*Event*/ e) {
            // summary:
 			//    override function:  called when the Cancel button is clicked in Create mode.
 			// description:
 			//    override function:  called when the Cancel button is clicked in Create mode.
 			// e:
 			//    DOM event
        },
		// Business Methods...

		reset: function() {
			// summary:
			//    Resets the form.
			// description:
			//    Clear the input fields of the form
			this._setInitMode();
			this.inherited(arguments);
		},

		displayObject: function(obj) {
			// summary:
			//   display an object in the form.  This object will be layed out
			//   in the form according to the formmap.
			// description:
			//   This object iterates over all the properties in the object and
			//   locates a input field in the form that corresponds with the
			//   property.  This mapping between properties and input fields is
			//   defined in the formmap.
			// obj:
			//   The object that will be displayed in the form.
			this._thedisplayobject = obj;
			//copy fields in the object to the form.
			for (var i = 0; i < this._formmap.length; i++) {
				this._propertyToWidgetMap[this._formmap[i].fieldid].setValue(eval("obj." + this._formmap[i].property));
			}
			this.setViewMode();
		},

		createObject: function() {
			// summary:
			//    Prepare the form to create a new object.
			// description:
			//    This methods puts the form in createmode, and clears all
			//    references to objects that it may be displaying at the
			//    time this method is called
			this._thedisplayobject = null;
			this.setCreateMode();
		},

		getObjectName: function() {
			// summary:
			//    returns the name of the object that this form shows
			//    in a human readable form
			// description:
			//    this property can be set using the objectName attribute
			//    in the defining HTML tag.
			return this.objectName;
		},
		
		getConstructorFunction: function() {
			return this.constructorFunction;
		},
		
		getObjectIdFields: function() {
			// summary:
			//    Returns an array containing the names of the properties
			//    that establish the id of the objects that are displayed
			//    in the form.
			// description:
			//    Returns an array of strings:  each entry is a property
			//    name that was defined in the formmap and was tagged with
			//    isId: true.
			var idfields = new Array();
			for (var i = 0; i < this._formmap.length; i++) {
				if (this._formmap[i].isId === true) {
					idfields.push(this._formmap[i].property);
				}
			}
			return idfields;
		},

		_hideErrorMessage: function(/*String*/property) {
			var tip = this._tooltips[property];
			tip.hide(tip.__PpwNode);
			while (tip.__PpwConnectHandle.length > 0) {
				dojo.disconnect(tip.__PpwConnectHandle.pop());
			}
			delete this._tooltips[property];
		},

		displayErrorMessages: function(/*Object[]*/messages) {
			// summary:
			//    display error messages on the form using Dojo ToolTip widgets
			// description:
			//    display error messages on the form using Dojo ToolTip widgets.
			//    These messages are different from the typical input field
			//    validation messages in that they typically come from the server.
			//    This method provides a means to display error messages that
			//    result from server side form validation.
			// messages:
			//    an array of messages.  The array consists of objects that
			//    must have the following properties:  "property" and
			//    "message".  For example:
			//    [ {property: "name", message: "name is not unique"},
			//      {property: "e-mail", message: "the domain name for this email address does not exist."}]

			for (var property in this._tooltips) {
				// remove old tooltips if any
				this._tooltips[property].hide(this._tooltips[property].__PpwNode);
				delete this._tooltips[property];
			}

			for (var i = 0; i < messages.length; i++) {
				var tt = new dijit._MasterTooltip();
				var formwidget = this._propertyToWidgetMap[messages[i].property];
				//no fading
				tt.duration = 1;
				//needed to recreate the fade functions
				tt.postCreate();
				//let's just cache the node and the message in the tooltip, so
				//we can reference them when resizing.
				tt.__PpwNode = this._propertyToDomNodeMap[messages[i].property];
				tt.__PpwMessage = messages[i].message;

				//attach to some events on the form field to make the tip disappear
				tt.__PpwConnectHandle = new Array();
				//every _Widget has an onChange method
				tt.__PpwConnectHandle.push(
					dojo.connect(
						formwidget,
						"onChange",
						null,
						dojo.hitch(this, this._hideErrorMessage, messages[i].property)
					)
				);
				//FormValueWidgets have an _onKeyPress Method... private or not... we
				//attach to it, so we can hide the errormessage on first keypress.
				if (formwidget._onKeyPress && dojo.isFunction(formwidget._onKeyPress)) {
					tt.__PpwConnectHandle.push(
						dojo.connect(
							formwidget,
							"_onKeyPress",
							null,
							dojo.hitch(this, this._hideErrorMessage, messages[i].property)
						)
					);
				}
				this._tooltips[messages[i].property] = tt;
				//show the tooltip
				tt.show(tt.__PpwMessage, tt.__PpwNode);
			}
		}
	}
);