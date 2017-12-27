﻿using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Serializing;

[Serializable, SelectionBase]
public class Tile : MonoBehaviour {

    public static readonly Vector3 SIZE = new Vector3(1, 0.2f, 1);
    public static readonly Vector3 POSITION_OFFSET = new Vector3(0.5f, 0f, 0.5f);

    public IntVector2 coordinates;// { get; private set; }
    public Block occupant;// { get; private set; }
    public bool IsUp { get; private set; }
    public Transform TileMesh { get { return transform.GetChild(0); } }

    public Action OnEnter, OnExit;
    public Action OnMoveDownStart, OnMoveDownEnd;
    public Action OnMoveUpStart, OnMoveUpEnd;

    [SerializeField]
    private bool moveUpAtStart = true;
    public bool MoveUpAtStart { get { return moveUpAtStart; } }

    private float DownHeight { get { return -10; } }
    private float UpHeight { get { return 0 - SIZE.y / 2; } }

    private Coroutine currentCoroutine;

    public void OnDeserialize(TileData data) {
        coordinates = new IntVector2(data.x, data.z);
        moveUpAtStart = data.moveUpAtStart;
    }

    private void Awake() {
        GameEvents.OnGameOver += OnGameOver;

        if (TileSettings.UseStartingTransitions)
            TileMesh.position = new Vector3(TileMesh.position.x, DownHeight, transform.position.z);

        if (moveUpAtStart) {
            float delay = Random.Range(0, TileSettings.MoveUpDelayRandomMax);
            float duration = TileSettings.MoveUpStandardDuration + Random.Range(0, TileSettings.MoveUpDurationMax);
            MoveUp(duration, delay);
        } else {
            TileMesh.gameObject.SetActive(false);
        }
    }

    public void SetCoordinates(IntVector2 coordinates) {
        this.coordinates = coordinates;
    }

    public virtual void Enter(Block block) {
        if(OnEnter != null)
            OnEnter.Invoke();
        occupant = block;
    }

    public virtual void Exit(Block block) {
        if(OnExit != null)
            OnExit.Invoke();
        if(occupant == block)
            occupant = null;
    }

    private void OnMoveUpStartFunction() {
        IsUp = true;
        TileMesh.gameObject.SetActive(true);
        if (OnMoveUpStart != null)
            OnMoveUpStart.Invoke();
    }

    private void OnMoveUpEndFunction() {
        if (OnMoveUpEnd != null)
            OnMoveUpEnd.Invoke();
    }

    private void OnMoveDownStartFunction() {
        IsUp = false;
        if (OnMoveDownStart != null) 
            OnMoveDownStart.Invoke();
    }

    private void OnMoveDownEndFunction() {
        TileMesh.gameObject.SetActive(false);
        if (OnMoveDownEnd != null)
            OnMoveDownEnd.Invoke();
    }

    private void MoveUp(float duration, float delay) {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        //currentCoroutine = StartCoroutine(Tween.Scale(transform, 0, moveUpStandardDuration, Vector3.zero, Vector3.one, OnMoveUpStartFunction, OnMoveUpEndFunction));
        //Vector3 start = new Vector3(transform.position.x, transform.position.y - Random.Range(10, 20), transform.position.z);
        //StartCoroutine(Tween.Move(transform, 0, .75f, start, transform.position, null));
        Vector3 start = new Vector3(TileMesh.position.x, DownHeight, TileMesh.position.z);
        Vector3 target = new Vector3(TileMesh.position.x, UpHeight, TileMesh.position.z);
        currentCoroutine = StartCoroutine(Tween.MoveBetweenRemaining(TileMesh, delay, duration, start, target, OnMoveUpStartFunction, OnMoveUpEndFunction));
    }

    public void MoveDown(float duration, float delay) {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        if (occupant != null)
            occupant.transform.SetParent(TileMesh);

        Vector3 start = new Vector3(TileMesh.position.x, UpHeight, TileMesh.position.z);
        Vector3 target = new Vector3(TileMesh.position.x, DownHeight, TileMesh.position.z);
        currentCoroutine = StartCoroutine(Tween.MoveBetweenRemaining(TileMesh, delay, duration, start, target, OnMoveDownStartFunction, OnMoveDownEndFunction));
    }

    public void ToggleMovement() {
        if (IsUp)
            MoveDown(TileSettings.MoveDownStandardDuration, 0);
        else
            MoveUp(TileSettings.MoveUpStandardDuration, 0);
    }

    public void SetOccupant(Block occupant) {
        this.occupant = occupant;
    }

    private void OnGameOver() {
        if(!IsUp)
            return;

        float delay = TileSettings.TransitionOnGameOverDelay + Random.Range(0, TileSettings.TransitionOnGameOverDurationRandomMax);
        MoveDown(TileSettings.MoveDownStandardDuration, delay);
        GameEvents.OnGameOver -= OnGameOver;
    }

    private void OnDrawGizmos() {
        if (moveUpAtStart || IsUp)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y - SIZE.y / 2, transform.position.z), SIZE);
        GizmosExtension.DrawArrow(transform.position, Vector3.down, Gizmos.color, .3f, 30);
    }
}