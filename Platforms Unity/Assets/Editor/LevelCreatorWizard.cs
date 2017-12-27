using UnityEditor;
using UnityEngine;

public class LevelCreator : ScriptableWizard {

    [SerializeField]
    private int chapterNr = 1;
    [SerializeField]
    private int levelNr = 1;

    private static int ChapterNr = 1;
    private static int LevelNr = 1;

    [MenuItem("GameObject/Create Light Wizard")]
    public static void CreateWizard() {
        DisplayWizard<LevelCreator>("Create new level", "Create", "Cancel").Init();
    }

    private void Init() {
        chapterNr = ChapterNr;
        levelNr = LevelNr;
    }

    void OnWizardCreate() {
        //LevelsHandler.AddNewLevel(CreateFileName());
        LevelNr = levelNr;
        ChapterNr = chapterNr;
    }

    void OnWizardUpdate() {
        helpString = "Save new level as: " + CreateFileName();
    }

    private string CreateFileName() {
        return "Chapter_00" + chapterNr + "_Level_00" + levelNr;
    }

    void OnWizardOtherButton() {
        Close();
    }
}