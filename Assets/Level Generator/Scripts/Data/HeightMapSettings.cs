using UnityEngine;
using LevelGenerator.Entity.Structs;

namespace LevelGenerator.Data
{
    [CreateAssetMenu(menuName = "LevelGenerator/Data/HeightMapSettings")]
    internal class HeightMapSettings : UpdatableData
    {
        [SerializeField] private NoiseSettings _noiseSettings;
        [SerializeField] private bool _useFalloff;
        [SerializeField] private float _heightMultiplier;
        [SerializeField] private AnimationCurve _heightCurve;

        internal NoiseSettings NoiseSettings => _noiseSettings;
        internal bool UseFalloff => _useFalloff;
        internal float HeightMultiplier => _heightMultiplier;
        internal AnimationCurve HeightCurve => _heightCurve ;
        internal float MinHeight => _heightMultiplier * _heightCurve.Evaluate(0);
        internal float MaxHeight => _heightMultiplier * _heightCurve.Evaluate(1);

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            _noiseSettings.ValidateValues();
            base.OnValidate();
        }
#endif
    }
}
