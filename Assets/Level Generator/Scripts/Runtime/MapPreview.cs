using UnityEngine;
using LevelGenerator.Data;
using LevelGenerator.Entity;
using LevelGenerator.Entity.Structs;

namespace LevelGenerator
{
    internal class MapPreview : MonoBehaviour
    {
        [SerializeField] private Renderer _textureRender;
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;

        [SerializeField] private DrawMode _drawMode;

        [SerializeField] private MeshSettings _meshSettings;
        [SerializeField] private HeightMapSettings _heightMapSettings;
        [SerializeField] private TextureData _textureData;

        [SerializeField] private Material _terrainMaterial;

        [SerializeField, Range(0, MeshSettings.NumSupportedLODs - 1)] private int _editorPreviewLOD;
        [SerializeField] private bool _autoUpdate;
        
        public bool AutoUpdate => _autoUpdate;

        internal enum DrawMode { NoiseMap, Mesh, FalloffMap };

        internal void DrawMapInEditor()
        {
            _textureData.ApplyToMaterial(_terrainMaterial);
            _textureData.UpdateMeshHeights(_terrainMaterial, _heightMapSettings.MinHeight, _heightMapSettings.MaxHeight);
            HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(_meshSettings.NumVertsPerLine, _meshSettings.NumVertsPerLine, _heightMapSettings, Vector2.zero);

            if (_drawMode == DrawMode.NoiseMap)
            {
                DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
            }
            else if (_drawMode == DrawMode.Mesh)
            {
                DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.Values, _meshSettings, _editorPreviewLOD));
            }
            else if (_drawMode == DrawMode.FalloffMap)
            {
                DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(_meshSettings.NumVertsPerLine), 0, 1)));
            }
        }

        internal void DrawTexture(Texture2D texture)
        {
            _textureRender.sharedMaterial.mainTexture = texture;
            _textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;

            _textureRender.gameObject.SetActive(true);
            _meshFilter.gameObject.SetActive(false);
        }

        internal void DrawMesh(MeshData meshData)
        {
            _meshFilter.sharedMesh = meshData.CreateMesh();

            _textureRender.gameObject.SetActive(false);
            _meshFilter.gameObject.SetActive(true);
        }

        private void OnValuesUpdated()
        {
            if (!Application.isPlaying)
            {
                DrawMapInEditor();
            }
        }

        private void OnTextureValuesUpdated() =>
            _textureData.ApplyToMaterial(_terrainMaterial);

        private void OnValidate()
        {
            if (_meshSettings != null)
            {
                _meshSettings.OnValuesUpdated -= OnValuesUpdated;
                _meshSettings.OnValuesUpdated += OnValuesUpdated;
            }
            if (_heightMapSettings != null)
            {
                _heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
                _heightMapSettings.OnValuesUpdated += OnValuesUpdated;
            }
            if (_textureData != null)
            {
                _textureData.OnValuesUpdated -= OnTextureValuesUpdated;
                _textureData.OnValuesUpdated += OnTextureValuesUpdated;
            }
        }
    }
}