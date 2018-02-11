using System.Collections;
using System.Collections.Generic;
using Serializing;
using UnityEngine;
using UnityEngine.Events;

public class TriggerTile : Tile {

    [SerializeField]
    private bool isRepeatable;
    public bool IsRepeatable { get { return isRepeatable; } }

    public UnityEvent OnEnterEvent;

    public UnityEvent[] testEvents;
    public float delayBetween = 1;

    public override void OnDeserializeEvents(TileData tileData) {
        PressureTileData data = tileData as PressureTileData;
        UnityEventData.DeserializeEvents(OnEnterEvent, data.onEnterEventData);
    }

    public override void Enter(Block block) {
        base.Enter(block);
        if (OnEnterEvent != null)
            OnEnterEvent.Invoke();
    }
}
