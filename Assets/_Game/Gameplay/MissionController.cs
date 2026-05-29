using System;
using SGame.Data;
using UnityEngine;

namespace SGame.Gameplay
{
    public class MissionController : MonoBehaviour
    {
        [SerializeField] private MissionDefinition missionDefinition;
        [SerializeField] private string profileId = "";
        [SerializeField] private TrafficSignalState currentSignalState = TrafficSignalState.Red;
        [SerializeField] private float nearestVehicleDistanceMeters = 100f;
        [SerializeField] private bool playerIsOnCrosswalk = true;
        [SerializeField] private float timingDiscipline01 = 0.8f;
        [SerializeField] private int baseCoinReward = 15;
        [SerializeField] private int starBonusMultiplier = 5;
        [SerializeField] private bool runtimeAllowCrossOnlyOnGreen = true;
        [SerializeField] private float runtimeSafeVehicleDistanceMeters = 12f;
        [SerializeField] private int runtimeRequiredSafeCrosses = 1;
        [SerializeField] private string runtimeMissionId = "P1-M1";
        [SerializeField] private string runtimeMissionTitleKey = "mission.p1m1.title";
        [SerializeField] private string runtimeMissionBriefingKey = "mission.p1m1.briefing";

        public MissionRuntimeState State { get; private set; } = MissionRuntimeState.None;
        public string CurrentMissionId => runtimeMissionId;
        public string CurrentMissionTitleKey => runtimeMissionTitleKey;
        public string CurrentMissionBriefingKey => runtimeMissionBriefingKey;
        public event Action<MissionRuntimeState> StateChanged;
        public event Action<bool, string> DecisionEvaluated;
        public event Action<int, int, int, int> MissionCompleted;
        public event Action MissionIdentityChanged;

        private int _safeDecisions;
        private int _unsafeDecisions;
        private bool _hasRuntimeOverride;
        private float _missionStartTimeSeconds;

        public void StartMission()
        {
            if (string.IsNullOrWhiteSpace(profileId))
            {
                profileId = PlayerSession.ActiveProfileId;
            }

            _safeDecisions = 0;
            _unsafeDecisions = 0;
            _missionStartTimeSeconds = Time.time;
            if (!_hasRuntimeOverride)
            {
                ApplyMissionDefaultsIfNeeded();
            }
            SetState(MissionRuntimeState.Briefing);
            SetState(MissionRuntimeState.Playing);
        }

        public void SetSignalState(bool isGreen)
        {
            currentSignalState = isGreen ? TrafficSignalState.Green : TrafficSignalState.Red;
        }

        public void UpdateNearestVehicleDistance(float meters)
        {
            nearestVehicleDistanceMeters = Mathf.Max(0f, meters);
        }

        public void SetCrosswalkUsage(bool onCrosswalk)
        {
            playerIsOnCrosswalk = onCrosswalk;
        }

        public void ConfigureRuntime(float safeVehicleDistanceMeters, int requiredSafeCrosses, bool allowCrossOnlyOnGreen, float timingDiscipline)
        {
            _hasRuntimeOverride = true;
            runtimeSafeVehicleDistanceMeters = Mathf.Max(0f, safeVehicleDistanceMeters);
            runtimeRequiredSafeCrosses = Mathf.Max(1, requiredSafeCrosses);
            runtimeAllowCrossOnlyOnGreen = allowCrossOnlyOnGreen;
            timingDiscipline01 = Mathf.Clamp01(timingDiscipline);
        }

        public void ConfigureMissionIdentity(string missionId, string missionTitleKey, string missionBriefingKey)
        {
            _hasRuntimeOverride = true;
            if (!string.IsNullOrWhiteSpace(missionId))
            {
                runtimeMissionId = missionId;
            }

            if (!string.IsNullOrWhiteSpace(missionTitleKey))
            {
                runtimeMissionTitleKey = missionTitleKey;
            }

            if (!string.IsNullOrWhiteSpace(missionBriefingKey))
            {
                runtimeMissionBriefingKey = missionBriefingKey;
            }

            MissionIdentityChanged?.Invoke();
        }

