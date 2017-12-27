using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public class Counter : LogicObject {

    [SerializeField]
    private int targetValue = 1;

    public int currentValue;

    protected override string IconTextireName { get { return "Counter_Icon"; } }

    public UnityAction OnTargetReached;

    public void Increment() {
        currentValue++;

        if (currentValue == targetValue)
            OnTargetReached.Invoke();
    }

    public void Decrement() {
        currentValue--;

        if (currentValue == targetValue)
            OnTargetReached.Invoke();
    }
}
