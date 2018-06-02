using System;
using UnityEngine;
using Serialization;
using Random = UnityEngine.Random;

[Serializable, SelectionBase]
public class Block : MonoBehaviour, ISerializableGameObject {

    public static readonly Vector3 POSITION_OFFSET = new Vector3(0.5f, 0.5f, 0.5f);
    public static readonly Vector3 SIZE = new Vector3(1f, 1f, 1f);

    private GUID guid;
    public GUID Guid {
        get {
            if (guid == null || guid.ID == 0)
                guid = new GUID(GetInstanceID(), this);
            return guid;
        }
        set {
            guid = value;
        }
    }
    public IntVector2 Coordinates { get { return tileStandingOn.coordinates; } }

    public Tile tileStandingOn;

    private Renderer rend;

    protected virtual void Awake() {
        if(GeneralConfig.UseTransitionAnimations)
            GameEvents.OnLevelStart += IntroTransition;
        GameEvents.OnIntroComplete += OnIntroComplete;
    }

    protected virtual void IntroTransition() {
        Vector3 target = transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y + BlockConfig.IntroAnimationStartingHeight, transform.position.z);
        float duration = BlockConfig.IntroAnimationDuration.GetRandom();
        float delay = BlockConfig.IntroAnimationDelay.GetRandom();
        StartCoroutine(Tween.MoveBetween(transform, delay, duration, transform.position, target));
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
        Destroy(gameObject, BlockConfig.DestroyDelayOnFall);
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

    #region Serialization
    public virtual DataContainer Serialize() {
        return new BlockData(this);
    }

    public virtual object Deserialize(DataContainer data) {
        BlockData parsedData = data as BlockData;
        IntVector2 coordinates = new IntVector2(parsedData.x, parsedData.z);
        Tile tile = LevelManager.CurrentLevel.Tiles[coordinates];
        transform.position = new Vector3(tile.transform.position.x, Block.POSITION_OFFSET.y, tile.transform.position.z);
        Vector3 rotation = transform.eulerAngles;
        rotation.y = parsedData.yRot;
        transform.eulerAngles = rotation;
        name = GetType().FullName + " " + coordinates;
        tile.SetOccupant(this);
        SetTileStandingOn(tile);
        return parsedData;
    }
    #endregion

    #region Events
    protected virtual void OnIntroComplete() {
        SubscribeToTileEvents(tileStandingOn);
    }

    protected virtual void OnTileStandingOnMoveUpStart() { }

    protected virtual void OnTileStandingOnMoveUpEnd() {
        transform.SetParent(LevelManager.Instance.transform);
    }

    protected virtual void OnTileStandingOnMoveDownStart() {
        if(tileStandingOn != null)
            transform.SetParent(tileStandingOn.MeshParent);
    }

    protected virtual void OnTileStandingOnMoveDownEnd() {
        UnSubscribeToTileEvents(tileStandingOn);
    }

    protected virtual void OnDestroy() {
        GameEvents.OnIntroComplete -= OnIntroComplete;
        GameEvents.OnLevelStart -= IntroTransition;
    }
    #endregion

    #region Unity Actions
    public void RotateClockWise(int amount) {
        transform.Rotate(Vector3.up * amount);
    }
    #endregion
}

