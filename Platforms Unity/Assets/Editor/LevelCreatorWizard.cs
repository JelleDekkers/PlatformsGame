using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LevelCreator : ScriptableWizard {

    [SerializeField] private string fileName = "";
    private static string persistentFileName;

    private static HashSet<string> levelFiles;
    private static HashSet<string> LevelFiles {
        get { if (levelFiles == null)
                levelFiles = GetLevelFileNames();
            return levelFiles;
        }
    }

    public static void CreateWizard() {
        DisplayWizard<LevelCreator>("Create new level", "Create").Init();
        levelFiles = GetLevelFileNames();
    }

    protected override bool DrawWizardGUI() {
        return base.DrawWizardGUI();
    }

    private void Init() {
        fileName = persistentFileName;
    }

    private void OnWizardCreate() {
        LevelManager.Instance.levelAsset = LevelsHandler.AddNewLevel(LevelManager.CurrentLevel, fileName);
        persistentFileName = fileName;
    }

    private void OnWizardUpdate() {
        helpString = "Enter a name to save the level";

        string error = "";
        isValid = IsValid(ref error);

        if (!isValid)
            errorString = error;
        else
            errorString = "";
    }

    private static HashSet<string> GetLevelFileNames() {
        string path = "Levels/";
        HashSet<string> temp = new HashSet<string>();
        foreach (Object o in Resources.LoadAll(path))
            temp.Add(o.name);
        return temp;
    }

    private bool IsValid(ref string error) {
        bool valid = true;
        if (fileName == "") {
            valid = false;
            error = "FileName can't be empty \n";
        } 
        if(LevelFiles.Contains(fileName)) {
            valid = false;
            error += "A file with this name already exists";
        }
        return valid;
    }
}