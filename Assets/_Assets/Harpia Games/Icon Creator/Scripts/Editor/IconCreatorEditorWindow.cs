
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace IconCreator
{
    public class IconCreatorEditorWindow : EditorWindow
    {
        private Sample gettingStartedSample;

        [MenuItem("Window/Icon creator")]
        private static void OpenWindow()
        {
            IconCreatorEditorWindow window = (IconCreatorEditorWindow)GetWindow(typeof(IconCreatorEditorWindow));

            window.minSize = new Vector2(340, 560);
            window.maxSize = new Vector2(340, 560);
            window.titleContent = new GUIContent("Icon Creator - Getting started");

            window.Show();
        }

        private void OnGUI()
        {
            DrawLogo();
            SeparatorLine();
            DrawButtons();
            SeparatorLine();
            DrawHelp();

        }

        private void OnEnable()
        {

        }


        private void OnDisable()
        {

        }


        public static void DrawLogo()
        {
            GUIStyle style = new GUIStyle(EditorStyles.largeLabel);
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize *= 2;

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Icon Creator", style, GUILayout.ExpandWidth(true), GUILayout.Height(40));
            GUILayout.EndHorizontal();


        }

        private static void DrawButtons()
        {
            GUILayout.Label("1- Open an icon creator scene: ", EditorStyles.largeLabel);

            GUILayout.Space(10);

            GUILayout.Height(100);

            if (GUILayout.Button("Materials Icon Creator Scene", GUILayout.Height(50)))
            {
                OpenScene("Materials Icon Creator");
                Debug.Log("Materials Icon Creator Scene opened");
                EditorSceneManager.GetActiveScene();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Prefabs Icon Creator Scene", GUILayout.Height(50)))
            {
                OpenScene("Prefabs Icon Creator");
                Debug.Log("Prefabs Icon Creator Scene opened");
            }

            string currentScene = EditorSceneManager.GetActiveScene().name;

            if (currentScene == "Prefabs Icon Creator" || currentScene == "Materials Icon Creator")
            {

                GUILayout.Space(10);

                GUILayout.Label("2- Select the icon creator object: ", EditorStyles.largeLabel);

                if (GUILayout.Button("Select Icon Creator Object", GUILayout.Height(50)))
                {
                    Selection.activeTransform = GameObject.FindObjectOfType<IconCreator>().transform;
                }

                if (Selection.activeTransform == null) return;

                if (Selection.activeTransform.gameObject.name == "Icon creator Camera")
                {
                    GUILayout.Space(10);
                    GUILayout.Label("3 - Now follow instructions on the inspector window");
                }
            }

        }


        private static void DrawHelp()
        {
            GUILayout.Label("Need Help?", EditorStyles.largeLabel);

            GUILayout.Space(10);


            if (GUILayout.Button("Open Documentation", GUILayout.Height(30)))
            {
                Application.OpenURL("https://docs.google.com/document/d/1O7FnBUAFJEZwadJSbIgfp5peQOi2QJfD_77_FMJ_i8g/edit#heading=h.v0kpr5d309c1");
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Support Email", GUILayout.Height(30)))
            {
                Application.OpenURL("mailto:harpiagamesstudio@gmai.com?subject=Icon%20Creator%20Help");
            }

            GUIStyle style = new GUIStyle(EditorStyles.largeLabel);
            style.alignment = TextAnchor.MiddleCenter;


            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Contact: harpiagamesstudio@gmail.com", style, GUILayout.ExpandWidth(true), GUILayout.Height(20));
            GUILayout.EndHorizontal();
            GUILayout.Space(20);

            SeparatorLine();
        }

        private static void SeparatorLine()
        {
            EditorGUILayout.Space();
            GUILayout.Label("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();
        }

        private static void OpenScene(string sceneName)
        {

            string[] a = AssetDatabase.FindAssets(sceneName);

            var path = AssetDatabase.GUIDToAssetPath(a[0]);

            if (a.Length > 1)
            {
                Debug.LogWarning($"(Icon creator) Attention: {a.Length} scenes where founded with name {sceneName}. Opening the first one ");
            }

            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);

        }

        
    }
}