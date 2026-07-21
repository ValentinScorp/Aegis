[System.Serializable]
public class UnitConfigEntry
{
    public string unit_type;
    public int max_health;
    public int attack_damage;
    public float attack_time;
    public float attack_distance;
    public float attack_event_time;
    public bool can_shoot;
    public int shoot_damage;
    public float shoot_time;
    public float shoot_distance;
    public float shoot_event_time;
    public float search_radius;
    public float chase_radius;
    public float move_speed;
    public float walk_animation_speed_multiplier;
}

[System.Serializable]
public class UnitConfigCollection
{
    public UnitConfigEntry[] units;
}