        public void EmitGuidance(string reasonKey, bool positiveTone = true, string guidanceType = "custom")
        {
            DecisionEvaluated?.Invoke(positiveTone, reasonKey);
            AnalyticsEventService.LogChallengeEvent(
                profileId,
                runtimeMissionId,
                guidanceType,
                reasonKey,
                Time.time - _missionStartTimeSeconds);
        }

        public void OnCrossAttempt()
        {
            if (State != MissionRuntimeState.Playing || missionDefinition == null)
            {
                return;
            }

            SetState(MissionRuntimeState.Evaluating);

            var input = new SafetyDecisionInput(currentSignalState, nearestVehicleDistanceMeters, playerIsOnCrosswalk);
            var result = SafetyEvaluator.EvaluateCrossAttempt(
                input,
                runtimeAllowCrossOnlyOnGreen,
                runtimeSafeVehicleDistanceMeters);

            if (result.IsSafe)
            {
                _safeDecisions++;
            }
            else
            {
                _unsafeDecisions++;
            }

            DecisionEvaluated?.Invoke(result.IsSafe, result.ReasonKey);
            AnalyticsEventService.LogDecision(
                profileId,
                runtimeMissionId,
                result.IsSafe,
                result.ReasonKey,
                Time.time - _missionStartTimeSeconds);

            if (_safeDecisions >= runtimeRequiredSafeCrosses)
            {
                CompleteMission();
                return;
            }

            SetState(MissionRuntimeState.Playing);
        }

        public void OnWaitAction()
        {
            if (State == MissionRuntimeState.Playing)
            {
                // Waiting is a neutral/safe choice in MVP week 1.
                const string reasonKey = "feedback.info.wait";
                DecisionEvaluated?.Invoke(true, reasonKey);
                AnalyticsEventService.LogDecision(
                    profileId,
                    runtimeMissionId,
                    true,
                    reasonKey,
                    Time.time - _missionStartTimeSeconds);
            }
        }

        private void CompleteMission()
        {
            var scoreInput = new MissionScoreInput(_safeDecisions, _unsafeDecisions, timingDiscipline01);
            var scoreResult = ScoreCalculator.Calculate(scoreInput, missionDefinition);
            SetState(MissionRuntimeState.Result);

            SaveService.SaveMissionResult(
                profileId: profileId,
                missionId: runtimeMissionId,
                score: scoreResult.FinalScore,
                stars: scoreResult.Stars);

            var earnedCoins = Mathf.Max(0, baseCoinReward + (scoreResult.Stars * starBonusMultiplier));
            var totalCoins = SaveService.AddCoins(profileId, earnedCoins);
            AnalyticsEventService.LogMissionSummary(
                profileId,
                runtimeMissionId,
                _safeDecisions,
                _unsafeDecisions,
                scoreResult.FinalScore,
                scoreResult.Stars,
                earnedCoins,
                Time.time - _missionStartTimeSeconds);

            MissionCompleted?.Invoke(scoreResult.FinalScore, scoreResult.Stars, earnedCoins, totalCoins);
        }

        private void SetState(MissionRuntimeState newState)
        {
            State = newState;
            StateChanged?.Invoke(State);
        }

        private void ApplyMissionDefaultsIfNeeded()
        {
            if (missionDefinition == null)
            {
                return;
            }

            runtimeAllowCrossOnlyOnGreen = missionDefinition.allowCrossOnlyOnGreen;
            runtimeSafeVehicleDistanceMeters = missionDefinition.safeVehicleDistanceMeters;
            runtimeRequiredSafeCrosses = missionDefinition.requiredSafeCrosses;
            runtimeMissionId = missionDefinition.missionId;
            runtimeMissionTitleKey = missionDefinition.titleKey;
            runtimeMissionBriefingKey = missionDefinition.briefingKey;
        }
    }
}
