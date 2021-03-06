﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorLevelBuilder : Editor {

    private static EditorLevelBuilder instance;

    private bool draggingMouse;
    private bool holdingCtrl;
    private GUIStyle guiStyle;

    private int buildModeSelectionIndex;
    private int buildModeSelectionIndexPrevFrame;
    private int subSelectionIndex = -1;
    private int buttonWidth = 90;
    private BuildMode buildMode;
    private Type[] buildModes = new Type[] { null, typeof(BuildModeTile), typeof(BuildModeBlock), typeof(BuildModeWall) };
    private string[] buildModeNames = new string[] { "None", "Tiles", "Blocks", "Walls" };

    [MenuItem("Level Tools/Level Builder")]
    public static void InitGUI() {
        if (instance == null) {
            instance = (EditorLevelBuilder)CreateInstance(typeof(EditorLevelBuilder));
            instance.Init();
            SceneView.RepaintAll();
            Selection.activeObject = null;
        } else {
            DestroyImmediate(instance);
            SceneView.RepaintAll();
        }
    }

    private void Init() {
        SceneView.onSceneGUIDelegate += RenderSceneGUI;
        guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.white;

        if(LevelManager.Instance == null) {
            Debug.Log("No instance of level manager found");
            DestroyImmediate(this);
        }
    }

    private void RenderSceneGUI(SceneView sceneview) {
        holdingCtrl = (Event.current.modifiers == EventModifiers.Control);
        
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(5, 5, 100, 20));
        buildModeSelectionIndex = EditorGUILayout.Popup(buildModeSelectionIndex, buildModeNames, GUILayout.Width(75));
        GUILayout.EndArea();
        Handles.EndGUI();

        if (buildModeSelectionIndex != buildModeSelectionIndexPrevFrame)
            SetNewBuildMode(buildModeSelectionIndex);
        buildModeSelectionIndexPrevFrame = buildModeSelectionIndex;

        if (buildMode != null)
            Update();

        if (Event.current.type == EventType.MouseDown)
            draggingMouse = true;
        else if (Event.current.type == EventType.MouseUp)
            draggingMouse = false;
    }

    private void SetNewBuildMode(int index) {
        Type t = buildModes[index];

        if (t == null)
            buildMode = null;
        else
            buildMode = (BuildMode)Activator.CreateInstance(t);

        Selection.activeObject = null;
        draggingMouse = false;
        subSelectionIndex = -1;
    }

    private void Update() {
        SelectionGridUI();

        // keeps levelBuilder focused:
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Keyboard));

        if (subSelectionIndex < 0)
            return;

        buildMode.Update();

        if (Event.current.isMouse && draggingMouse && Event.current.button == 0) {
            if (!HoldingCtrl())
                buildMode.Build(subSelectionIndex, EditorHelper.GetMousePositionInScene());
            else
                buildMode.Remove(EditorHelper.GetMousePositionInScene());
        }
    }

    private void SelectionGridUI() {
        Rect r = new Rect(85, 5, buttonWidth * buildMode.Prefabs.Length, 15);

        Handles.BeginGUI();
        subSelectionIndex = GUI.SelectionGrid(r, subSelectionIndex, buildMode.PrefabNames, buildMode.Prefabs.Length);
        GUI.Label(new Rect(10, 30, 1000, 20), "Right click to place.", guiStyle);
        GUI.Label(new Rect(10, 50, 1000, 20), "Hold Ctrl + right click to remove.", guiStyle);
            
        Handles.EndGUI();
    }

    private bool HoldingCtrl() {
        return Event.current.modifiers == EventModifiers.Control;
    }

    public static IntVector2 ConvertPositionToGridCoordinate(Vector3 position) {
        return new IntVector2(position.x.RoundToInt(), position.z.RoundToInt());
    }

    private void CleanUp() {
        SceneView.onSceneGUIDelegate -= RenderSceneGUI;
    }

    private void OnDestroy() {
        CleanUp();
    }
}


