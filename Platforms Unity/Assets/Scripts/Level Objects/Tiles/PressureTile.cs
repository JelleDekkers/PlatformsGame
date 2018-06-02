using System.Collections;
using System.Collections.Generic;
using Serialization;
using UnityEngine;
using UnityEngine.Events;

public class PressureTile : Tile {

    public UnityEvent OnEnterEvent;
    public UnityEvent OnExitEvent;

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

    #region Serialization
    public override DataContainer Serialize() {
        return new PressureTileData(this);
    }

    public override object Deserialize(DataContainer data) {
        TileData baseData = base.Deserialize(data) as TileData;
        PressureTileData parsedData = baseData as PressureTileData;
        parsedData.onEnterEventData.Deserialize(ref OnEnterEvent);
        parsedData.onExitEventData.Deserialize(ref OnExitEvent);
        return parsedData;
    }
    #endregion
}
