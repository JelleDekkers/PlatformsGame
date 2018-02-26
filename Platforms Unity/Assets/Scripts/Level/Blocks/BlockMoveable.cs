using System;
using System.Collections;
using System.Collections.Generic;
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

    public virtual bool CanMoveInDirection(IntVector2 direction, out MovementInfo movementInfo) {
        IntVector2 neighbourCoordinates = tileStandingOn.coordinates + direction;
        movementInfo = new MovementInfo(this, direction, direction, null);

        if (LevelManager.CurrentLevel.Walls.ContainsWall(tileStandingOn.coordinates, neighbourCoordinates)) {
            Portal portal = LevelManager.CurrentLevel.Walls.GetWall(tileStandingOn.coordinates, neighbourCoordinates) as Portal;
            if (portal.CanTeleport()) 
                neighbourCoordinates = portal.GetPortalExitCoordinates(tileStandingOn.coordinates, out movementInfo.newDirection);
        }

        Tile neighbourTile = LevelManager.CurrentLevel.Tiles.GetTile(neighbourCoordinates);

        if (neighbourTile != null && neighbourTile.IsUp) {
            if (neighbourTile.occupant == null) {
                return true;
            } else {
                movementInfo.neighbourBlock = neighbourTile.occupant;
                return true;
            }
        }
        return true;
    }

    public virtual void MoveInDirection(IntVector2 direction, float duration) {
        Tile newTile = LevelManager.CurrentLevel.Tiles.GetTile(tileStandingOn.coordinates + direction);

        if (LevelManager.CurrentLevel.Walls.ContainsWall(tileStandingOn.coordinates, tileStandingOn.coordinates + direction)) {
            Portal portal = LevelManager.CurrentLevel.Walls.GetWall(tileStandingOn.coordinates, tileStandingOn.coordinates + direction) as Portal;
            if (portal.CanTeleport()) {
                portal.Teleport(this, tileStandingOn.coordinates, duration);
                StartCoroutine(MoveCoroutineThroughPortal(direction, duration));
                return;
            }
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
        Tile tileWasStandingOn = tileStandingOn;
        OnMoveStart(null);

        float time = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + direction.ToVector3();

        while (time < movementDurationBeforeFall * BlockSettings.FallCutOff) {
            transform.position = Vector3.Lerp(startPos, endPos, time / movementDurationBeforeFall);
            time += Time.deltaTime;
            yield return null;
        }

        if(OnFall != null)
            OnFall.Invoke();
        Rigidbody rBody = gameObject.GetComponent<Rigidbody>();
        StartCoroutine(tileWasStandingOn.EnableColliderTemporarily());
        rBody.isKinematic = false;
        rBody.AddForce(direction.ToVector3() * BlockSettings.FallForce, ForceMode.Acceleration);
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

        if (tileStandingOn != null && tileStandingOn.GetType() == typeof(SlideTile))
            OnMoveEndedOnSlideTile(direction);    
    }

    private void OnMoveEndedOnSlideTile(IntVector2 directionSlidTo) {
        MovementInfo info;
        if (CanMoveInDirection(directionSlidTo, out info)) {
            if (info.neighbourBlock == null) {
                MoveInDirection(info.newDirection, BlockSettings.MoveDuration);
                // in case I do want sliding blocks to push moveable blocks one tile further:
            //} else if (info.neighbourBlock.GetType() == typeof(BlockMoveable)) {
            //    BlockMoveable newNeighbourBlock = info.neighbourBlock as BlockMoveable;
            //    if (newNeighbourBlock.CanMoveInDirection(info.newDirection, out info))
            //        newNeighbourBlock.MoveInDirection(info.newDirection, BlockSettings.MoveDuration);
            }
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
