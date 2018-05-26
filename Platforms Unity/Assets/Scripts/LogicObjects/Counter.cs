using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public class Counter : LogicObject {

    [SerializeField]
    private int targetValue = 1;

    public int currentValue;
    public UnityEvent OnTargetReachedEvent;
    public UnityEvent OnValueChangedAfterTargetReachedEvent;

    [MenuItem("GameObject/" + MENU_PATH + "Counter", false, 0)]
    [MenuItem(MENU_PATH + "Counter", false, 0)]
    public static void CreateCounterObject() {
        GameObject g = new GameObject();
        g.name = "Counter";
        g.AddComponent<Counter>();
    }

    public void Increment() {
        OnValueChanged(currentValue + 1);
        currentValue++;
    }

    public void Decrement() {
        OnValueChanged(currentValue - 1);
        currentValue--;
    }

    private void OnValueChanged(int newValue) {
        if(newValue == targetValue && OnTargetReachedEvent != null)
            OnTargetReachedEvent.Invoke();
        else if(currentValue == targetValue && OnValueChangedAfterTargetReachedEvent != null) 
            OnValueChangedAfterTargetReachedEvent.Invoke();
    }
}
