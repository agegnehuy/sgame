namespace SGame.Gameplay
{
    public enum TrafficSignalState
    {
        Red = 0,
        Green = 1
    }

    public readonly struct SafetyDecisionInput
    {
        public SafetyDecisionInput(TrafficSignalState signalState, float nearestVehicleDistanceMeters, bool usedCrosswalk)
        {
            SignalState = signalState;
            NearestVehicleDistanceMeters = nearestVehicleDistanceMeters;
            UsedCrosswalk = usedCrosswalk;
        }

        public TrafficSignalState SignalState { get; }
        public float NearestVehicleDistanceMeters { get; }
        public bool UsedCrosswalk { get; }
    }

    public readonly struct SafetyDecisionResult
    {
        public SafetyDecisionResult(bool isSafe, string reasonKey)
        {
            IsSafe = isSafe;
            ReasonKey = reasonKey;
        }

        public bool IsSafe { get; }
        public string ReasonKey { get; }
    }

    public static class SafetyEvaluator
    {
        public static SafetyDecisionResult EvaluateCrossAttempt(SafetyDecisionInput input, MissionDefinition mission)
        {
            return EvaluateCrossAttempt(input, mission.allowCrossOnlyOnGreen, mission.safeVehicleDistanceMeters);
        }

        public static SafetyDecisionResult EvaluateCrossAttempt(
            SafetyDecisionInput input,
            bool allowCrossOnlyOnGreen,
            float safeVehicleDistanceMeters)
        {
            if (allowCrossOnlyOnGreen && input.SignalState == TrafficSignalState.Red)
            {
                return new SafetyDecisionResult(false, "feedback.unsafe.red_light");
            }

            if (input.NearestVehicleDistanceMeters < safeVehicleDistanceMeters)
            {
                return new SafetyDecisionResult(false, "feedback.unsafe.vehicle_close");
            }

            if (!input.UsedCrosswalk)
            {
                return new SafetyDecisionResult(false, "feedback.unsafe.no_crosswalk");
            }

            return new SafetyDecisionResult(true, "feedback.safe.cross_now");
        }
    }
}
