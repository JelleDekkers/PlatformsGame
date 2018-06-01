using System.Collections;
using System.Collections.Generic;
using Serialization;
using UnityEngine;
using UnityEngine.Events;

public class PressureTile : Tile, ISerializableGameObject {

    [SerializeField]
    private bool isRepeatable;
    public bool IsRepeatable { get { return isRepeatable; } }

    public UnityEvent OnEnterEvent;
    public UnityEvent OnExitEvent;

    public override void DeserializeEvents(TileData tileData) {
        PressureTileData data = tileData as PressureTileData;
        UnityEventData.DeserializeEvents(OnEnterEvent, data.onEnterEventData);
        UnityEventData.DeserializeEvents(OnExitEvent, data.onExitEventData);
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

    #region Serialization
    public override DataContainer Serialize() {
        return new PressureTileData(this);
    }

    public override object Deserialize(DataContainer data) {
        TileData baseData = base.Deserialize(data) as TileData;
        PressureTileData parsedData = baseData as PressureTileData;
        // TODO: events
        return parsedData;
    }
    #endregion
}
