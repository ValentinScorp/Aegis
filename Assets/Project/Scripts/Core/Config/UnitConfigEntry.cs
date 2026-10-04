[System.Serializable]
public class UnitConfigEntry
{
    public string unit_type;
    public float base_strength;
    public float base_speed;
    public float base_spirit;
    public string main_weapon_primary;
    public string off_weapon_primary;
    public string main_weapon_secondary;
    public string off_weapon_secondary;
}

[System.Serializable]
public class UnitConfigCollection
{
    public UnitConfigEntry[] units;
}