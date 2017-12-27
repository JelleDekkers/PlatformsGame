using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressureTile : Tile {

    [SerializeField]
    private bool isRepeatable;
    public bool IsRepeatable { get { return isRepeatable; } }

    public UnityEvent OnEnterUnityEvent;
    public UnityEvent OnExitUnityEvent;

    // onenter/onexit play click sound? via event

    public UnityEvent[] testEvents;
    public float delayBetween = 1;

    public override void Enter(Block block) {
        base.Enter(block);
        if (OnEnterUnityEvent != null)
            OnEnterUnityEvent.Invoke();
    }

    public override void Exit(Block block) {
        base.Exit(block);
        if (OnExitUnityEvent != null)
            OnExitUnityEvent.Invoke();
    }
}
