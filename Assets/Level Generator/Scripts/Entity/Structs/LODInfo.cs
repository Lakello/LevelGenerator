using LevelGenerator.Data;
using System;
using UnityEngine;

namespace LevelGenerator.Entity.Structs
{
    [Serializable]
    internal class LODInfo
    {
        [SerializeField, Range(0, MeshSettings.NumSupportedLODs - 1)] private int _lod;
        [SerializeField] private float _visibleDstThreshold;

        internal int Lod => _lod;
        internal float VisibleDstThreshold => _visibleDstThreshold;
        internal float SqrVisibleDstThreshold => _visibleDstThreshold * _visibleDstThreshold;
    }
}
