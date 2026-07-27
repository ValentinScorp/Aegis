#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace Aegis.Core
{
    public static class UnitConfigImporter
    {
        private const string JsonPath = "Assets/Content/Configs/unit_configs.json";
        private const string SOFolder = "Assets/Project/Configs/Units/";
        private const string WeaponSOFolder = "Assets/Project/Configs/Weapons/";

        [MenuItem("Aegis/Import Unit Configs From JSON")]
        public static void ImportFromJson()
        {
            if (!File.Exists(JsonPath)) {
                Debug.LogError($"JSON not found at {JsonPath}");
                return;
            }

            string json = File.ReadAllText(JsonPath);
            var data = JsonConvert.DeserializeObject<UnitConfigCollection>(json);

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
                config.BaseStrength = entry.base_strength;
                config.BaseSpeed = entry.base_speed;
                config.BaseSpirit = entry.base_spirit;

                config.MainHandPrimary = ResolveWeapon(entry.main_hand_primary);
                config.OffHandPrimary = ResolveWeapon(entry.off_hand_primary);
                config.MainHandSecondary = ResolveWeapon(entry.main_hand_secondary);
                config.OffHandSecondary = ResolveWeapon(entry.off_hand_secondary);

                EditorUtility.SetDirty(config);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Import done: {created} created, {updated} updated.");
        }

        private static WeaponConfig ResolveWeapon(string id)
        {
            if (string.IsNullOrEmpty(id) || id == "none") return null;

            string path = $"{WeaponSOFolder}{id}.asset";
            var weapon = AssetDatabase.LoadAssetAtPath<WeaponConfig>(path);
            if (weapon == null)
                Debug.LogWarning($"WeaponConfig '{id}' not found at {path} — did you run 'Import Weapon Configs' first?");
            return weapon;
        }
    }
}
#endif