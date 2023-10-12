using System;
using System.Collections.Generic;
using UnityEngine;

namespace LevelGenerator.Data
{
    public class UpdatableData : ScriptableObject
    {
#if UNITY_EDITOR
        private HashSet<Action> _previews = new();

        public IReadOnlyCollection<Action> Previews => _previews;

        public void AddMapPreview(Action valuesUpdatedCallBack) =>
            _previews.Add(valuesUpdatedCallBack);
#endif
    }
}