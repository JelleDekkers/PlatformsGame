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

        // onEnter:
        for (int i = 0; i < data.onEnterEventData.Length; i++) {
            MethodInfo method = null;
            UnityAction action = null;

            if (data.onEnterEventData[i].typeName == typeof(Tile).FullName) {
                UnityEventData d = data.onEnterEventData[i];
                IntVector2 coordinates = new IntVector2(int.Parse(d.typeArgs[0]), int.Parse(d.typeArgs[1]));
                Tile target = LevelManager.CurrentLevel.Tiles.GetTile(coordinates);
                method = target.GetType().GetMethod(d.methodName);
                action = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), target, method);
#if UNITY_EDITOR
                UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(OnEnterEvent, action);
#else
                OnEnterEvent.AddListener(action);
#endif
            }
        }

        // onExit
        for (int i = 0; i < data.onExitEventData.Length; i++) {
            MethodInfo method = null;
            UnityAction action = null;

            if (data.onExitEventData[i].typeName == typeof(Tile).FullName) {
                UnityEventData d = data.onExitEventData[i];
                IntVector2 coordinates = new IntVector2(int.Parse(d.typeArgs[0]), int.Parse(d.typeArgs[1]));
                Tile target = LevelManager.CurrentLevel.Tiles.GetTile(coordinates);
                method = target.GetType().GetMethod(d.methodName);
                action = (UnityAction)Delegate.CreateDelegate(typeof(UnityAction), target, method);

#if UNITY_EDITOR
                UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(OnExitEvent, action);
#else
            OnExitEvent.AddListener(action);
#endif
            }
        }
    }

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
