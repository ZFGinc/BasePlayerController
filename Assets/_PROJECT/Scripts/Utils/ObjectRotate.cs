using UnityEngine;

namespace ZFGinc.Utils
{
    public class ObjectRotate : MonoBehaviour
    {
        [SerializeField] protected Vector3 _direction;
        [SerializeField] protected float _speed;

        protected virtual void FixedUpdate()
        {
            transform.Rotate(_direction * Time.fixedDeltaTime * _speed);
        }
    }
}