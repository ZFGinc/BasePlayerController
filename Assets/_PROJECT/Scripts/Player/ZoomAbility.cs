using UnityEngine;

namespace ZFGinc.Player
{
    public class ZoomAbility : MonoBehaviour
    {
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private Camera _itemsCamera;
        [Space(10)]
        [SerializeField] private float _defaultZoom = 40f;
        [SerializeField] private float _maxZoom = 10f;
        [Space(10)]
        [SerializeField] private float _zoomSpeed = 3f;

        private InputBinding _inputBinding;

        private float _tempZoom;

        private void Awake()
        {
            _inputBinding = GetComponent<InputBinding>();
            _tempZoom = _defaultZoom;
        }

        private void Update()
        {
            _tempZoom = Mathf.Lerp(
                _tempZoom,
                (Input.GetKey(_inputBinding.zoomButton) && _inputBinding.IsCanZoom) ? _maxZoom : _defaultZoom,
                _zoomSpeed * Time.deltaTime
            );

            _playerCamera.fieldOfView = _tempZoom;
            _itemsCamera.fieldOfView = _tempZoom;
        }
    }
}