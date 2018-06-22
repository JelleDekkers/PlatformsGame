using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestClass : MonoBehaviour, ITest<TestDataClassChild> {

    public TestDataClassChild GetTest() {
        return new TestDataClassChild();
    }
}


public class TestDataClassBase {

}

public class TestDataClassChild : TestDataClassBase {

}