using UnityEngine;

namespace ZFGinc.Player
{
    public class AnimationController : MonoBehaviour
    {
        private static readonly int Forward = Animator.StringToHash("Forward");
        private static readonly int Turn = Animator.StringToHash("Turn");
        private static readonly int Ground = Animator.StringToHash("OnGround");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int InWater = Animator.StringToHash("InWater");

        private Character _character;

        private void Awake()
        {
            _character = GetComponentInParent<Character>();
            _character.Jumped += OnJump;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            bool inVoid = _character.movementMode == Character.MovementMode.Swimming;

            Animator animator = _character.GetAnimator();
            Vector3 move = transform.InverseTransformDirection(_character.GetMovementDirection());

            float forwardAmount = _character.useRootMotion && _character.GetRootMotionController()
                ? move.z
                : Mathf.InverseLerp(0.0f, _character.GetMaxSpeed(), _character.GetSpeed())
                        / (_character.isSprinting ? 1 : 2);

            float turnAmount = _character.GetMovementInput().x / (_character.isSprinting ? 1 : 2);

            animator.SetFloat(Forward, forwardAmount > 0.1f ? forwardAmount : 0, 0.1f, deltaTime);
            animator.SetFloat(Turn, turnAmount > 0.08f || turnAmount < -0.08f ? turnAmount : 0, 0.1f, deltaTime);
            animator.SetBool(Ground, _character.IsGrounded());
            animator.SetBool(InWater, inVoid);
        }

        private void OnJump()
        {
            if (_character.movementMode == Character.MovementMode.Swimming) return;

            Animator animator = _character.GetAnimator();
            animator.SetTrigger(Jump);
        }
    }
}