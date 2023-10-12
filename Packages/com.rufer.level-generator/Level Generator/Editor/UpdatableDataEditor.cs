using UnityEngine;
using UnityEditor;
using LevelGenerator.Data;

namespace LevelGenerator.UnityTool
{
    [CustomEditor(typeof(UpdatableData), true)]
    public class UpdatableDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (DrawDefaultInspector())
                Update();

            if (GUILayout.Button("Update"))
            {
                Update();
                EditorUtility.SetDirty(target);
            }
        }

        private void Update()
        {
            UpdatableData data = (UpdatableData)target;

            foreach (var updatedCallBack in data.Previews)
                updatedCallBack();
        }
    }
}