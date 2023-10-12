using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace LevelGenerator.Data
{
    [CreateAssetMenu(menuName = "LevelGenerator/Data/TextureData")]
    internal class TextureData : UpdatableData
    {
        private const int TextureSize = 512;
        private const TextureFormat FormatTexture = TextureFormat.RGB565;

        [SerializeField] private Layer[] _layers;
        
        private float _savedMinHeight;
        private float _savedMaxHeight;

        internal void ApplyToMaterial(Material material)
        {
            material.SetInt("layerCount", _layers.Length);
            material.SetColorArray("baseColours", _layers.Select(x => x.Tint).ToArray());
            material.SetFloatArray("baseStartHeights", _layers.Select(x => x.StartHeight).ToArray());
            material.SetFloatArray("baseBlends", _layers.Select(x => x.BlendStrength).ToArray());
            material.SetFloatArray("baseColourStrength", _layers.Select(x => x.TintStrength).ToArray());
            material.SetFloatArray("baseTextureScales", _layers.Select(x => x.TextureScale).ToArray());

            Texture2DArray texturesArray = GenerateTextureArray(_layers.Select(x => x.Texture).ToArray());
            material.SetTexture("baseTextures", texturesArray);

            UpdateMeshHeights(material, _savedMinHeight, _savedMaxHeight);
        }

        internal void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
        {
            _savedMinHeight = minHeight;
            _savedMaxHeight = maxHeight;

            material.SetFloat("minHeight", minHeight);
            material.SetFloat("maxHeight", maxHeight);
        }

        private Texture2DArray GenerateTextureArray(Texture2D[] textures)
        {
            Texture2DArray textureArray = new Texture2DArray(TextureSize, TextureSize, textures.Length, FormatTexture, true);
            for (int i = 0; i < textures.Length; i++)
            {
                textureArray.SetPixels(textures[i].GetPixels(), i);
            }
            textureArray.Apply();
            return textureArray;
        }

        [System.Serializable]
        internal class Layer
        {
            public Texture2D Texture;
            public Color Tint;
            [Range(0, 1)]
            public float TintStrength;
            [Range(0, 1)]
            public float StartHeight;
            [Range(0, 1)]
            public float BlendStrength;
            public float TextureScale;
        }
    }
}