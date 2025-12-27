using UnityEngine;
using ZFGinc.Objects;

namespace ZFGinc.Player
{
    public class LookingObjectRay : MonoBehaviour
    {
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private float _rayDistance = 2.5f;
        [SerializeField] private float _hoverDistance = 3f;
        [SerializeField] private LayerMask _targetLayers;
        [SerializeField] private LayerMask _breakLayers;

        [SerializeField] private InteractObject _currentInteractObject = null;

        private Vector3 _currentPoint = Vector3.zero;
        private RaycastHit? _hitInfo;
        private GameObject _lookingObject;
        private InputBinding _inputBinding;

        public Transform CameraTransform => _playerCamera.transform;

        public InteractObject GetInteractObject() => _currentInteractObject;

        public GameObject LookingGameObject => _lookingObject;

        public Vector3 CurrentPoint => _currentPoint;

        public float Distance => GetDistance();

        public void Initialize(InputBinding inputBinding)
        {
            _inputBinding = inputBinding;
        }

        private void Update()
        {
            Debug.DrawRay(CameraTransform.position, CameraTransform.forward * _rayDistance, Color.yellow);

            if (_currentInteractObject != null && _currentInteractObject.IsOpen) return;

            if (Physics.Raycast(CameraTransform.position, CameraTransform.forward, out RaycastHit hit, _rayDistance, _targetLayers))
            {
                _currentPoint = hit.point;
                _lookingObject = hit.transform.gameObject;

                if (hit.collider.gameObject.layer == _breakLayers)
                {
                    HideInteractMenu();
                    return;
                }

                if (hit.transform.TryGetComponent(out InteractObject interactObject) && !_inputBinding.IsMenuOpened)
                {
                    if (_inputBinding.IsLockInput) return;
                    if (hit.distance > _hoverDistance || !interactObject.enabled)
                    {
                        HideInteractMenu();
                        return;
                    }

                    if (_currentInteractObject != null)
                    {
                        HideInteractMenu();
                    }

                    _hitInfo = hit;
                    ShowInteractMenu(interactObject);

                    return;
                }
                else
                {
                    HideInteractMenu();
                }
            }
            else if (_currentInteractObject != null)
            {
                HideInteractMenu();
            }
        }

        private void ShowInteractMenu(InteractObject interactObject)
        {
            _currentInteractObject = interactObject;
            _currentInteractObject.IsHover = true;
            _currentInteractObject.ShowInteractMenu();
        }

        private void HideInteractMenu()
        {
            if (_currentInteractObject == null) return;

            _currentInteractObject.IsHover = false;
            _currentInteractObject.HideInteractMenu();
            _currentInteractObject = null;
        }

        private float GetDistance()
        {
            if (GetInteractObject() == null) return 9999f;

            float distance = Vector3.Distance(CameraTransform.position, GetInteractObject().transform.position);

            return distance;
        }
    }
}