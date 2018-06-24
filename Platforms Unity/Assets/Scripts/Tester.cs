using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour {

    private void Start() {
        List<ITest<TestDataClassBase>> testList = new List<ITest<TestDataClassBase>>();
        for (int i = 0; i < transform.childCount; i++) {
            ITest<TestDataClassBase> serializable = transform.GetChild(i).gameObject.GetInterface<ITest<TestDataClassBase>>();
            if (serializable != null)
                testList.Add(serializable);
        }

        Debug.Log(testList.Count);
    }
}
