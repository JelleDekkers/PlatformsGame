using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObject : MonoBehaviour, ISerializable {

    public IntVector2 coordinates;
    public bool moveUpAtStart = true;

    public MyGUID Guid {
        get; private set;
    }

    public virtual DataContainer Serialize() {
        return new TestData(this);
    }

    public virtual object Deserialize(DataContainer data) {
        TestData parsedData = data as TestData;
        Guid = new MyGUID(data.guid);
        coordinates = new IntVector2(parsedData.x, parsedData.z);
        moveUpAtStart = parsedData.moveUpAtStart;
        return parsedData;
    }
}
