using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerTile : Tile {

    [SerializeField]
    private bool isRepeatable;
    public bool IsRepeatable { get { return isRepeatable; } }

    public UnityEvent OnEnterUnityEvent;

    // onenter/onexit play click sound? via event

    public UnityEvent[] testEvents;
    public float delayBetween = 1;

    public override void Enter(Block block) {
        base.Enter(block);
        if (OnEnterUnityEvent != null)
            OnEnterUnityEvent.Invoke();
    }
}
