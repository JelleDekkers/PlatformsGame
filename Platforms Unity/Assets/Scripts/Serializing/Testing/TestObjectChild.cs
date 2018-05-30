using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObjectChild : TestObject {

    public int extraVar;

    public override DataContainer Serialize() {
        return new TestDataChild(this);
    }

    public override object Deserialize(DataContainer data) {
        TestData baseData = base.Deserialize(data) as TestData;
        TestDataChild parsedData = baseData as TestDataChild;
        extraVar = parsedData.extraVar;
        return parsedData;
    }
}
