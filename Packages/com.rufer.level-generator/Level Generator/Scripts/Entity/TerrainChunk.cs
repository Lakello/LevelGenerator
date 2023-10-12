using LevelGenerator.Data;
using LevelGenerator.Entity.Structs;
using UnityEngine;

namespace LevelGenerator.Entity
{
    internal class TerrainChunk
    {
        private const float ColliderGenerationDistanceThreshold = 5;

        private Vector2 _coord;

        private GameObject _meshObject;
        private Vector2 _sampleCentre;
        private Bounds _bounds;

        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;

        private LODInfo[] _detailLevels;
        private LODMesh[] _lodMeshes;
        private int _colliderLODIndex;

        private HeightMap _heightMap;
        private bool _heightMapReceived;
        private int _previousLODIndex = -1;
        private bool _hasSetCollider;
        private float _maxViewDst;

        private HeightMapSettings _heightMapSettings;
        private MeshSettings _meshSettings;
        private Transform _viewer;

        internal Vector2 Coord => _coord;

        private bool IsVisible => _meshObject.activeSelf;
        private Vector2 ViewerPosition => new Vector2(_viewer.position.x, _viewer.position.z);

        internal event System.Action<TerrainChunk, bool> VisibilityChanged;

        internal TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material material)
        {
            _coord = coord;
            _detailLevels = detailLevels;
            _colliderLODIndex = colliderLODIndex;
            _heightMapSettings = heightMapSettings;
            _meshSettings = meshSettings;
            _viewer = viewer;

            _sampleCentre = coord * meshSettings.MeshWorldSize / meshSettings.MeshScale;
            Vector2 position = coord * meshSettings.MeshWorldSize;
            _bounds = new Bounds(position, Vector2.one * meshSettings.MeshWorldSize);


            _meshObject = new GameObject("Terrain Chunk");
            _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
            _meshFilter = _meshObject.AddComponent<MeshFilter>();
            _meshCollider = _meshObject.AddComponent<MeshCollider>();
            _meshRenderer.material = material;

            _meshObject.transform.position = new Vector3(position.x, 0, position.y);
            _meshObject.transform.parent = parent;
            SetVisible(false);

            _lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++)
            {
                _lodMeshes[i] = new LODMesh(detailLevels[i].Lod);
                _lodMeshes[i].UpdateCallback += UpdateTerrainChunk;
                if (i == colliderLODIndex)
                {
                    _lodMeshes[i].UpdateCallback += UpdateCollisionMesh;
                }
            }

            _maxViewDst = detailLevels[detailLevels.Length - 1].VisibleDstThreshold;

        }

        internal void Load() =>
            ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(_meshSettings.NumVertsPerLine, _meshSettings.NumVertsPerLine, _heightMapSettings, _sampleCentre), OnHeightMapReceived);

        internal void UpdateTerrainChunk()
        {
            if (_heightMapReceived)
            {
                float viewerDstFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(ViewerPosition));

                bool wasVisible = IsVisible;
                bool visible = viewerDstFromNearestEdge <= _maxViewDst;

                if (visible)
                {
                    int lodIndex = 0;

                    for (int i = 0; i < _detailLevels.Length - 1; i++)
                    {
                        if (viewerDstFromNearestEdge > _detailLevels[i].VisibleDstThreshold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (lodIndex != _previousLODIndex)
                    {
                        LODMesh lodMesh = _lodMeshes[lodIndex];
                        if (lodMesh.HasMesh)
                        {
                            _previousLODIndex = lodIndex;
                            _meshFilter.mesh = lodMesh.Mesh;
                        }
                        else if (!lodMesh.HasRequestedMesh)
                        {
                            lodMesh.RequestMesh(_heightMap, _meshSettings);
                        }
                    }


                }

                if (wasVisible != visible)
                {

                    SetVisible(visible);
                    if (VisibilityChanged != null)
                    {
                        VisibilityChanged(this, visible);
                    }
                }
            }
        }
        
        internal void UpdateCollisionMesh()
        {
            if (!_hasSetCollider)
            {
                float sqrDstFromViewerToEdge = _bounds.SqrDistance(ViewerPosition);

                if (sqrDstFromViewerToEdge < _detailLevels[_colliderLODIndex].SqrVisibleDstThreshold)
                {
                    if (!_lodMeshes[_colliderLODIndex].HasRequestedMesh)
                    {
                        _lodMeshes[_colliderLODIndex].RequestMesh(_heightMap, _meshSettings);
                    }
                }

                if (sqrDstFromViewerToEdge < ColliderGenerationDistanceThreshold * ColliderGenerationDistanceThreshold)
                {
                    if (_lodMeshes[_colliderLODIndex].HasMesh)
                    {
                        _meshCollider.sharedMesh = _lodMeshes[_colliderLODIndex].Mesh;
                        _hasSetCollider = true;
                    }
                }
            }
        }
        
        private void SetVisible(bool visible) =>
            _meshObject.SetActive(visible);

        private void OnHeightMapReceived(object heightMapObject)
        {
            _heightMap = (HeightMap)heightMapObject;
            _heightMapReceived = true;

            UpdateTerrainChunk();
        }
    }

    internal class LODMesh
    {
        private Mesh _mesh;
        private bool _hasRequestedMesh;
        private bool _hasMesh;

        private int _lod;

        internal Mesh Mesh => _mesh;
        internal bool HasRequestedMesh => _hasRequestedMesh;
        internal bool HasMesh => _hasMesh;

        internal event System.Action UpdateCallback;

        internal LODMesh(int lod) =>
            _lod = lod;

        internal void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
        {
            _hasRequestedMesh = true;
            ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.Values, meshSettings, _lod), OnMeshDataReceived);
        }

        private void OnMeshDataReceived(object meshDataObject)
        {
            _mesh = ((MeshData)meshDataObject).CreateMesh();
            _hasMesh = true;

            UpdateCallback();
        }
    }
}