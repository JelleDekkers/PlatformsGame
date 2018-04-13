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

    public string[] GetEventArgsForDeserialization() {
        return new string[] { Coordinates.x.ToString(), Coordinates.z.ToString() };
    }

    #region Events
    protected virtual void OnIntroComplete() {
        SubscribeToTileEvents(tileStandingOn);
    }

    protected virtual void OnTileStandingOnMoveUpStart() { }

    protected virtual void OnTileStandingOnMoveUpEnd() {
        transform.SetParent(null);
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
    }
    #endregion

    #region Unity Actions
    public void RotateClockWise(int amount) {
        transform.Rotate(Vector3.up * amount);
    }
    #endregion
}

