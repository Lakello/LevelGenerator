using System;
using UnityEngine;

namespace LevelGenerator.Entity.Structs
{
    [Serializable]
    internal class NoiseSettings
    {
        [SerializeField] private Noise.NormalizeMode _normalizeMode;
        [SerializeField] private float _scale = 50;
        [SerializeField] private int _octaves = 6;
        [SerializeField, Range(0, 1)] private float _persistance = .6f;
        [SerializeField] private float _lacunarity = 2;
        [SerializeField] private int _seed;
        [SerializeField] private Vector2 _offset;

        public Noise.NormalizeMode NormalizeMode => _normalizeMode;
        public float Scale => _scale;
        public int Octaves => _octaves;
        public float Persistance => _persistance;
        public float Lacunarity => _lacunarity;
        public int Seed => _seed;
        public Vector2 Offset => _offset;

        internal void ValidateValues()
        {
            _scale = Mathf.Max(_scale, 0.01f);
            _octaves = Mathf.Max(_octaves, 1);
            _lacunarity = Mathf.Max(_lacunarity, 1);
            _persistance = Mathf.Clamp01(_persistance);
        }
    }
}
