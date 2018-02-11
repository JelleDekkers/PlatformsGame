﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Serializing;

[SelectionBase]
public class Portal : Wall, ILaserDiverter, IActivatable, ISerializableEventTarget {

    public Laser Laser { get; private set; }
    public Transform Pivot { get { return transform.GetChild(0); } }
    public bool IsActive { get; private set; }

    [SerializeField]
    private bool isActiveOnStart;
    public bool IsActiveOnStart { get { return isActiveOnStart; } }
    [SerializeField]
    private Portal connectedPortal;
    public Portal ConnectedPortal { get { return connectedPortal; } }

    [SerializeField]
    private GameObject depthMaskFront, depthMaskBack;
    private GameObject portalFront;
    private GameObject portalBack;
    public BoxCollider RayBlockerFront { get; private set; }
    public BoxCollider RayBlockerBack { get; private set; }

    private const int DEPTH_MASK_VALUE = 3000;
    private const int NORMAL_MASK_VALUE = 2000;
    private BoxCollider boxCollider;

    private void Awake() {
        if (TileSettings.UseStartingTransitions)
            GameEvents.OnLevelStart += IntroTransition;
        GameEvents.OnIntroComplete += OnIntroComplete;
    
        IsActive = isActiveOnStart;
        boxCollider = GetComponent<BoxCollider>();
        portalFront = depthMaskFront.transform.parent.gameObject;
        portalBack = depthMaskBack.transform.parent.gameObject;
        RayBlockerFront = portalBack.GetComponent<BoxCollider>();
        RayBlockerBack = portalFront.GetComponent<BoxCollider>();
    }

    private void IntroTransition() {
        Vector3 start = new Vector3(transform.position.x, transform.position.y + BlockSettings.IntroStartingHeight, transform.position.z);
        float duration = BlockSettings.IntroStandardDuration + Random.Range(0, BlockSettings.IntroDurationMax);
        StartCoroutine(Tween.MoveBetween(transform, 0, duration, start, transform.position, null, null));
    }

    public void SetIsActiveOnStart(bool active) {
        isActiveOnStart = active;
    }

    public void SetEdge(TileEdge edge) {
        Edge = edge;
    }

    public void SetConnectedPortal(Portal p) {
        connectedPortal = p;
    }

    public void Teleport(BlockMoveable b, IntVector2 from, float duration) {
        if (connectedPortal == null)
            return;

        if (from == Edge.TileTwo)
            StartCoroutine(TeleportFromFront(b, duration));
        else
            StartCoroutine(TeleportFromBack(b, duration));
    }

    private IEnumerator TeleportFromFront(BlockMoveable block, float duration) {
        float time = 0;

        depthMaskFront.SetActive(true);
        RayBlockerFront.enabled = true;
        connectedPortal.RayBlockerBack.enabled = true;
        connectedPortal.depthMaskBack.SetActive(true);
        block.SetRenderQueue(DEPTH_MASK_VALUE);
        BlockMoveable copy = Instantiate(block, connectedPortal.Edge.TileTwo.ToVector3() + Block.POSITION_OFFSET, Quaternion.identity);
        copy.name = block.name;
        block.enabled = false;
        copy.SetTileStandingOn(null);
        IntVector2 moveDirection = new IntVector2(connectedPortal.Edge.TileOne.x - connectedPortal.Edge.TileTwo.x, connectedPortal.Edge.TileOne.z - connectedPortal.Edge.TileTwo.z);
        copy.MoveFromPortal(moveDirection, connectedPortal.Edge.TileTwo, duration);

        while (time < duration) {
            time += Time.deltaTime;
            yield return null;
        }

        depthMaskFront.SetActive(false);
        RayBlockerFront.enabled = false;
        connectedPortal.RayBlockerBack.enabled = false;
        connectedPortal.depthMaskBack.SetActive(false);
    }

