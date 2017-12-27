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

    private IInputSystem input;
    private int horizontalInput, verticalInput;

    protected override void Awake() {
        base.Awake();
        instance = this;
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

    public void TryMoveInDirection(IntVector2 direction) {
        List<MovementInfo> connections;
        if (CanMoveInDirection(direction, out connections)) {
            connections.Add(new MovementInfo(this, direction));
            for (int i = connections.Count - 1; i >= 0; i--)
                connections[i].block.MoveInDirection(connections[i].direction, BlockSettings.MoveDuration);
        }
    }

    private bool CanMoveInDirection(IntVector2 direction, out List<MovementInfo> connectedBlocks) {
        connectedBlocks = new List<MovementInfo>();
        int maxNeighboursPossible = LevelManager.CurrentLevel.Tiles.Count;
        IntVector2 neighbourCoordinates = tileStandingOn.coordinates;

        for (int i = 1; i < maxNeighboursPossible; i++) {
            neighbourCoordinates += direction;
            
            if(LevelManager.CurrentLevel.Walls.ContainsWall(tileStandingOn.coordinates, neighbourCoordinates)) {
                Portal portal = LevelManager.CurrentLevel.Walls.GetWall(tileStandingOn.coordinates, neighbourCoordinates) as Portal;
                neighbourCoordinates = portal.GetPortalExitCoordinates(tileStandingOn.coordinates, out direction);
            }

            Tile neighbourTile = LevelManager.CurrentLevel.Tiles.GetTile(neighbourCoordinates);

            if (neighbourTile == null || !neighbourTile.IsUp)
                break;
            if (neighbourTile.occupant == null)
                break;
            else {
                BlockMoveable moveableNeighbour = neighbourTile.occupant.GetComponent<BlockMoveable>();
                if (moveableNeighbour == null)
                    return false;
                else
                    connectedBlocks.Add(new MovementInfo(moveableNeighbour, direction));
            }
        }
        return true;
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

    protected override IEnumerator MoveCoroutineThroughPortal(IntVector2 direction, float duration) {
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

    protected override IEnumerator FallCoroutine(IntVector2 direction, float duration) {
        isMoving = true;
        if(tileStandingOn != null)
            tileStandingOn.Exit(null);
        tileStandingOn = null;

        float time = 0f;
        Vector3 rotationPivot = transform.position + (direction.ToVector3() / 2);
        rotationPivot.y = 0;
        Vector3 rotationDirection = new Vector3(direction.z, 0, -direction.x);
        isMoving = true;

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
