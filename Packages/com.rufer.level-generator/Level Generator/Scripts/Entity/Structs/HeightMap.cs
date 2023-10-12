namespace LevelGenerator.Entity.Structs
{
    internal struct HeightMap
    {
        internal readonly float[,] Values;
        internal readonly float MinValue;
        internal readonly float MaxValue;

        internal HeightMap(float[,] values, float minValue, float maxValue)
        {
            Values = values;
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }
}
