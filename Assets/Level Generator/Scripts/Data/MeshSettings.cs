using System.Collections.Generic;
using UnityEngine;

namespace LevelGenerator.Data
{
    [CreateAssetMenu(menuName = "LevelGenerator/Data/MeshSettings")]
    internal class MeshSettings : UpdatableData
    {
        internal const int NumSupportedLODs = 5;
        internal const int NumSupportedChunkSizes = 9;
        internal const int NumSupportedFlatshadedChunkSizes = 3;

        private readonly int[] _supportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

        [SerializeField] private float _meshScale = 2.5f;
        [SerializeField] private bool _useFlatShading;
        [SerializeField, Range(0, NumSupportedChunkSizes - 1)] private int _chunkSizeIndex;
        [SerializeField, Range(0, NumSupportedFlatshadedChunkSizes - 1)] private int _flatshadedChunkSizeIndex;

        internal float MeshScale => _meshScale;
        internal bool UseFlatShading => _useFlatShading;
        internal int ChunkSizeIndex => _chunkSizeIndex;
        internal int FlatshadedChunkSizeIndex => _flatshadedChunkSizeIndex;
        internal int NumVertsPerLine => _supportedChunkSizes[(_useFlatShading) ? _flatshadedChunkSizeIndex : _chunkSizeIndex] + 5;
        internal float MeshWorldSize => (NumVertsPerLine - 3) * _meshScale;
        internal IReadOnlyCollection<int> SupportedChunkSizes => _supportedChunkSizes;
    }
}
