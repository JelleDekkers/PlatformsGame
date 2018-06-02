using System.Collections;
using System.Collections.Generic;
using Serialization;
using UnityEngine;
using UnityEngine.Events;

public class TriggerTile : Tile {

    [SerializeField]
    private bool isRepeatable;
    public bool IsRepeatable { get { return isRepeatable; } }

    public UnityEvent OnEnterEvent;

    public UnityEvent[] testEvents;
    public float delayBetween = 1;

    public override void Enter(Block block) {
        base.Enter(block);
        if (OnEnterEvent != null)
            OnEnterEvent.Invoke();
    }

    #region Serialization
    public override DataContainer Serialize() {
        return new TriggerTileData(this);
    }

    public override object Deserialize(DataContainer data) {
        TileData baseData = base.Deserialize(data) as TileData;
        TriggerTileData parsedData = baseData as TriggerTileData;
        parsedData.onEnterEventData.Deserialize(ref OnEnterEvent);
        return parsedData;
    }
    #endregion
}
