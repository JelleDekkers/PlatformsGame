using Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObject : MonoBehaviour, ITestSerializable {

    public IntVector2 coordinates;
    public bool moveUpAtStart = true;

    public GUID Guid {
        get; private set;
    }

    public virtual TestDataContainer Serialize() {
        return new TestData(this) as TestDataContainer;
    }

    public virtual object Deserialize(TestDataContainer data) {
        TestData parsedData = data as TestData;
        Guid = new GUID(data.guid, this);
        coordinates = new IntVector2(parsedData.x, parsedData.z);
        moveUpAtStart = parsedData.moveUpAtStart;
        return parsedData;
    }
}
