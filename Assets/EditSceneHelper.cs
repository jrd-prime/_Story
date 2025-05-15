#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class EditSceneHelper
{
    private const string BootstrapScenePath = "Assets/_StoryGame/Static/Scenes/Bootstrap.unity";
    private const string GameScenePath = "Assets/_StoryGame/Static/Scenes/GameScene.unity";

    static EditSceneHelper() => EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.ExitingEditMode:
            {
                for (var i = SceneManager.sceneCount - 1; i >= 0; i--)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    if (scene.IsValid() && scene.isLoaded)
                        EditorSceneManager.CloseScene(scene, true);
                }

                if (File.Exists(BootstrapScenePath))
                    EditorSceneManager.OpenScene(BootstrapScenePath, OpenSceneMode.Single);
                else
                    Debug.LogError($"Бутстрап-сцена не найдена по пути: {BootstrapScenePath}");
                break;
            }
            case PlayModeStateChange.EnteredEditMode:
            {
                var isGameSceneOpen = false;
                for (var i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    if (!scene.IsValid() || scene.path != GameScenePath) continue;
                    isGameSceneOpen = true;
                    break;
                }

                if (isGameSceneOpen) return;
                if (File.Exists(GameScenePath))
                    EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
                else
                    Debug.LogError($"Игровая сцена не найдена по пути: {GameScenePath}");
                break;
            }
        }
    }
}

#endif