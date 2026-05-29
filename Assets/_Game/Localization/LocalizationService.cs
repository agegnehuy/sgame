using System;
using System.Collections.Generic;
using UnityEngine;

namespace SGame.Localization
{
    [Serializable]
    internal class LocalizationEntry
    {
        public string key;
        public string value;
    }

    [Serializable]
    internal class LocalizationFile
    {
        public string language;
        public List<LocalizationEntry> entries;
    }

    public static class LocalizationService
    {
        private static Dictionary<string, string> _entries = new();
        public static string CurrentLanguage { get; private set; } = "en";
        public static event Action<string> LanguageChanged;

        public static void LoadFromJson(TextAsset jsonAsset)
        {
            if (jsonAsset == null)
            {
                Debug.LogWarning("Localization JSON asset missing.");
                return;
            }

            var parsed = JsonUtility.FromJson<LocalizationFile>(jsonAsset.text);
            if (parsed == null)
            {
                Debug.LogWarning("Failed to parse localization file.");
                return;
            }

            CurrentLanguage = parsed.language ?? "en";
            _entries = new Dictionary<string, string>();
            if (parsed.entries == null)
            {
                return;
            }

            foreach (var entry in parsed.entries)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.key))
                {
                    continue;
                }

                _entries[entry.key] = entry.value ?? string.Empty;
            }

            LanguageChanged?.Invoke(CurrentLanguage);
        }

        public static string Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }

            return _entries.TryGetValue(key, out var value) ? value : key;
        }
    }
}
