using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class CustomEditorToolbar
{
    static CustomEditorToolbar()
    {
        // Ajouter le bouton à la barre d'outils
        ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
    }

    private static void OnToolbarGUI()
    {
        if (GUILayout.Button("StartMenuScene", GUILayout.Width(120)))
        {
            LoadMenuScene();
        }
    }

    private static void LoadMenuScene()
    {
        if (Application.isPlaying)
        {
            Debug.LogWarning("Cannot load the scene while the game is running.");
            return;
        }

        // Charger la scène "MenuScene"
        if (EditorSceneManager.GetSceneByName("MenuScene") != null)
        {
            EditorSceneManager.OpenScene("Assets/Scenes/MenuScene.unity");
            EditorApplication.isPlaying = true;
        }
        else
        {
            Debug.LogError("Scene 'MenuScene' not found. Make sure it's in the correct path and added to the build settings.");
        }
    }
}
