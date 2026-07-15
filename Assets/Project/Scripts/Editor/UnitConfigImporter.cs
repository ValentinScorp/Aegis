#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Aegis.Core
{
    public static class UnitConfigImporter
    {
        private const string JsonPath = "Assets/Content/Configs/unit_configs.json";
        private const string SOFolder = "Assets/Project/Configs/";

        [MenuItem("Aegis/Import Unit Configs From JSON")]
        public static void ImportFromJson()
        {
            if (!File.Exists(JsonPath)) {
                Debug.LogError($"JSON not found at {JsonPath}");
                return;
            }

            if (!AssetDatabase.IsValidFolder(SOFolder)) {
                Directory.CreateDirectory(SOFolder);
                AssetDatabase.Refresh();
            }

            string json = File.ReadAllText(JsonPath);
            UnitConfigCollection data = JsonUtility.FromJson<UnitConfigCollection>(json);

            int created = 0, updated = 0;

            foreach (var entry in data.units) {
                if (!System.Enum.TryParse(entry.unit_type, out EntityType parsedType)) {
                    Debug.LogWarning($"Unknown EntityType '{entry.unit_type}', skipped.");
                    continue;
                }

                string assetPath = $"{SOFolder}{entry.unit_type}.asset";
                UnitConfig config = AssetDatabase.LoadAssetAtPath<UnitConfig>(assetPath);

                bool isNew = config == null;
                if (isNew) {
                    config = ScriptableObject.CreateInstance<UnitConfig>();
                    AssetDatabase.CreateAsset(config, assetPath);
                    created++;
                } else {
                    updated++;
                }

                config.UnitType = parsedType;
                config.MaxHealth = entry.max_health;
                config.AttackTime = entry.attack_time;
                config.AttackDamage = entry.attack_damage;
                config.AttackRadius = entry.attack_distance;
                config.CanShoot = entry.can_shoot;
                config.ShootTime = entry.shoot_time;
                config.ShootRadius = entry.shoot_distance;
                config.AttackEventTime = entry.attack_event_time;
                config.ShootEventTime = entry.shoot_event_time;

                EditorUtility.SetDirty(config);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Import done: {created} created, {updated} updated.");
        }
    }
}
#endif