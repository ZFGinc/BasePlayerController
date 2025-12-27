using System;
using UnityEngine;

namespace ZFGinc.Objects.Placement
{
    public class PlacementArea : MonoBehaviour
    {
        public bool IsOptimized = true;

        [SerializeField] private string _requiredKey;
        [SerializeField] protected Vector3 _positionOffset;
        [SerializeField] private Quaternion _rotationOffset;

        private bool _isHasPlaceObject;
        private ObjectToPlace _placeObject;

        public bool IsHasPlaceObejct => _isHasPlaceObject && (PlaceObject == null ? false : PlaceObject.gameObject.activeSelf);
        public string RequiredKey => _requiredKey;
        public Vector3 PositionOffset => transform.position + _positionOffset;
        public Quaternion Rotation => Quaternion.Euler(transform.rotation.eulerAngles + _rotationOffset.eulerAngles);
        public ObjectToPlace PlaceObject => _placeObject;

        public Action<bool> OnPlaceObejctChanged;

        private void LateUpdate()
        {
            if (PlaceObject == null && _isHasPlaceObject)
            {
                ChangeState(false, null);
            }

            if (PlaceObject != null && !PlaceObject.gameObject.activeSelf)
            {
                ChangeState(false, null);
            }
        }

        public virtual void ChangeState(bool state, ObjectToPlace obj)
        {
            _placeObject = obj;
            _isHasPlaceObject = state;
            OnPlaceObejctChanged?.Invoke(state);
        }
    }
}