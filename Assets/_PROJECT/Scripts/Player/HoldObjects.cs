using UnityEngine;
using ZFGinc.Objects;
using ZFGinc.Objects.Placement;

namespace ZFGinc.Player
{
    public class HoldObjects : MonoBehaviour
    {
        [SerializeField] private Transform _holdPivot;
        [Space(10)]
        [SerializeField] private float _minZoom = .5f;
        [SerializeField] private float _maxZoom = 2f;
        [SerializeField] private float _pushMaxErrorDistance = 2f;
        [Space]
        [SerializeField] private float _zoomSpeed = 5f;
        [SerializeField] private float _scrollSpeed = .2f;
        [SerializeField] private float _speedRotation = 3f;
        [Space]
        [SerializeField] private float _pushVelocityMultiplayer = 2f;
        [SerializeField] private float _pushEpsilonMagnitude = 0.3f;
        [Space]
        [SerializeField, Range(0.1f, 3f)] private float _massObejctFactor = 0.3f;

        private InputBinding _inputBinding;
        private LookingObjectRay _lookingObjectRay;
        private FirstPersonCharacter _firstPersonCharacter;

        private InteractObject _cachedInteractObject;
        private Vector3 _cachedRotation = Vector3.zero;
        private Vector3 _pushedVector = Vector3.zero;
        private float _currentZoom = 10f;
        private float _cachedObjectMass = 1f;
        private bool _isHold = false;
        private bool _isPush = false;

        public bool IsHold => _isHold;

        private void Update()
        {
            PlayerInput();
            CalculateVelocity();
            RotateObject();
            TransformateHoldObject();
            CheckErrorDistance();
        }

        private void LateUpdate()
        {
            if (_cachedInteractObject != null && _cachedInteractObject.CanScroll)
            {
                _holdPivot.localPosition = Vector3.zero;
                return;
            }

            _currentZoom = Mathf.Clamp(_currentZoom, _minZoom, _maxZoom);
            _holdPivot.localPosition = Vector3.Lerp(
                _holdPivot.localPosition,
                new Vector3(0, 0, _currentZoom),
                _zoomSpeed * Time.deltaTime);
        }

        public void Initialize(InputBinding inputBinding, LookingObjectRay lookingObjectRay, FirstPersonCharacter firstPersonCharacter)
        {
            _inputBinding = inputBinding;
            _lookingObjectRay = lookingObjectRay;
            _firstPersonCharacter = firstPersonCharacter;

            _inputBinding.OnMouseScrollWheel += MouseScroll;

            _currentZoom = (_minZoom + _maxZoom) / 2;
        }

        private void MouseScroll(int value)
        {
            _currentZoom += value * _scrollSpeed;
            ScrollObject(value);
        }

        private void PlayerInput()
        {
            if (_inputBinding.IsMenuOpened) return;

            if (Input.GetKeyDown(_inputBinding.holdButton))
            {
                HoldObject();
            }
            if (Input.GetKeyUp(_inputBinding.holdButton))
            {
                ReleaseObject();
            }
        }

        private void CalculateVelocity()
        {
            if (!_isHold || _cachedInteractObject == null) return;
            if (!_cachedInteractObject.CanMoveObject) return;

            _pushedVector = _holdPivot.transform.position - _cachedInteractObject.transform.position;
            _pushedVector *= (1 / _cachedObjectMass);
        }

        private void TryPushObject()
        {
            if (!_isHold || _cachedInteractObject == null) return;

            if (_pushedVector.sqrMagnitude > _pushEpsilonMagnitude) _isPush = true;
            else _isPush = false;

            _cachedInteractObject.TrySetVelocity((_pushedVector * _pushVelocityMultiplayer) + GetGravityVector());

            _pushedVector = Vector3.zero;
        }

