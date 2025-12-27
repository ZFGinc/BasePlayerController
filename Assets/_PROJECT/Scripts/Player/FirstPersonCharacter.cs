using UnityEngine;
using ZFGinc.Player.Utils;

namespace ZFGinc.Player
{
    public class FirstPersonCharacter : Character
    {
        [Space(30)]
        [SerializeField] private LayerMask _oxygenLayer;
        [SerializeField] private LayerMask _noVoidLayer;

        [Space(30)]

        [Tooltip("The first person camera parent.")]
        public Transform cameraParent;

        [Space(10)]
        [Tooltip("Head position")]
        public Transform headStandPosition;
        public Transform headCrouchPosition;

        [Space(10)]
        [Tooltip("Head position")]
        public float speedFollowToHead = 1f;

        private float _cameraPitch;
        private Transform _headPosition;

        public virtual void AddControlYawInput(float value)
        {
            if (!canCameraRotation) return;

            if (value != 0.0f)
                AddYawInput(value);
        }

        public virtual void AddControlPitchInput(float value, float minPitch = -80.0f, float maxPitch = 80.0f)
        {
            if (!canCameraRotation) return;

            if (value != 0.0f)
                _cameraPitch = MathLib.ClampAngle(_cameraPitch + value, minPitch, maxPitch);
        }

        protected virtual void UpdateCameraParentRotation()
        {
            if (!canCameraRotation) return;

            cameraParent.localRotation = Quaternion.Euler(_cameraPitch, 0.0f, 0.0f);
        }

        protected virtual void UpdateCameraParentPosition()
        {
            if (!canCameraRotation) return;

            _headPosition = IsCrouched() ? headCrouchPosition : headStandPosition;

            cameraParent.position = Vector3.Lerp(cameraParent.position, _headPosition.position, speedFollowToHead * Time.deltaTime);
        }

        protected virtual void LateUpdate()
        {
            UpdateCameraParentRotation();
            UpdateCameraParentPosition();
        }

        protected override void Reset()
        {
            base.Reset();

            SetRotationMode(RotationMode.None);
        }

        protected bool CheckOxygenTrigger()
        {
            return Physics.CheckSphere(transform.position + Vector3.up, 2f, _oxygenLayer);
        }

        protected bool CheckNoVoidTrigger()
        {
            return Physics.CheckSphere(transform.position + Vector3.up, 2f, _noVoidLayer);
        }
    }
}
