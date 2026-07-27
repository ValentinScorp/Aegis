namespace Aegis.Core
{
    public class UnitStats
    {
        public readonly float BaseStrength;
        public readonly float BaseSpeed;
        public readonly float BaseSpirit;

        public UnitStats(float strength, float speed, float spirit)
        {
            BaseStrength = strength;
            BaseSpeed = speed;
            BaseSpirit = spirit;
        }

        public float GetStat(StatType type) => type switch {
            StatType.Strength => BaseStrength,
            StatType.Speed => BaseSpeed,
            StatType.Spirit => BaseSpirit,
            _ => 0f
        };
    }
}