        RigidbodyConstraints constraints = RigidbodyConstraints.None;
        Vector3 rotation = Vector3.zero;
        private void TransformateHoldObject()
        {
            if (!_isHold) return;
            if (_cachedInteractObject == null) return;

            if (_cachedInteractObject.Rigidbody == null) return;

            rotation = Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * _cachedRotation;

            constraints = _cachedInteractObject.Rigidbody.constraints;

            if (_cachedInteractObject.AcessVectorRotate.x == 0) constraints = constraints | RigidbodyConstraints.FreezeRotationX;
            if (_cachedInteractObject.AcessVectorRotate.y == 0) constraints = constraints | RigidbodyConstraints.FreezeRotationY;
            if (_cachedInteractObject.AcessVectorRotate.z == 0) constraints = constraints | RigidbodyConstraints.FreezeRotationZ;

            _cachedInteractObject.Rigidbody.constraints = constraints;

            _cachedInteractObject.Rigidbody.linearVelocity = (_pushedVector * _pushVelocityMultiplayer) + GetGravityVector();
            _cachedInteractObject.Rigidbody.angularVelocity = rotation.normalized * _speedRotation * _cachedInteractObject.SpeedRotateMultiplayer;
        }

        private void RotateObject()
        {
            if (_cachedInteractObject == null) return;
            if (!_cachedInteractObject.CanRotateObject) return;

            if (_isHold && Input.GetKey(_inputBinding.RCM))
            {
                _firstPersonCharacter.canCameraRotation = false;
                Vector2 mouseInput = _inputBinding.GetMouseDirection();

                _cachedRotation.y = -mouseInput.x;
                _cachedRotation.x = mouseInput.y;

            }
            if (_isHold && Input.GetKeyUp(_inputBinding.RCM))
            {
                _firstPersonCharacter.canCameraRotation = true;
                _cachedRotation = Vector2.zero;
            }
        }

        private void ScrollObject(float value)
        {
            if (_cachedInteractObject == null) return;
            if (!_cachedInteractObject.CanScroll) return;

            _cachedInteractObject.OnScroll(value / (Input.GetKey(_inputBinding.sprintButton) ? 1 : 10));
        }

        private void HoldObject()
        {
            if (_lookingObjectRay.GetInteractObject() == null) return;

            _cachedInteractObject = _lookingObjectRay.GetInteractObject();

            if (_cachedInteractObject.IsGrab) return;

            if (_cachedInteractObject.CanHoldObject)
            {
                if (_cachedInteractObject.gameObject.TryGetComponent(out ObjectToPlace obj)) obj.ReleseObject();

                _cachedInteractObject.InteractAudio(InteractionCode.Hold);

                _cachedRotation = Vector3.zero;
                _currentZoom = _maxZoom / 2;

                _isHold = true;
                _cachedInteractObject.IsHold = true;

                _cachedInteractObject.Hold();

                _cachedObjectMass = GetObjectMass();
            }
            else
            {
                _cachedInteractObject.InteractAudio(InteractionCode.Error);
                _isHold = false;
            }
        }

        private void ReleaseObject()
        {
            if (_cachedInteractObject == null) return;
            if (_cachedInteractObject.IsGrab) return;

            if (!_cachedInteractObject.CanHoldObject)
            {
                _cachedInteractObject.InteractAudio(InteractionCode.Error);
                return;
            }

            TryPushObject();

            if (_isPush) _cachedInteractObject.InteractAudio(InteractionCode.Push);
            else _cachedInteractObject.InteractAudio(InteractionCode.Unhold);

            _firstPersonCharacter.canCameraRotation = true;
            _cachedInteractObject.IsHold = false;
            _cachedInteractObject = null;
            _cachedObjectMass = 1f;
            _isHold = false;
            _isPush = false;
        }

        private void CheckErrorDistance()
        {
            if (!_isHold) return;
            if (_cachedInteractObject == null) return;
            if (_cachedInteractObject.IsGrab) return;

            float distance = Vector3.Distance(_holdPivot.position, _cachedInteractObject.transform.position);

            if (distance > _pushMaxErrorDistance)
            {
                ReleaseObject();
            }
        }

        private float GetObjectMass()
        {
            if (_cachedInteractObject == null) return 1f;
            if (_cachedInteractObject.Rigidbody == null) return 1f;
            if (!IsHold) return 1f;

            return _cachedInteractObject.Rigidbody.mass * _massObejctFactor;
        }

        private Vector3 GetGravityVector()
        {
            return Vector3.down * _cachedObjectMass * (_cachedInteractObject.InWater ? 0f : 1f);
        }
    }
}