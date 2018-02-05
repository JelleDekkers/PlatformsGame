using System.Collections;
using System.Collections.Generic;
using Serializing;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Reflection;

public class PressureTile : Tile {

    [SerializeField]
    private bool isRepeatable;
    public bool IsRepeatable { get { return isRepeatable; } }

    public UnityEvent OnEnterEvent;
    public UnityEvent OnExitEvent;

    public override void OnDeserializeEvents(TileData tileData) {
        PressureTileData data = tileData as PressureTileData;
        DeserializeEventListeners(OnEnterEvent, data.onEnterEventData);
        DeserializeEventListeners(OnExitEvent, data.onExitEventData);
    }

    private void DeserializeEventListeners(UnityEvent e, UnityEventData[] data) {
        for (int i = 0; i < data.Length; i++) {
            MethodInfo method = null;
            UnityAction action = null;

            if (!UnityEventData.DataLinks.ContainsKey(data[i].typeName)) {
                Debug.LogWarning(data[i].typeName + " is not found in UnityEventData.DataLinks");
                return;
            }

            UnityEngine.Object target = UnityEventData.GetTarget(data[i]);
            method = target.GetType().GetMethod(data[i].methodName);
            action = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), target, method);

#if UNITY_EDITOR
            UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(e, action);
#else
            OnEnterEvent.AddListener(action);
#endif
        }
    }

    public void EventTest() { }

    public override void Enter(Block block) {
        base.Enter(block);
        if (OnEnterEvent != null)
            OnEnterEvent.Invoke();
    }

    public override void Exit(Block block) {
        base.Exit(block);
        if (OnExitEvent != null)
            OnExitEvent.Invoke();
    }
}
