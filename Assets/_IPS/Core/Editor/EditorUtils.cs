using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using IPS;
using System.IO;

public class EditorUtils {
    [MenuItem("GAME/PLAY", priority = 10)]
    public static void Play() {
#if GDPR
        OpenGDPRScene();
#elif ADS || FIREBASE
        OpenSplashScene();
#else
        OpenGameSene();
#endif
        EditorApplication.isPlaying = true;
    }

#if GDPR
    [MenuItem("GAME/Scenes/Open GDPR Scene", priority = 1)]
    public static void OpenGDPRScene() {
        EditorSceneManager.OpenScene($"Assets/{ApiSettings.LIB_FOLDER}/Api/GDPR/GDPR.unity");
    }
#endif

#if ADS || FIREBASE
    [MenuItem("GAME/Scenes/Open Splash Scene", priority = 1)]
    public static void OpenSplashScene() {
        EditorSceneManager.OpenScene($"Assets/{ApiSettings.LIB_FOLDER}/Common/SplashScene/SplashScene.unity");
    }
#endif

    [MenuItem("GAME/Scenes/Open Game Scene", priority = 3)]
    public static void OpenGameSene() {
        EditorSceneManager.OpenScene("Assets/_GAME/Scenes/GameScene.unity");
    }

    [MenuItem("GAME/ClearAllData", priority = 1000)]
    public static void ClearAllData() {
        if (EditorUtility.DisplayDialog("Clear all", "Do you want to clear all data?", "Yes")) {
            PlayerPrefs.DeleteAll();
            if (Directory.Exists(Application.persistentDataPath)) {
                Directory.Delete(Application.persistentDataPath);
            }
        }
    }
}
