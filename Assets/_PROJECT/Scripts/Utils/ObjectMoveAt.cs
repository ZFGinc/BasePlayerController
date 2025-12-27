using UnityEngine;

namespace ZFGinc.Utils
{
    public class ObjectMoveAt : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _speed = 2f;

        private void FixedUpdate()
        {
            if (_target == null) return;

            transform.position = Vector3.Lerp(transform.position, _target.position, Time.fixedDeltaTime * _speed);
        }

        public void SetNewTarget(Transform newTarget) => _target = newTarget;

        public void SetNewSpeed(float speed) => _speed = speed;
    }
}