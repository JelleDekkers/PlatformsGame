using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable, SelectionBase]
public class Block : MonoBehaviour, ISerializableEventTarget {

    public static readonly Vector3 POSITION_OFFSET = new Vector3(0.5f, 0.5f, 0.5f);
    public static readonly Vector3 SIZE = new Vector3(1f, 1f, 1f);

    public Tile tileStandingOn;

    public IntVector2 Coordinates { get { return tileStandingOn.coordinates; } }

    private Renderer rend;

    public virtual void Deserialize(Serializing.BlockData data) { }

    protected virtual void Awake() {
        if(GeneralSettings.UseTransitions)
            GameEvents.OnLevelStart += IntroTransition;
        GameEvents.OnIntroComplete += OnIntroComplete;
    }

    protected virtual void IntroTransition() {
        Vector3 target = transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y + BlockSettings.IntroStartingHeight, transform.position.z); 
        float duration = BlockSettings.IntroStandardDuration + Random.Range(0, BlockSettings.IntroDurationMax);
        StartCoroutine(Tween.MoveBetween(transform, BlockSettings.IntroDelay, duration, transform.position, target));
    }

    public void SetTileStandingOn(Tile tile) {
        tileStandingOn = tile;
    }

    public void SetRenderQueue(int value) {
        if (rend == null)
            rend = GetComponent<Renderer>();
        Material[] materials = rend.materials;
        for (int i = 0; i < materials.Length; ++i) {
            materials[i].renderQueue = value;
        }
    }

    protected virtual void DestroySelfWithTimer() {
        Destroy(gameObject, BlockSettings.DestroyDelayOnFall);
    }

    protected void UnSubscribeToTileEvents(Tile t) {
        if (tileStandingOn == null)
            return;
        t.OnMoveDownEnd -= OnTileStandingOnMoveDownEnd;
        t.OnMoveDownStart -= OnTileStandingOnMoveDownStart;
        t.OnMoveUpEnd -= OnTileStandingOnMoveUpEnd;
        t.OnMoveUpStart -= OnTileStandingOnMoveUpStart;
    }

    protected void SubscribeToTileEvents(Tile t) {
        tileStandingOn.OnMoveDownEnd += OnTileStandingOnMoveDownEnd;
        tileStandingOn.OnMoveDownStart += OnTileStandingOnMoveDownStart;
        tileStandingOn.OnMoveUpEnd += OnTileStandingOnMoveUpEnd;
        tileStandingOn.OnMoveUpStart += OnTileStandingOnMoveUpStart;
    }

    public string[] GetEventArgsForDeserialization() {
        return new string[] { Coordinates.x.ToString(), Coordinates.z.ToString() };
    }

    public static string GetTypeName(Block block) {
        if (block.GetType() == typeof(Player))
            return "Player";
        else if (block.GetType() == typeof(BlockMoveable))
            return "Block (Moveable)";
        else if (block.GetType() == typeof(LaserSource))
            return "Laser Source";
        else if (block.GetType() == typeof(LaserReciever))
            return "Laser Reciever";
        else if (block.GetType() == typeof(LaserDiverter))
            return "Laser Diverter";
        else if (block.GetType() == typeof(LaserDiverterMoveable))
            return "Laser Diverter (Moveable)";
        return "Block (Immoveable)";
    }

    #region Events
    protected virtual void OnIntroComplete() {
        SubscribeToTileEvents(tileStandingOn);
    }

    protected virtual void OnTileStandingOnMoveUpStart() {
        //transform.SetParent(tileStandingOn.TileMesh);
    }

    protected virtual void OnTileStandingOnMoveUpEnd() { }

    protected virtual void OnTileStandingOnMoveDownStart() {
        if(tileStandingOn != null)
            transform.SetParent(tileStandingOn.TileMesh);
    }

    protected virtual void OnTileStandingOnMoveDownEnd() { }

    protected virtual void OnDestroy() {
        GameEvents.OnIntroComplete -= OnIntroComplete;
    }
    #endregion
}

