#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace Aegis.Core
{
    public static class WeaponConfigImporter
    {
        private const string JsonPath = "Assets/Content/Configs/weapon_configs.json";
        private const string SOFolder = "Assets/Project/Configs/Weapons/";

        [MenuItem("Aegis/Import Weapon Configs From JSON")]
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
            WeaponConfigCollection data = JsonConvert.DeserializeObject<WeaponConfigCollection>(json);

            int created = 0, updated = 0;

            foreach (var entry in data.items) {
                if (!TryParseWeaponType(entry.weapon_type, out WeaponType parsedType)) {
                    Debug.LogWarning($"Unknown WeaponType '{entry.weapon_type}' for '{entry.id}', skipped.");
                    continue;
                }

                string assetPath = $"{SOFolder}{entry.id}.asset";
                WeaponConfig config = AssetDatabase.LoadAssetAtPath<WeaponConfig>(assetPath);

                bool isNew = config == null;
                if (isNew) {
                    config = ScriptableObject.CreateInstance<WeaponConfig>();
                    AssetDatabase.CreateAsset(config, assetPath);
                    created++;
                } else {
                    updated++;
                }

                config.Id = entry.id;
                config.DisplayName = entry.display_name;
                config.Animation = entry.animation;
                config.WeaponType = parsedType;
                config.ProjectileId = entry.projectile;
                config.Damage = entry.damage;
                config.AttackRange = entry.attack_range;
                config.Weight = entry.weight;
                config.ArmorValue = entry.armor_value;
                config.DeflectChance = entry.deflect_chance;
                config.EffectIds = entry.effects ?? System.Array.Empty<string>();

                EditorUtility.SetDirty(config);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Import done: {created} created, {updated} updated.");
        }

        // "one_hand_sword" -> "OneHandSword", бо Enum.TryParse не розбирає snake_case сам
        private static bool TryParseWeaponType(string snakeCase, out WeaponType result)
        {
            string pascalCase = string.Concat(
                snakeCase.Split('_').Select(w => char.ToUpper(w[0]) + w.Substring(1))
            );
            return System.Enum.TryParse(pascalCase, out result);
        }
    }
}
#endif