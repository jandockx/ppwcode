﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=2.0.50727.1432.
// 


/// <remarks/>
//[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
//[System.SerializableAttribute()]
//[System.Diagnostics.DebuggerStepThroughAttribute()]
//[System.ComponentModel.DesignerCategoryAttribute("code")]
//[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public class messages {
    
    private messagesMessage[] itemsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("message", Form=System.Xml.Schema.XmlSchemaForm.None, IsNullable=true)]
    public messagesMessage[] Items {
        get {
            return this.itemsField;
        }
        set {
            this.itemsField = value;
        }
    }
}

/// <remarks/>
//[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
//[System.SerializableAttribute()]
//[System.Diagnostics.DebuggerStepThroughAttribute()]
//[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public  class messagesMessage {
    
    private int lcidField;


    private string lcidText;

    [System.Xml.Serialization.XmlIgnore()]
    public string LcidText
    {
        get { return lcidText; }
        set { lcidText = value; }
    }
    
    private string valueField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public int lcid {
        get {
            return this.lcidField;
        }
        set {
            this.lcidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public string Value {
        get {
            return this.valueField;
        }
        set {
            this.valueField = value;
        }
    }

    public override int GetHashCode()
    {
        return lcid.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        if (!obj.GetType().Equals(typeof(messagesMessage))) return false;
        messagesMessage msg = (messagesMessage)obj;

        return lcid.Equals(msg.lcid);
    }
}