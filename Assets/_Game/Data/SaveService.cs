using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SGame.Data
{
    [Serializable]
    public class ProfileRecord
    {
        public string profileId;
        public string displayName;
        public string avatarId;
    }

    [Serializable]
    public class ProfileCollection
    {
        public List<ProfileRecord> profiles = new();
    }

    [Serializable]
    public class MissionResultRecord
    {
        public string missionId;
        public int bestScore;
        public int stars;
        public int attempts;
    }

    [Serializable]
    public class ProfileProgress
    {
        public string profileId;
        public int coins;
        public List<MissionResultRecord> missions = new();
        public List<string> ownedItems = new();
        public string equippedClothesId;
        public string equippedShoesId;
        public string equippedAccessoryId;
    }

    public static class SaveService
    {
        public static event Action<string> ProgressChanged;

        private static string ProfilesPath => Path.Combine(Application.persistentDataPath, "profiles.json");

        private static string GetProgressPath(string profileId)
        {
            return Path.Combine(Application.persistentDataPath, $"progress_{profileId}.json");
        }

        public static string GetActiveProfileId()
        {
            var profiles = LoadProfiles();
            if (profiles.profiles.Count == 0)
            {
                CreateProfile("p1", "P1", "boy_01");
                profiles = LoadProfiles();
            }

            return profiles.profiles[0].profileId;
        }

        public static ProfileCollection LoadProfiles()
        {
            if (!File.Exists(ProfilesPath))
            {
                return new ProfileCollection();
            }

            var json = File.ReadAllText(ProfilesPath);
            var collection = JsonUtility.FromJson<ProfileCollection>(json);
            return collection ?? new ProfileCollection();
        }

        public static void CreateProfile(string profileId, string displayName, string avatarId)
        {
            var collection = LoadProfiles();
            var existing = collection.profiles.Find(p => p.profileId == profileId);
            if (existing != null)
            {
                return;
            }

            collection.profiles.Add(new ProfileRecord
            {
                profileId = profileId,
                displayName = displayName,
                avatarId = avatarId
            });

            AtomicWrite(ProfilesPath, JsonUtility.ToJson(collection, true));
        }

        public static ProfileProgress LoadProgress(string profileId)
        {
            var path = GetProgressPath(profileId);
            if (!File.Exists(path))
            {
                return new ProfileProgress { profileId = profileId, coins = 0 };
            }

            var json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<ProfileProgress>(json);
            return data ?? new ProfileProgress { profileId = profileId, coins = 0 };
        }

        public static void SaveMissionResult(string profileId, string missionId, int score, int stars)
        {
            var progress = LoadProgress(profileId);
            var mission = progress.missions.Find(m => m.missionId == missionId);
            if (mission == null)
            {
                mission = new MissionResultRecord
                {
                    missionId = missionId,
                    bestScore = score,
                    stars = stars,
                    attempts = 1
                };
                progress.missions.Add(mission);
            }
            else
            {
                mission.attempts += 1;
                mission.bestScore = Math.Max(mission.bestScore, score);
                mission.stars = Math.Max(mission.stars, stars);
            }

            var json = JsonUtility.ToJson(progress, true);
            AtomicWrite(GetProgressPath(profileId), json);
            NotifyProgressChanged(profileId);
        }

        public static int AddCoins(string profileId, int amount)
        {
            var progress = LoadProgress(profileId);
            progress.coins = Math.Max(0, progress.coins + Math.Max(0, amount));
            AtomicWrite(GetProgressPath(profileId), JsonUtility.ToJson(progress, true));
            NotifyProgressChanged(profileId);
            return progress.coins;
        }

        public static bool TrySpendCoins(string profileId, int amount, out int remainingCoins)
        {
            var normalized = Math.Max(0, amount);
            var progress = LoadProgress(profileId);
            if (progress.coins < normalized)
            {
                remainingCoins = progress.coins;
                return false;
            }

            progress.coins -= normalized;
            AtomicWrite(GetProgressPath(profileId), JsonUtility.ToJson(progress, true));
            NotifyProgressChanged(profileId);
            remainingCoins = progress.coins;
            return true;
        }

        public static int GetCoins(string profileId)
        {
            var progress = LoadProgress(profileId);
            return Math.Max(0, progress.coins);
        }

        public static bool OwnsItem(string profileId, string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return false;
            }

            var progress = LoadProgress(profileId);
            return progress.ownedItems.Contains(itemId);
        }

        public static bool PurchaseItem(string profileId, string itemId, int cost, out int remainingCoins)
        {
            remainingCoins = GetCoins(profileId);
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return false;
            }

            var progress = LoadProgress(profileId);
            if (!progress.ownedItems.Contains(itemId))
            {
                if (progress.coins < Math.Max(0, cost))
                {
                    remainingCoins = progress.coins;
                    return false;
                }

                progress.coins -= Math.Max(0, cost);
                progress.ownedItems.Add(itemId);
                AtomicWrite(GetProgressPath(profileId), JsonUtility.ToJson(progress, true));
                NotifyProgressChanged(profileId);
            }

            remainingCoins = progress.coins;
            return true;
        }

        public static bool EquipItem(string profileId, string itemId, string slot)
        {
            if (string.IsNullOrWhiteSpace(itemId) || string.IsNullOrWhiteSpace(slot))
            {
                return false;
            }

            var progress = LoadProgress(profileId);
            if (!progress.ownedItems.Contains(itemId))
            {
                return false;
            }

            switch (slot.ToLowerInvariant())
            {
                case "clothes":
                    progress.equippedClothesId = itemId;
                    break;
                case "shoes":
                    progress.equippedShoesId = itemId;
                    break;
                case "accessory":
                    progress.equippedAccessoryId = itemId;
                    break;
                default:
                    return false;
            }

            AtomicWrite(GetProgressPath(profileId), JsonUtility.ToJson(progress, true));
            NotifyProgressChanged(profileId);
            return true;
        }

        public static ProfileRecord GetProfile(string profileId)
        {
            var collection = LoadProfiles();
            return collection.profiles.Find(p => p.profileId == profileId);
        }

        public static string GetEquippedItemId(string profileId, string slot)
        {
            var progress = LoadProgress(profileId);
            return slot.ToLowerInvariant() switch
            {
                "clothes" => progress.equippedClothesId,
                "shoes" => progress.equippedShoesId,
                "accessory" => progress.equippedAccessoryId,
                _ => string.Empty
            };
        }

        public static ProfileProgress GetProgressSnapshot(string profileId)
        {
            return LoadProgress(profileId);
        }

        public static MissionResultRecord GetMissionResult(string profileId, string missionId)
        {
            var progress = LoadProgress(profileId);
            return progress.missions.Find(m => m.missionId == missionId);
        }

        public static void ResetProfileProgress(string profileId)
        {
            if (string.IsNullOrWhiteSpace(profileId))
            {
                return;
            }

            var fresh = new ProfileProgress
            {
                profileId = profileId,
                coins = 0
            };

            AtomicWrite(GetProgressPath(profileId), JsonUtility.ToJson(fresh, true));
            NotifyProgressChanged(profileId);
        }

        private static void AtomicWrite(string targetPath, string content)
        {
            var tempPath = targetPath + ".tmp";
            File.WriteAllText(tempPath, content);
            if (File.Exists(targetPath)) File.Delete(targetPath);
            File.Move(tempPath, targetPath);
        }

        private static void NotifyProgressChanged(string profileId)
        {
            ProgressChanged?.Invoke(profileId);
        }
    }
}
