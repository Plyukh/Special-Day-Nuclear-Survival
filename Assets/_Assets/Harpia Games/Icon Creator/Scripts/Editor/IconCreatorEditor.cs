using UnityEditor;
using UnityEngine;

namespace IconCreator
{
    public class IconCreatorEditor : Editor
    {
        private SerializedProperty useDafaultNameProp;
        private SerializedProperty includeResolutionInFileNameProp;
        private SerializedProperty iconFileNameProp;
        private SerializedProperty useTransparencyProp;
        private SerializedProperty lookAtObjectCenterProp;
        private SerializedProperty dynamicFovProp;
        private SerializedProperty folderNameProp;
        private SerializedProperty modeProp;
        private SerializedProperty cameraControllsrop;
        private SerializedProperty saveLocationProp;
        private SerializedProperty previewProp;
        private SerializedProperty fovOffsetProp;

        private SerializedProperty cameraUpProp;
        private SerializedProperty cameraDownProp;
        private SerializedProperty nextKeyProp;
        private SerializedProperty forceIconProp;



        protected void DrawDefaultIconCreatorInspector(IconCreator script)
        {
            Color oldGUiColor = GUI.color;

            GUILayout.BeginVertical("GroupBox");

            // ----- SELECT MODE ------
            GUILayout.Label("1 - Select mode", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(modeProp, new GUIContent("Mode", "Manual mode: You can set the angle of the object FOV and camera position.\n\nAutomatic mode: All icons are created at once, very fast"));
            if (script.mode == IconCreator.Mode.Manual)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.PropertyField(nextKeyProp, new GUIContent("Next icon key", "Creates an icon and goes to next icon  (manual mode only)"));
                EditorGUILayout.PropertyField(forceIconProp, new GUIContent("Force icon key", "Force creating an icon, useful for icons with animations (manual mode only)"));


                EditorGUILayout.Space(10);
                EditorGUILayout.PropertyField(cameraControllsrop, new GUIContent("Camera moviment?", "Do you want to move you camera?"));
                if (script.cameraMoviment)
                {
                    EditorGUILayout.PropertyField(cameraUpProp, new GUIContent("Camera Up", "Translates the camera in Y direction up"));
                    EditorGUILayout.PropertyField(cameraDownProp, new GUIContent("Camera Down", "Translates the camera in Y direction down"));
                }
                EditorGUILayout.Space(10);
            }

            Debug.Log($"{this.GetType()}",this);
            if (this.GetType() == typeof(MaterialsIconsCreator))
            {
                EditorGUILayout.PropertyField(previewProp, new GUIContent("Preview in edit mode", "(Recommended)\n\nPreview the icon in edit mode.\n\nIcon Camera Creator must be active in game view.\n\n'Dynamic fov' or 'Look at mesh center' must be active"));
            }
            GUILayout.EndVertical();


            // ----- SELECT FILE LOCATION ------
            GUILayout.BeginVertical("GroupBox", GUILayout.ExpandWidth(false));
            EditorGUILayout.LabelField("2 - Files props", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(saveLocationProp, (new GUIContent("Path", "Root folder of the icons")));
            EditorGUILayout.PropertyField(folderNameProp, new GUIContent("Root folder name", "The parent folder of your icons. This folder will be created."));

            EditorGUILayout.PropertyField(useDafaultNameProp, new GUIContent("Use material / prefab name", "Use same name as material / prefab"));

            EditorGUILayout.PropertyField(includeResolutionInFileNameProp, (new GUIContent("Include resolution", "Include resolution into the file name?")));

            if (!script.useDafaultName)
            {

                    EditorGUILayout.PropertyField(iconFileNameProp, new GUIContent("Icon file name", "The base name of the final png File"));
               

                if (string.IsNullOrWhiteSpace(script.iconFileName))
                {
                    EditorGUILayout.HelpBox("File name cannot be null or white spaces", MessageType.Error);
                }

                
            }

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Final location: ");
            EditorGUILayout.LabelField(script.GetFinalFolder().Replace(@"\", @"/") + "/" + script.GetFileName("exempleName", 1));
            GUILayout.EndVertical();

            //

            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.LabelField("3 - Icons properties", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(useTransparencyProp, new GUIContent("Use transparency", "Do you want transparent background in your icons?\n\nRemoves skybox from the icon.6"));
            //if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.GetType().Name == "UniversalRenderPipelineAsset" && script.useTransparency)
            //{
            //    EditorGUILayout.HelpBox("Transparency and post processing is not yet supported", MessageType.Warning);
            //}
            EditorGUILayout.PropertyField(dynamicFovProp, new GUIContent("Dynamic fov ", "Try to find the best FOV value to fit the object whitin the image file."));

            if (script.dynamicFov)
            {
                EditorGUILayout.PropertyField(fovOffsetProp, new GUIContent("Icon padding", "Icon padding"));
            }

            EditorGUILayout.PropertyField(lookAtObjectCenterProp, new GUIContent("Look at prefab mesh center", "Forces the camera look at the object's mesh center, tries to center it on the screen"), script.lookAtObjectCenter);
            GUILayout.EndVertical();

            GUILayout.BeginVertical("GroupBox");
            EditorGUILayout.LabelField("4 - Resolution", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Current resolution " + script.mainCam.pixelWidth + " x " + script.mainCam.pixelHeight);
            EditorGUILayout.LabelField("Choose your desired resolution in Unity's Game window");
            GUILayout.EndVertical();

        }

        protected static void DrawButtons(IconCreator script)
        {
            if (GUILayout.Button("Build icons!", GUILayout.Height(40)))
            {
                if (!script.CheckConditions()) return;

                if (!EditorApplication.isPlaying)
                {
                    Debug.LogError("You need to enter the play mode!");
                    return;
                }

                script.BuildIcons();
            }

            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.Space(15);
                EditorGUILayout.HelpBox("You need to enter the play mode!", MessageType.Warning);
            }
        }

        protected void OnEnable()
        {
            // Fetch the objects from the GameObject script to display in the inspector

            useDafaultNameProp = serializedObject.FindProperty("useDafaultName");
            includeResolutionInFileNameProp = serializedObject.FindProperty("includeResolutionInFileName");
            iconFileNameProp = serializedObject.FindProperty("iconFileName");
            useTransparencyProp = serializedObject.FindProperty("useTransparency");
            lookAtObjectCenterProp = serializedObject.FindProperty("lookAtObjectCenter");
            dynamicFovProp = serializedObject.FindProperty("dynamicFov");
            folderNameProp = serializedObject.FindProperty("folderName");
            modeProp = serializedObject.FindProperty("mode");
            
            saveLocationProp = serializedObject.FindProperty("pathLocation");
            previewProp = serializedObject.FindProperty("preview");
            fovOffsetProp = serializedObject.FindProperty("fovOffset");
            cameraControllsrop = serializedObject.FindProperty("cameraMoviment");

#if ENABLE_INPUT_SYSTEM
            cameraDownProp = serializedObject.FindProperty("cameraGoDown");
            cameraUpProp = serializedObject.FindProperty("cameraGoUp");
            nextKeyProp = serializedObject.FindProperty("nextIconKey");
            forceIconProp = serializedObject.FindProperty("forceIcon");
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            cameraDownProp = serializedObject.FindProperty("cameraGoDownLegacy");
            cameraUpProp = serializedObject.FindProperty("cameraGoUpLegacy");
            nextKeyProp = serializedObject.FindProperty("nextIconKeyLegacy");
            forceIconProp = serializedObject.FindProperty("forceIconLegacy");
#endif



        }
    }
}