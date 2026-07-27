#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Aegis.Core
{
    public static class EffectConfigImporter
    {
        private const string JsonPath = "Assets/Content/Configs/effect_configs.json";
        private const string SOFolder = "Assets/Project/Configs/Effects/";

        [MenuItem("Aegis/Import Effect Configs From JSON")]
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
            EffectConfigCollection data = JsonConvert.DeserializeObject<EffectConfigCollection>(json);

            int created = 0, updated = 0;

            foreach (var entry in data.effects) {
                if (!TryParsePascal(entry.action, out EffectAction parsedAction)) {
                    Debug.LogWarning($"Unknown EffectAction '{entry.action}' for '{entry.id}', skipped.");
                    continue;
                }
                if (!System.Enum.TryParse(entry.target, true, out EffectTarget parsedTarget)) {
                    Debug.LogWarning($"Unknown EffectTarget '{entry.target}' for '{entry.id}', skipped.");
                    continue;
                }
                if (!System.Enum.TryParse(entry.nature, true, out EffectNature parsedNature)) {
                    Debug.LogWarning($"Unknown EffectNature '{entry.nature}' for '{entry.id}', skipped.");
                    continue;
                }

                string assetPath = $"{SOFolder}{entry.id}.asset";
                EffectConfig config = AssetDatabase.LoadAssetAtPath<EffectConfig>(assetPath);

                bool isNew = config == null;
                if (isNew) {
                    config = ScriptableObject.CreateInstance<EffectConfig>();
                    AssetDatabase.CreateAsset(config, assetPath);
                    created++;
                } else {
                    updated++;
                }

                config.Id = entry.id;
                config.Action = parsedAction;
                config.Target = parsedTarget;
                config.Nature = parsedNature;
                config.Description = entry.description;
                config.Value = entry.value;
                config.Duration = entry.duration;

                EditorUtility.SetDirty(config);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Import done: {created} created, {updated} updated.");
        }

        private static bool TryParsePascal(string snakeCase, out EffectAction result)
        {
            string pascalCase = string.Concat(
                snakeCase.Split('_').Select(w => char.ToUpper(w[0]) + w.Substring(1))
            );
            return System.Enum.TryParse(pascalCase, out result);
        }
    }
}
#endif