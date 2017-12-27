using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class PostProcessingLogic {

#if !UNITY_IOS && !UNITY_ANDROID
    //[PostProcessBuild(1)]
    //public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
    //    MobileTouchManager mobileManager = (MobileTouchManager)GameObject.FindObjectOfType(typeof(MobileTouchManager));
    //    GameObject.DestroyImmediate(mobileManager.gameObject);
    //    Debug.Log("Current platform: " + Application.platform + " Destroyed Mobile Manager from build");
    //}

    [PostProcessScene]
    public static void OnPostprocessScene() {
        MobileTouchManager mobileManager = (MobileTouchManager)GameObject.FindObjectOfType(typeof(MobileTouchManager));
        GameObject.DestroyImmediate(mobileManager.gameObject);
        //Debug.Log("Current platform: " + Application.platform + " Destroyed Mobile Manager from scene");
    }
#endif
}
