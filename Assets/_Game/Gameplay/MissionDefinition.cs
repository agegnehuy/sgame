using UnityEngine;

namespace SGame.Gameplay
{
    [CreateAssetMenu(menuName = "SGame/Mission Definition", fileName = "MissionDefinition")]
    public class MissionDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string missionId = "P1-M1";
        public string titleKey = "mission.p1m1.title";
        public string briefingKey = "mission.p1m1.briefing";

        [Header("Rules")]
        public bool allowCrossOnlyOnGreen = true;
        public float safeVehicleDistanceMeters = 12f;
        public int requiredSafeCrosses = 1;

        [Header("Scoring")]
        [Range(0, 100)] public int safetyWeight = 70;
        [Range(0, 100)] public int accuracyWeight = 20;
        [Range(0, 100)] public int timingWeight = 10;
    }
}
