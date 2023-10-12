using UnityEngine;
using System.Collections.Generic;
using LevelGenerator.Data;
using LevelGenerator.Entity;
using LevelGenerator.Entity.Structs;

namespace LevelGenerator
{
    internal class TerrainGenerator : MonoBehaviour
    {
        private const float ViewerMoveThresholdForChunkUpdate = 25f;
        private const float SqrViewerMoveThresholdForChunkUpdate = ViewerMoveThresholdForChunkUpdate * ViewerMoveThresholdForChunkUpdate;

        [SerializeField] private int _colliderLODIndex;
        [SerializeField] private LODInfo[] _detailLevels;

        [SerializeField] private MeshSettings _meshSettings;
        [SerializeField] private HeightMapSettings _heightMapSettings;
        [SerializeField] private TextureData _textureSettings;

        [SerializeField] private Transform _viewer;
        [SerializeField] private Material _mapMaterial;

        private Vector2 _viewerPosition;
        private Vector2 _viewerPositionOld;

        private float _meshWorldSize;
        private int _chunksVisibleInViewDst;

        private Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
        private List<TerrainChunk> _visibleTerrainChunks = new List<TerrainChunk>();

        private void Start()
        {
            _textureSettings.ApplyToMaterial(_mapMaterial);
            _textureSettings.UpdateMeshHeights(_mapMaterial, _heightMapSettings.MinHeight, _heightMapSettings.MaxHeight);

            float maxViewDst = _detailLevels[_detailLevels.Length - 1].VisibleDstThreshold;
            _meshWorldSize = _meshSettings.MeshWorldSize;
            _chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / _meshWorldSize);

            UpdateVisibleChunks();
        }

        private void Update()
        {
            _viewerPosition = new Vector2(_viewer.position.x, _viewer.position.z);

            if (_viewerPosition != _viewerPositionOld)
            {
                foreach (TerrainChunk chunk in _visibleTerrainChunks)
                {
                    chunk.UpdateCollisionMesh();
                }
            }

            if ((_viewerPositionOld - _viewerPosition).sqrMagnitude > SqrViewerMoveThresholdForChunkUpdate)
            {
                _viewerPositionOld = _viewerPosition;
                UpdateVisibleChunks();
            }
        }

        private void UpdateVisibleChunks()
        {
            HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
            for (int i = _visibleTerrainChunks.Count - 1; i >= 0; i--)
            {
                alreadyUpdatedChunkCoords.Add(_visibleTerrainChunks[i].Coord);
                _visibleTerrainChunks[i].UpdateTerrainChunk();
            }

            int currentChunkCoordX = Mathf.RoundToInt(_viewerPosition.x / _meshWorldSize);
            int currentChunkCoordY = Mathf.RoundToInt(_viewerPosition.y / _meshWorldSize);

            for (int yOffset = -_chunksVisibleInViewDst; yOffset <= _chunksVisibleInViewDst; yOffset++)
            {
                for (int xOffset = -_chunksVisibleInViewDst; xOffset <= _chunksVisibleInViewDst; xOffset++)
                {
                    Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                    if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                    {
                        if (_terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                        {
                            _terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                        }
                        else
                        {
                            TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, _heightMapSettings, _meshSettings, _detailLevels, _colliderLODIndex, transform, _viewer, _mapMaterial);
                            _terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
                            newChunk.VisibilityChanged += OnTerrainChunkVisibilityChanged;
                            newChunk.Load();
                        }
                    }
                }
            }
        }

        private void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
        {
            if (isVisible)
            {
                _visibleTerrainChunks.Add(chunk);
            }
            else
            {
                _visibleTerrainChunks.Remove(chunk);
            }
        }
    }
}