    private IEnumerator TeleportFromBack(BlockMoveable block, float duration) {
        float time = 0;

        depthMaskBack.SetActive(true);
        RayBlockerBack.enabled = true;
        connectedPortal.RayBlockerFront.enabled = true;
        connectedPortal.depthMaskFront.SetActive(true);
        block.SetRenderQueue(DEPTH_MASK_VALUE);
        BlockMoveable copy = Instantiate(block, connectedPortal.Edge.TileOne.ToVector3() + Block.POSITION_OFFSET, Quaternion.identity);
        copy.name = block.name;
        block.enabled = false;
        copy.SetTileStandingOn(null);
        //IntVector2 moveDirection = connectedPortal.Edge.TileOne.ToAbsolute() - connectedPortal.Edge.TileTwo.ToAbsolute();
        IntVector2 moveDirection = new IntVector2(connectedPortal.Edge.TileTwo.x - connectedPortal.Edge.TileOne.x, connectedPortal.Edge.TileTwo.z - connectedPortal.Edge.TileOne.z);
        copy.MoveFromPortal(moveDirection, connectedPortal.Edge.TileOne, duration);

        while (time < duration) {
            time += Time.deltaTime;
            yield return null;
        }

        depthMaskBack.SetActive(false);
        RayBlockerBack.enabled = false;
        connectedPortal.RayBlockerFront.enabled = false;
        connectedPortal.depthMaskFront.SetActive(false);
    }

    public IntVector2 GetPortalExitCoordinates(IntVector2 entry, out IntVector2 direction) {
        if (entry == Edge.TileOne) {
            direction = new IntVector2(connectedPortal.Edge.TileTwo.x - connectedPortal.Edge.TileOne.x, connectedPortal.Edge.TileTwo.z - connectedPortal.Edge.TileOne.z);// connectedPortal.Edge.TileOne.ToAbsolute() - connectedPortal.Edge.TileTwo.ToAbsolute();
            return connectedPortal.Edge.TileTwo;
        } else {
            direction = new IntVector2(connectedPortal.Edge.TileOne.x - connectedPortal.Edge.TileTwo.x, connectedPortal.Edge.TileOne.z - connectedPortal.Edge.TileTwo.z);// connectedPortal.Edge.TileTwo.ToAbsolute() - connectedPortal.Edge.TileOne.ToAbsolute();
            return connectedPortal.Edge.TileOne;
        }
    }

    private void OnDrawGizmos() {
        GizmosExtension.DrawArrow(transform.GetChild(0).position, transform.right * 0.75f, Color.blue);
        GizmosExtension.DrawArrow(transform.GetChild(0).position, -transform.right * 0.75f, Color.red);
    }

    private void OnDrawGizmosSelected() {
        if(connectedPortal != null)
            GizmosExtension.DrawArrow(transform.GetChild(0).position, connectedPortal.transform.GetChild(0).position - transform.GetChild(0).position, Color.black);
    }

    public void OnIntroComplete() {
        if (isActiveOnStart && connectedPortal != null && connectedPortal.IsActive)
            Activate();
        else
            Deactivate();
    }

    public void OnDivertLaserStart(LaserSource src) {
        if (Laser == null) {
            Laser = transform.GetChild(0).GetComponent<Laser>();
            if (Laser == null) {
                Quaternion direction = Quaternion.Euler(transform.eulerAngles.x, connectedPortal.transform.eulerAngles.y + 90, connectedPortal.transform.eulerAngles.z);
                Laser = src.CreateNewLaser(connectedPortal.Pivot, direction);
            }
        }

        Laser.Init(src);
        Laser.SetActive(true);
    }

    public void OnDivertLaserEnd() {
        Laser.SetActive(false);
    }

    public void DivertLaser() {
        Laser.Fire();
    }

    public void Toggle() {
        if (IsActive)
            Deactivate();
        else
            Activate();
    }

    public void Activate() {
        IsActive = true;
        portalFront.SetActive(true);
        portalBack.SetActive(true);
        boxCollider.enabled = true;
    }

    public void Deactivate() {
        IsActive = false;
        portalFront.SetActive(false);
        portalBack.SetActive(false);
        boxCollider.enabled = false;
    }

    public string[] GetEventArgs() {
        return new string[] { Edge.TileOne.x.ToString(), Edge.TileOne.z.ToString(), Edge.TileTwo.x.ToString(), Edge.TileTwo.z.ToString() };
    }
}
