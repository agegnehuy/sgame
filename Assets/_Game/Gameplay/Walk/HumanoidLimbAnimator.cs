using UnityEngine;

namespace SGame.Gameplay.Walk
{
    /// <summary>
    /// Procedural walking animation for a "stick-figure" humanoid built from
    /// primitives. Swings the four limbs around the X axis at their pivot
    /// when IsMoving is true, and eases them back to rest when not moving.
    /// </summary>
    public class HumanoidLimbAnimator : MonoBehaviour
    {
        [SerializeField] private Transform leftArm;
        [SerializeField] private Transform rightArm;
        [SerializeField] private Transform leftLeg;
        [SerializeField] private Transform rightLeg;

        [SerializeField] private float swingFrequency = 6f;
        [SerializeField] private float armSwingDegrees = 28f;
        [SerializeField] private float legSwingDegrees = 38f;
        [SerializeField] private float idleEaseSpeed = 6f;

        private float _phase;
        private float _amount;

        public bool IsMoving { get; set; }

        public void Configure(Transform lArm, Transform rArm, Transform lLeg, Transform rLeg)
        {
            leftArm = lArm;
            rightArm = rArm;
            leftLeg = lLeg;
            rightLeg = rLeg;
        }

        public void SetMoving(bool moving)
        {
            IsMoving = moving;
        }

        private void Update()
        {
            // Ramp the swing amount up/down so transitions are smooth.
            float target = IsMoving ? 1f : 0f;
            _amount = Mathf.MoveTowards(_amount, target, idleEaseSpeed * Time.deltaTime);

            if (IsMoving)
            {
                _phase += Time.deltaTime * swingFrequency;
            }

            float swing = Mathf.Sin(_phase) * _amount;
            float armAngle = swing * armSwingDegrees;
            float legAngle = swing * legSwingDegrees;

            if (leftArm  != null) leftArm.localRotation  = Quaternion.Euler( armAngle, 0f, 0f);
            if (rightArm != null) rightArm.localRotation = Quaternion.Euler(-armAngle, 0f, 0f);
            if (leftLeg  != null) leftLeg.localRotation  = Quaternion.Euler(-legAngle, 0f, 0f);
            if (rightLeg != null) rightLeg.localRotation = Quaternion.Euler( legAngle, 0f, 0f);
        }
    }
}
