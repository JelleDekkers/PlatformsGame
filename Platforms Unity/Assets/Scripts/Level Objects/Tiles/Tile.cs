using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using Serializing;

[Serializable, SelectionBase]
public class Tile : MonoBehaviour, ISerializableEventTarget {

    public static readonly Vector3 SIZE = new Vector3(1, 0.2f, 1);
    public static readonly Vector3 POSITION_OFFSET = new Vector3(0.5f, 0f, 0.5f);

    public IntVector2 coordinates;// { get; private set; }
    public Block occupant;
    public bool IsInUpState { get; private set; }
    public Transform MeshParent { get { return transform.GetChild(0); } }

    public Action OnMoveDownStart, OnMoveDownEnd;
    public Action OnMoveUpStart, OnMoveUpEnd;

    [SerializeField]
    private bool moveUpAtStart = true;
    public bool MoveUpAtStart { get { return moveUpAtStart; } }

    private float DownHeight { get { return TileConfig.DisabledPositionHeight; } }
    private float UpHeight { get { return 0 - SIZE.y / 2; } }

    private Coroutine currentCoroutine;

    private void Awake() {
        GameEvents.OnGameOver += OnGameOver;

        if (moveUpAtStart) {
            if (GeneralConfig.UseTransitionAnimations) {
                MeshParent.position = new Vector3(MeshParent.position.x, DownHeight, MeshParent.position.z);
                MoveUpRandomized();
            } else {
                OnMoveUpStartFunction();
                OnMoveUpEndFunction();
            }
        } else {
            MeshParent.gameObject.SetActive(false);
        }
    }

    public IEnumerator EnableColliderTemporarily() {
        BoxCollider col = MeshParent.gameObject.AddComponent<BoxCollider>();
        col.center = new Vector3(0, -0.4f, 0);
        float timer = 0;
        while (timer < TileConfig.TempColliderEnabledDuration) {
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(col);
    }

    public void SetCoordinates(IntVector2 coordinates) {
        this.coordinates = coordinates;
    }

    public virtual void Enter(Block block) {
        occupant = block;
    }

    public virtual void Exit(Block block) {
        if (occupant == block)
            occupant = null;
    }

    public void MoveUpRandomized() {
        float delay = TileConfig.MoveUpAnimationDelay.GetRandom();
        float duration = TileConfig.MoveUpAnimationDuration.GetRandom();
        Vector3 start = new Vector3(MeshParent.localPosition.x, DownHeight, MeshParent.localPosition.z);
        Vector3 target = new Vector3(MeshParent.localPosition.x, UpHeight, MeshParent.localPosition.z);
        currentCoroutine = StartCoroutine(Tween.MoveBetween(MeshParent, delay, duration, start, target, OnMoveUpStartFunction, OnMoveUpEndFunction));
    }

    public void MoveUp() {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        Vector3 start = new Vector3(MeshParent.localPosition.x, DownHeight, MeshParent.localPosition.z);
        Vector3 target = new Vector3(MeshParent.localPosition.x, UpHeight, MeshParent.localPosition.z);
        currentCoroutine = StartCoroutine(Tween.MoveBetweenRemaining(MeshParent, 0, TileConfig.MoveUpStandardAnimationDuration, start, target, OnMoveUpStartFunction, OnMoveUpEndFunction));
    }

    public void MoveDownRandomized() {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        if (occupant != null)
            occupant.transform.SetParent(MeshParent);
        
        float delay = TileConfig.MoveDownAnimationDelay.GetRandom();
        Vector3 start = new Vector3(MeshParent.localPosition.x, UpHeight, MeshParent.localPosition.z);
        Vector3 target = new Vector3(MeshParent.localPosition.x, DownHeight, MeshParent.localPosition.z);
        currentCoroutine = StartCoroutine(Tween.MoveBetween(MeshParent, delay, TileConfig.MoveDownAnimationDuration, start, target, OnMoveDownStartFunction, OnMoveDownEndFunction));
    }

    public void MoveDown() {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        if (occupant != null)
            occupant.transform.SetParent(MeshParent);

        Vector3 start = new Vector3(MeshParent.localPosition.x, UpHeight, MeshParent.localPosition.z);
        Vector3 target = new Vector3(MeshParent.localPosition.x, DownHeight, MeshParent.localPosition.z);
        currentCoroutine = StartCoroutine(Tween.MoveBetweenRemaining(MeshParent, 0, TileConfig.MoveDownAnimationDuration, start, target, OnMoveDownStartFunction, OnMoveDownEndFunction));
    }

    public void TogglePositionState() {
        if (IsInUpState)
            MoveDown();
        else
            MoveUp();
    }

    public void SetOccupant(Block occupant) {
        this.occupant = occupant;
    }

    private void OnGameOver() {
        if (!IsInUpState)
            return;

        MoveDownRandomized();
        GameEvents.OnGameOver -= OnGameOver;
    }

    private void OnDrawGizmos() {
        if (moveUpAtStart || IsInUpState)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y - SIZE.y / 2, transform.position.z), SIZE);
        GizmosExtension.DrawArrow(transform.position, Vector3.down, Gizmos.color, .3f, 30);
    }

    public string[] GetEventArgsForDeserialization() {
        return new string[] { coordinates.x.ToString(), coordinates.z.ToString() };
    }

    #region Serialization
    //public TileData2 Serialize() {
    //    return new TileData2(this);
    //}

    //public bool Deserialize(TileData2 data) {
    //    return true;
    //}

    public virtual void Deserialize(TileData data) {
        //coordinates = new IntVector2(data.x, data.z);
        //moveUpAtStart = data.moveUpAtStart;
    }

    public virtual void DeserializeEvents(TileData data) { }
    #endregion

    #region Events
    private void OnMoveUpStartFunction() {
        IsInUpState = true;
        MeshParent.gameObject.SetActive(true);
        if (OnMoveUpStart != null)
            OnMoveUpStart.Invoke();
    }

    private void OnMoveUpEndFunction() {
        if (OnMoveUpEnd != null)
            OnMoveUpEnd.Invoke();
    }

    private void OnMoveDownStartFunction() {
        IsInUpState = false;
        if (OnMoveDownStart != null)
            OnMoveDownStart.Invoke();
    }

    private void OnMoveDownEndFunction() {
        MeshParent.gameObject.SetActive(false);
        if (OnMoveDownEnd != null)
            OnMoveDownEnd.Invoke();
    }
    #endregion
}
