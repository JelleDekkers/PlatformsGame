using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

[Serializable]
[XmlInclude(typeof(TestData))]
[XmlInclude(typeof(TestDataChild))]
public abstract class DataContainer {

    [XmlAttribute] public string objectTypeName; 
    [XmlAttribute] public int guid;

    protected DataContainer() { }
    public DataContainer(UnityEngine.Object obj) {
        objectTypeName = obj.GetType().FullName;

        ISerializable serializableObj = obj as ISerializable;
        if (serializableObj.Guid != null)
            guid = serializableObj.Guid.ID;
        else
            guid = obj.GetInstanceID();
    }
}

public class TestData : DataContainer {

    [XmlElement] public int x;
    [XmlElement] public int z;
    [XmlElement] public bool moveUpAtStart;

    protected TestData() { }
    public TestData(TestObject obj) : base(obj) {
        x = obj.coordinates.x;
        z = obj.coordinates.z;
        moveUpAtStart = obj.moveUpAtStart;
    }
}

public class TestDataChild : TestData {

    [XmlElement] public int extraVar;

    protected TestDataChild() { }
    public TestDataChild(TestObjectChild obj) : base(obj) {
        extraVar = obj.extraVar;
    }
}
