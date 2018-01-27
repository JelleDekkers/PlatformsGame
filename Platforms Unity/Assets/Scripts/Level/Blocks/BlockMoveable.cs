using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class BlockMoveable : Block {

    [SerializeField]
    protected bool isMoving = false; // public get, protected set

    protected Coroutine currentCoroutine;
    protected Rigidbody rBody;
    protected Action OnFall;

    protected override void Awake() {
        base.Awake();
        rBody = GetComponent<Rigidbody>();
        OnFall += DestroySelfWithTimer;
    }

    protected override void IntroTransition() {
        isMoving = true;
        base.IntroTransition();
    }

    private bool CanMoveInDirection(IntVector2 direction) {
        // player.canmove moet hiervan gebruik maken
        return true;
    }

    public virtual void MoveInDirection(IntVector2 direction, float duration) {
        Tile newTile = LevelManager.CurrentLevel.Tiles.GetTile(tileStandingOn.coordinates + direction);

        if (LevelManager.CurrentLevel.Walls.ContainsWall(tileStandingOn.coordinates, tileStandingOn.coordinates + direction)) {
            Portal portal = LevelManager.CurrentLevel.Walls.GetWall(tileStandingOn.coordinates, tileStandingOn.coordinates + direction) as Portal;
            portal.Teleport(this, tileStandingOn.coordinates, duration);
            StartCoroutine(MoveCoroutineThroughPortal(direction, duration));
            return;
        }

        if (newTile != null && newTile.IsUp)
            StartCoroutine(MoveCoroutine(newTile, direction, duration));
        else
            Fall(direction, duration);
    }

    public void MoveFromPortal(IntVector2 direction, IntVector2 from, float duration) {
        Tile newTile = LevelManager.CurrentLevel.Tiles.GetTile(from + direction);
        if (newTile != null)
            StartCoroutine(MoveCoroutine(newTile, direction, duration));
        else
            Fall(direction, duration);
    }

    public virtual void Fall(IntVector2 direction, float duration) {
        isMoving = true;
        if (tileStandingOn != null)
            tileStandingOn.SetOccupant(null);
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(FallCoroutine(direction, duration));
    }

    protected virtual IEnumerator MoveCoroutine(Tile newTile, IntVector2 direction, float duration) {
        OnMoveStart(newTile);

        float time = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + direction.ToVector3();

        while (time < duration * 0.95f) {
            transform.position = Vector3.Lerp(startPos, endPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        OnMoveEnd(endPos, direction);
    }

    protected virtual IEnumerator MoveCoroutineThroughPortal(IntVector2 direction, float duration) {
        OnMoveStart(null);

        float time = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + direction.ToVector3();

        while (time < duration * 0.95f) {
            transform.position = Vector3.Lerp(startPos, endPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        isMoving = false;
        Destroy(gameObject);
    }

    protected virtual IEnumerator FallCoroutine(IntVector2 direction, float movementDurationBeforeFall) {
        isMoving = true;
        Tile tileWasStandingOn = tileStandingOn;
        tileStandingOn.Exit(this);
        tileStandingOn = null;

        float time = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + direction.ToVector3();
        while (time < movementDurationBeforeFall * 0.5f) {
            transform.position = Vector3.Lerp(startPos, endPos, time / movementDurationBeforeFall);
            time += Time.deltaTime;
            yield return null;
        }

        if(OnFall != null)
            OnFall.Invoke();
        Rigidbody rBody = gameObject.GetComponent<Rigidbody>();
        StartCoroutine(tileWasStandingOn.EnableColliderTemporarily());
        rBody.isKinematic = false;
        rBody.AddForce(direction.ToVector3() * 100, ForceMode.Acceleration);
    }

    #region Events
    protected override void OnIntroComplete() {
        base.OnIntroComplete();
        isMoving = false;
    }

    protected virtual void OnMoveStart(Tile newTile) {
        isMoving = true;

        if (tileStandingOn != null) {
            tileStandingOn.Exit(this);
            UnSubscribeToTileEvents(tileStandingOn);
        }

        tileStandingOn = newTile;

        if (newTile != null) {
            newTile.Enter(this);
            SubscribeToTileEvents(newTile);

        }
    }

    private void OnMoveEnd(Vector3 endPos, IntVector2 direction) {
        isMoving = false;
        if (tileStandingOn.GetType() == typeof(SlideTile) && CanMoveInDirection(direction)) {
            Debug.Log("landed on slide Tile, moving again");
            MoveInDirection(direction, BlockSettings.MoveDuration);
        }
    }

    protected override void OnTileStandingOnMoveDownStart() {
        base.OnTileStandingOnMoveDownStart();
        isMoving = true;
        if (OnFall != null)
            OnFall.Invoke();
    }

    protected override void OnTileStandingOnMoveUpEnd() {
        base.OnTileStandingOnMoveUpEnd();
        transform.SetParent(null);
    }

    protected override void OnTileStandingOnMoveDownEnd() {
        UnSubscribeToTileEvents(tileStandingOn);
        Destroy(gameObject);
    }
    #endregion
}
