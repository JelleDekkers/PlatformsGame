using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

    private Transform mesh;
    private ILaserDiverter diverterCurrentlyHitting;
    private RaycastHit hit;
    private LaserSource source;
    private const float MAX_RAY_LENGTH = 1000f;
 
    public void Init(LaserSource source) {
        mesh = transform.GetChild(0);
        this.source = source;
    }

    public void SetActive(bool active) {
        gameObject.SetActive(active);
        if(!active)
            diverterCurrentlyHitting = null;
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
        if (gameObjectHit.GetComponent<LaserReciever>()) {
            Debug.Log("reciever hit");
            return;
        }

        ILaserDiverter newDiverterHit = gameObjectHit.GetInterface<ILaserDiverter>();

        if (newDiverterHit == diverterCurrentlyHitting)
            return;

        if (diverterCurrentlyHitting != null) {
            source.RemoveDiverter(diverterCurrentlyHitting);
            diverterCurrentlyHitting = null;
        }

        if (newDiverterHit != null && !source.Diverters.Contains(newDiverterHit)) {
            source.AddDiverter(newDiverterHit);
            diverterCurrentlyHitting = newDiverterHit;
        }
    }

    private void LaserHitNothing() {
        if (diverterCurrentlyHitting != null) {
            source.RemoveDiverter(diverterCurrentlyHitting);
            diverterCurrentlyHitting = null;
        }
    }
}
