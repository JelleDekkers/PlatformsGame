using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    [SerializeField]
    private float distanceFromTarget = 30;
    [SerializeField]
    private float lerpSpeed = 1;

    private Transform Target { get { return Player.Instance.transform; } }
    private Camera cam;
    private Vector3 startingRotation;

    private void Start() {
        cam = GetComponent<Camera>();
        startingRotation = transform.eulerAngles;
        cam.transform.position = Vector3.Lerp(cam.transform.position, GetCameraTargetPosition(), 1);
        cam.transform.rotation = Quaternion.Euler(30, -45, 0);

        GameEvents.OnGameOver += OnGameOver;
        GameEvents.OnLevelLoaded += OnLevelLoaded;
    }

    private void OnGameOver() {
        enabled = false;
    }

    private void OnLevelLoaded() {
        enabled = true;
    }

    private void LateUpdate() {
        FollowPlayer();
    }

    private void FollowPlayer() {
        if (Player.Instance.tileStandingOn != null) {
            cam.transform.position = Vector3.Lerp(cam.transform.position, GetCameraTargetPosition(), lerpSpeed * Time.deltaTime);
        }
    }

    private Vector3 GetCameraTargetPosition() {
        return new Vector3(Player.Instance.tileStandingOn.transform.position.x, 
                           Player.Instance.tileStandingOn.transform.position.y + Block.POSITION_OFFSET.y, 
                           Player.Instance.tileStandingOn.transform.position.z) - (Quaternion.Euler(startingRotation) * Vector3.forward * distanceFromTarget);
    }

    private void OnDestroy() {
        GameEvents.OnGameOver -= OnGameOver;
        GameEvents.OnLevelLoaded -= OnLevelLoaded;
    }
}
