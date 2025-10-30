using UnityEditor;
using UnityEngine;

namespace IconCreator
{
    [CustomEditor(typeof(PrefabIconCreator))]
    public class PrefabIconCreatorEditor : IconCreatorEditor
    {
        private SerializedProperty itemPositionProp;
        private SerializedProperty itensToShotProp;

        public override void OnInspectorGUI()
        {
            PrefabIconCreator script = (PrefabIconCreator)target;

            DrawDefaultIconCreatorInspector(script);

            GUILayout.BeginVertical("GroupBox");

            EditorGUILayout.LabelField("5 - Select the prefabs", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(itensToShotProp, new GUIContent("Prefabs", "Prefab list to take pictures"));
            GUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

            DrawButtons(script);
        }

        private new void OnEnable()
        {
            base.OnEnable();
            // Fetch the objects from the GameObject script to display in the inspector
            itemPositionProp = serializedObject.FindProperty("itemPosition");
            itensToShotProp = serializedObject.FindProperty("itensToShot");
        }
    }
}