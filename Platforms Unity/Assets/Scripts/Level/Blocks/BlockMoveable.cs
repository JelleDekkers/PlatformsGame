using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlockMoveable : Block, ILaserHittable {

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

    public virtual MovementInfo GetMovementInfo(IntVector2 direction, out bool canMove) {
        IntVector2 neighbourCoordinates = tileStandingOn.coordinates + direction;
        Tile neighbourTile = LevelManager.CurrentLevel.Tiles.GetTile(neighbourCoordinates);
        MovementInfo movementInfo = new MovementInfo(this, direction, direction, null, neighbourTile, null);

        if (LevelManager.CurrentLevel.Walls.ContainsWall(tileStandingOn.coordinates, neighbourCoordinates)) {
            Portal portal = LevelManager.CurrentLevel.Walls.GetWall(tileStandingOn.coordinates, neighbourCoordinates) as Portal;
            if (portal.CanTeleport()) {
                movementInfo.portal = portal;
                neighbourCoordinates = portal.GetPortalExitCoordinates(tileStandingOn.coordinates, out movementInfo.newDirection);
            }
        }

        if (neighbourTile != null && neighbourTile.IsUp) {
            // anders voor player:
            if (neighbourTile.GetType() == typeof(PlayerOnlyTile)) {
                canMove = false;
                return movementInfo;
            } else if (neighbourTile.occupant != null) {
                movementInfo.neighbourBlock = neighbourTile.occupant;
                if (neighbourTile.occupant.GetType() == typeof(BlockMoveable) || neighbourTile.occupant.GetType().BaseType == typeof(BlockMoveable)) {
                    canMove = true;
                    return movementInfo;
                } else {
                    canMove = false;
                    return movementInfo;
                }
            }
        }

        canMove = true;
        return movementInfo;
    }

    public virtual void Move(MovementInfo movement) {
        if (movement.portal != null) {
            movement.portal.Teleport(this, tileStandingOn.coordinates, BlockSettings.MoveDuration);
            StartCoroutine(MoveOutOfPortalCoroutine(movement.direction, BlockSettings.MoveDuration));
            return;
        }

        if (movement.newTile != null && movement.newTile.IsUp)
            StartCoroutine(MoveCoroutine(movement.newTile, movement.direction, BlockSettings.MoveDuration));
        else
            Fall(movement.direction, BlockSettings.MoveDuration);
    }
   
    public void MoveOutOfPortal(IntVector2 direction, IntVector2 from, float duration) {
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

    protected virtual IEnumerator MoveOutOfPortalCoroutine(IntVector2 direction, float duration) {
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

    public virtual void OnLaserHitStart(LaserSource source) {
        if (source.IsLethal)
            Destroy();
    }

    protected virtual void Destroy() {
        Debug.Log("destroy " + name);
    }

    public virtual void OnLaserHitEnd() { }

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
        bool canMove;
        MovementInfo movementInfo = GetMovementInfo(directionSlidTo, out canMove);
        if (canMove) {
            Move(movementInfo);
        }
        // in case I do want sliding blocks to push moveable blocks one tile further:
        //} else if (info.neighbourBlock.GetType() == typeof(BlockMoveable)) {
        //    BlockMoveable newNeighbourBlock = info.neighbourBlock as BlockMoveable;
        //    if (newNeighbourBlock.CanMoveInDirection(info.newDirection, out info))
        //        newNeighbourBlock.MoveInDirection(info.newDirection, BlockSettings.MoveDuration);        
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
