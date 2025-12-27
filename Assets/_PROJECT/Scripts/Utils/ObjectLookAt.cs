using UnityEngine;

namespace ZFGinc.Utils
{
    public class ObjectLookAt : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _speed = 2f;

        private Quaternion _lookRotation;
        private Vector3 _direction;

        private void FixedUpdate()
        {
            if (_target == null) return;

            //find the vector pointing from our position to the target
            _direction = (_target.position - transform.position).normalized;

            //create the rotation we need to be in to look at the target
            _lookRotation = Quaternion.LookRotation(_direction);

            //rotate us over time according to speed until we are in the required rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.fixedDeltaTime * _speed);
        }

        public void SetNewTarget(Transform newTarget) => _target = newTarget;
    }
}