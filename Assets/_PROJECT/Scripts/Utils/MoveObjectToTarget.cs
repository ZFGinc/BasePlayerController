using UnityEngine;

namespace ZFGinc.Utils
{
    public class MoveObjectToTarget : MonoBehaviour
    {
        public LayerMask mask;
        public Transform Target;
        public float Speed = 1f;

        private Vector3 _rotateVector;
        private bool _isMoving = true;

        private void OnEnable()
        {
            _rotateVector = new Vector3(Random, Random, Random);
        }

        private void FixedUpdate()
        {
            if (!_isMoving) return;
            transform.position = Vector3.Lerp(transform.position, Target.position, Time.fixedDeltaTime * Speed); ;
            transform.Rotate(_rotateVector * 0.3f);

            if (Mathf.Abs(Vector3.Distance(transform.position, Target.position)) < 5f) Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == mask)
            {
                _isMoving = false;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, Target.position);
        }

        private float Random => UnityEngine.Random.Range(-360f, 360);
    }
}