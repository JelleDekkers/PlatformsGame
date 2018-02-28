using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

    [SerializeField] private new Renderer renderer;
    [SerializeField] private float materialOffsetSpeed = 0.4f;

    private Transform mesh;
    private GameObject objectCurrentlyHitting;
    private ILaserHittable hittableObjectCurrentlyHitting;
    private RaycastHit hit;
    private LaserSource source;
    private const float MAX_RAY_LENGTH = 1000f;
    private Material material;
    private Vector2 materialTiling;
    private Vector2 materialOffset;

    public void Init(LaserSource source) {
        enabled = true;
        mesh = transform.GetChild(0);
        this.source = source;
        material = renderer.material;
        materialTiling = transform.parent.localScale;
        materialOffset = Vector3.zero;
    }

    private void Update() {
        Fire();
        materialTiling.y = transform.GetChild(0).localScale.y;
        material.mainTextureScale = materialTiling; 
        materialOffset.y = Time.time * materialOffsetSpeed * -1;
        material.mainTextureOffset = materialOffset;
    }

    public void ChangeColor(Color color) {
        material.color = color;
    }

    public void SetActive(bool active) {
        if (!active) {
            objectCurrentlyHitting = null;
            hittableObjectCurrentlyHitting = null;
        }
        gameObject.SetActive(active);
    }

    public void ScaleMesh(float distance) {
        distance *= 0.5f;
        mesh.localScale = new Vector3(mesh.localScale.x, distance, mesh.localScale.z);
    }

    public void Fire() {
        if (Physics.Raycast(transform.position, transform.forward, out hit, MAX_RAY_LENGTH)) {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red);
            LaserHitObject(hit.collider.gameObject);
            ScaleMesh(hit.distance);
        } else {
            LaserHitNothing();
            Debug.DrawRay(transform.position, transform.forward * MAX_RAY_LENGTH, Color.red);
            ScaleMesh(MAX_RAY_LENGTH);
        }
    }

    private void LaserHitObject(GameObject gameObjectHit) {
        if (gameObjectHit == objectCurrentlyHitting)
            return;

        if (hittableObjectCurrentlyHitting != null)
            hittableObjectCurrentlyHitting.OnLaserHitEnd();

        if (gameObjectHit.GetInterface<ILaserHittable>() != null) {
            hittableObjectCurrentlyHitting = gameObjectHit.GetInterface<ILaserHittable>();
            hittableObjectCurrentlyHitting.OnLaserHitStart(source);
        } else {
            hittableObjectCurrentlyHitting = null;
        }

        objectCurrentlyHitting = gameObjectHit;
    }

    private void LaserHitNothing() {
        if (hittableObjectCurrentlyHitting != null) {
            hittableObjectCurrentlyHitting.OnLaserHitEnd();
            hittableObjectCurrentlyHitting = null;
        }
        objectCurrentlyHitting = null;
    }

    private void OnDisable() {
        if (hittableObjectCurrentlyHitting != null)
            hittableObjectCurrentlyHitting.OnLaserHitEnd();
        hittableObjectCurrentlyHitting = null;
        objectCurrentlyHitting = null;
    }
}
