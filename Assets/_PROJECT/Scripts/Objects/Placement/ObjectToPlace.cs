using UnityEngine;

namespace ZFGinc.Objects.Placement
{
    [RequireComponent(typeof(InteractObject))]
    public class ObjectToPlace : MonoBehaviour
    {
        public bool IsPlaced { get; private set; } = false;

        [SerializeField] private string _key;

        private InteractObject _cachedInteractObject;
        private PlacementArea _placementTarget;
        private bool _alreadyPosition = false;

        public InteractObject Object => _cachedInteractObject;
        public string Key => _key;

        protected virtual void Awake()
        {
            _cachedInteractObject = GetComponent<InteractObject>();
        }

        protected virtual void FixedUpdate()
        {
            if (_placementTarget == null) return;
            if (_placementTarget.IsOptimized && _alreadyPosition) return;

            CheckDestination();
            SetTransform();
        }

        protected virtual void SetTransform()
        {
            if (_placementTarget == null) return;

            transform.SetPositionAndRotation(
                Vector3.Lerp(transform.position, _placementTarget.PositionOffset, Time.fixedDeltaTime * 10),
                Quaternion.Lerp(transform.rotation, _placementTarget.Rotation, Time.fixedDeltaTime * 10));
        }

        protected virtual void CheckDestination()
        {
            if (_placementTarget == null) return;

            _alreadyPosition = CheckPosition() <= 0.1f && CheckRotation() <= 1f;
        }

        protected virtual float CheckPosition()
        {
            return Vector3.Distance(_placementTarget.PositionOffset, transform.position);
        }

        protected virtual float CheckRotation()
        {
            return Quaternion.Angle(_placementTarget.Rotation, transform.rotation);
        }

        public void PlaceObject(PlacementArea target)
        {
            _placementTarget = target;

            if (_placementTarget.IsHasPlaceObejct)
            {
                _placementTarget = null;
                return;
            }

            IsPlaced = true;
            _alreadyPosition = false;

            _placementTarget.ChangeState(true, this);

            _cachedInteractObject.DisablePhysic();
        }

        public void ReleseObject()
        {
            if (_placementTarget == null) return;

            IsPlaced = false;
            _alreadyPosition = false;

            _cachedInteractObject.EnablePhysic();

            _placementTarget.ChangeState(false, null);

            _placementTarget = null;
        }

        public void ReleseObjectForce()
        {
            IsPlaced = false;
            _alreadyPosition = false;

            _cachedInteractObject.EnablePhysic();

            _placementTarget = null;
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            if (IsPlaced) return;
            if (_cachedInteractObject.IsHold || _cachedInteractObject.IsGrab) return;

            if (other.TryGetComponent(out PlacementArea obj))
            {
                if (Key != obj.RequiredKey) return;
                PlaceObject(obj);
            }
        }
    }
}