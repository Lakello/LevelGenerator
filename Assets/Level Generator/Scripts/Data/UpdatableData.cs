using UnityEngine;

namespace LevelGenerator.Data
{
    internal class UpdatableData : ScriptableObject
    {
        internal event System.Action OnValuesUpdated;
        internal bool autoUpdate;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (autoUpdate)
            {
                UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
            }
        }

        internal void NotifyOfUpdatedValues()
        {
            UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
            if (OnValuesUpdated != null)
            {
                OnValuesUpdated();
            }
        }
#endif
    }
}