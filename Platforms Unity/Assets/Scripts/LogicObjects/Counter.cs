using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Counter : LogicObject {

    public int targetValue = 1;
    public int currentValue;
    public UnityEvent OnTargetReachedEvent;
    public UnityEvent OnValueChangedAfterTargetReachedEvent;

#if UNITY_EDITOR
    [MenuItem("GameObject/" + MENU_PATH + "Counter", false, 0)]
    [MenuItem(MENU_PATH + "Counter", false, 0)]
    public static void CreateCounterObject() {
        Counter counter = Instantiate(PrefabManager.Counter);
        if(LevelManager.Instance != null)
            counter.transform.SetParent(LevelManager.Instance.transform);
    }
#endif

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

    #region Serialization
    public override DataContainer Serialize() {
        return new CounterData(this);
    }

    public override object Deserialize(DataContainer data) {
        LogicObjectData baseData = base.Deserialize(data) as LogicObjectData;
        CounterData parsedData = baseData as CounterData;
        targetValue = parsedData.targetValue;
        parsedData.OnTargetReachedEvent.Deserialize(ref OnTargetReachedEvent);
        parsedData.OnValueChangedAfterTargetReachedEvent.Deserialize(ref OnValueChangedAfterTargetReachedEvent);
        return parsedData;
    }
    #endregion
}
