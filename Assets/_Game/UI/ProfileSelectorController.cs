using System.Collections.Generic;
using SGame.Data;
using UnityEngine;
using UnityEngine.UI;

namespace SGame.UI
{
    public class ProfileSelectorController : MonoBehaviour
    {
        [SerializeField] private Dropdown profileDropdown;
        [SerializeField] private InputField newProfileNameInput;
        [SerializeField] private Button createProfileButton;
        [SerializeField] private CoinBalanceLabel coinBalanceLabel;
        [SerializeField] private AvatarLoadoutPreviewController avatarLoadoutPreview;
        [SerializeField] private MissionProgressSummaryController progressSummaryController;
        [SerializeField] private string defaultAvatarId = "boy_01";
        [SerializeField] private int maxProfiles = 3;

        private readonly List<ProfileRecord> _profiles = new();

        private void Awake()
        {
            if (createProfileButton != null)
            {
                createProfileButton.onClick.AddListener(CreateProfile);
            }

            if (profileDropdown != null)
            {
                profileDropdown.onValueChanged.AddListener(OnProfileChanged);
            }
        }

        private void Start()
        {
            EnsureMinimumProfile();
            ReloadProfiles();
        }

        private void EnsureMinimumProfile()
        {
            var profiles = SaveService.LoadProfiles();
            if (profiles.profiles.Count == 0)
            {
                SaveService.CreateProfile("p1", "P1", defaultAvatarId);
            }
        }

        private void ReloadProfiles()
        {
            _profiles.Clear();
            _profiles.AddRange(SaveService.LoadProfiles().profiles);

            if (profileDropdown == null)
            {
                return;
            }

            profileDropdown.ClearOptions();
            var options = new List<string>();
            foreach (var profile in _profiles)
            {
                options.Add(profile.displayName);
            }

            profileDropdown.AddOptions(options);
            profileDropdown.value = 0;
            profileDropdown.RefreshShownValue();
            if (_profiles.Count > 0)
            {
                PlayerSession.SetActiveProfileId(_profiles[0].profileId);
                coinBalanceLabel?.Refresh();
                avatarLoadoutPreview?.Refresh();
                progressSummaryController?.Refresh();
            }
        }

        private void CreateProfile()
        {
            if (_profiles.Count >= maxProfiles)
            {
                return;
            }

            var name = newProfileNameInput != null && !string.IsNullOrWhiteSpace(newProfileNameInput.text)
                ? newProfileNameInput.text.Trim()
                : $"P{_profiles.Count + 1}";
            var profileId = $"p{_profiles.Count + 1}";

            SaveService.CreateProfile(profileId, name, defaultAvatarId);
            ReloadProfiles();
            if (newProfileNameInput != null)
            {
                newProfileNameInput.text = string.Empty;
            }
        }

        private void OnProfileChanged(int index)
        {
            if (index < 0 || index >= _profiles.Count)
            {
                return;
            }

            PlayerSession.SetActiveProfileId(_profiles[index].profileId);
            coinBalanceLabel?.Refresh();
            avatarLoadoutPreview?.Refresh();
            progressSummaryController?.Refresh();
        }
    }
}
