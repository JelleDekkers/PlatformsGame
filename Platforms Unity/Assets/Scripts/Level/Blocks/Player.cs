using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player : BlockMoveable {

    private static Player instance;
    public static Player Instance {
        get  {
            if (instance == null)
                instance = FindObjectOfType<Player>();
            return instance;
        }
    }

    private static IInputSystem input;
    private int horizontalInput, verticalInput;

    protected override void Awake() {
        base.Awake();
        instance = this;
        if(input == null)
            input = InputSystem.GetPlatformDependentInputSystem(); 
        OnFall += GameEvents.OnGameOver;
    }

    protected override void IntroTransition() {
        isMoving = true;
        Vector3 target = transform.position;
        transform.position = new Vector3(transform.position.x, transform.position.y + PlayerSettings.IntroStartingHeight, transform.position.z);
        StartCoroutine(Tween.MoveBetween(transform, PlayerSettings.IntroDelay, PlayerSettings.IntroDuration, transform.position, target));
    }

    private void Update() {
        horizontalInput = (int)input.GetAxisRawHorizontal();
        verticalInput = (int)input.GetAxisRawVertical();

        if (CanMove(horizontalInput, verticalInput))
            TryMoveInDirection(new IntVector2(horizontalInput, verticalInput));
    }

    private bool CanMove(int horizontalInput, int verticalInput) {
        return !isMoving && tileStandingOn != null && (horizontalInput != 0 || verticalInput != 0);
    }

    private void TryMoveInDirection(IntVector2 direction) {
        List<MovementInfo> connections;
        int maxNeighboursPossible = LevelManager.CurrentLevel.Tiles.Count;
        bool canMove;

        // check self:
        MovementInfo movementInfo = GetMovementInfo(direction, out canMove);
        if (canMove)
            connections = new List<MovementInfo>() { movementInfo };
        else
            return;

        // get list of neighbouring connected blocks
        for (int i = 0; i < maxNeighboursPossible; i++) {
            if (movementInfo.neighbourBlock == null)
                break;
            else if (movementInfo.neighbourBlock.GetType() != typeof(BlockMoveable))
                return;
            else {
                BlockMoveable neighbourBlock = movementInfo.neighbourBlock as BlockMoveable;
                movementInfo = neighbourBlock.GetMovementInfo(movementInfo.newDirection, out canMove);
                if (canMove)
                    connections.Add(movementInfo);
                else
                    return;
            }
        }

        // move all connected blocks:
        for(int i = 0; i < connections.Count; i++) {
            connections[i].block.Move(connections[i]);
        }
    }

    public override MovementInfo GetMovementInfo(IntVector2 direction, out bool canMove) {
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
           if (neighbourTile.occupant != null) {
                movementInfo.neighbourBlock = neighbourTile.occupant;
                if (neighbourTile.occupant.GetType() == typeof(BlockMoveable)) {
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

    protected override IEnumerator MoveCoroutine(Tile newTile, IntVector2 direction, float duration) {
        OnMoveStart(newTile);

        float time = 0f;
        Vector3 rotationPivot = transform.position + (direction.ToVector3() / 2);
        rotationPivot.y = 0;
        Vector3 rotationDirection = new Vector3(direction.z, 0, -direction.x);
        Vector3 endPos = transform.position + direction.ToVector3();

        float angle = 90 / BlockSettings.MoveDuration;
        while(time < BlockSettings.MoveDuration * 0.95f) {
            transform.RotateAround(rotationPivot, rotationDirection, angle * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        transform.eulerAngles = new Vector3(Mathf.Round(transform.eulerAngles.x / 90) * 90, Mathf.Round(transform.eulerAngles.y / 90) * 90, Mathf.Round(transform.eulerAngles.z / 90) * 90);
        isMoving = false;
    }

    protected override IEnumerator MoveThroughPortalCoroutine(IntVector2 direction, float duration) {
        OnMoveStart(null);

        float time = 0f;
        Vector3 rotationPivot = transform.position + (direction.ToVector3() / 2);
        rotationPivot.y = 0;
        Vector3 rotationDirection = new Vector3(direction.z, 0, -direction.x);
        Vector3 endPos = transform.position + direction.ToVector3();

        float angle = 90 / BlockSettings.MoveDuration;
        while (time < BlockSettings.MoveDuration * 0.95f) {
            transform.RotateAround(rotationPivot, rotationDirection, angle * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        transform.eulerAngles = new Vector3(Mathf.Round(transform.eulerAngles.x / 90) * 90, Mathf.Round(transform.eulerAngles.y / 90) * 90, Mathf.Round(transform.eulerAngles.z / 90) * 90);
        Destroy(gameObject);
    }

    protected override IEnumerator FallCoroutine(IntVector2 direction, float movementDurationBeforeFall) {
        OnMoveStart(null);

        float time = 0f;
        Vector3 rotationPivot = transform.position + (direction.ToVector3() / 2);
        rotationPivot.y = 0;
        Vector3 rotationDirection = new Vector3(direction.z, 0, -direction.x);

        float angle = 90 / BlockSettings.MoveDuration;
        while (time < BlockSettings.MoveDuration) {
            transform.RotateAround(rotationPivot, rotationDirection, angle * Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }

        if (OnFall != null)
            OnFall.Invoke();
        GameEvents.OnGameOver.Invoke();

        rBody.isKinematic = false;
        rBody.AddForce(direction.ToVector3() * BlockSettings.FallDirectionalForce);
        rBody.AddForce(Vector3.down * BlockSettings.FallDownwardForce);
        rBody.AddTorque(rotationDirection * BlockSettings.FallRotationalForce);
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        OnFall -= GameEvents.OnGameOver;
    }
}
