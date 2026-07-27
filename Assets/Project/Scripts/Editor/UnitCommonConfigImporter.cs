#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace Aegis.Core
{
    public static class UnitCommonConfigImporter
    {
        private const string JsonPath = "Assets/Content/Configs/unit_common_configs.json";
        private const string AssetPath = "Assets/Project/Configs/UnitCommonConfig.asset";

        [MenuItem("Aegis/Import Unit Common Config From JSON")]
        public static void ImportFromJson()
        {
            if (!File.Exists(JsonPath)) {
                Debug.LogError($"JSON not found at {JsonPath}");
                return;
            }

            string json = File.ReadAllText(JsonPath);
            var entry = JsonConvert.DeserializeObject<UnitCommonConfigEntry>(json);

            UnitCommonConfig config = AssetDatabase.LoadAssetAtPath<UnitCommonConfig>(AssetPath);
            bool isNew = config == null;
            if (isNew) {
                config = ScriptableObject.CreateInstance<UnitCommonConfig>();
                AssetDatabase.CreateAsset(config, AssetPath);
            }

            config.UnarmedDamage = entry.unarmed_damage;
            config.UnarmedCooldown = entry.unarmed_cooldown;
            config.BaseHealth = entry.base_health;
            config.HealthPerStrength = entry.health_per_strength;
            config.SearchRadius = entry.search_radius;
            config.ChaseRadius = entry.chase_radius;
            config.MoveSpeed = entry.move_speed;
            config.WalkAnimationSpeedMultiplier = entry.walk_animation_speed_multiplier;

            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log(isNew ? "UnitCommonConfig created." : "UnitCommonConfig updated.");
        }
    }
}
#endif