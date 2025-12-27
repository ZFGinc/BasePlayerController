using UnityEngine;
using ZFGinc.Objects.Utils;

namespace ZFGinc.Objects
{
    [RequireComponent(typeof(InteractObject))]
    public class BreakableObject : MonoBehaviour, IDamageable
    {
        [SerializeField] private int Strength = 100;
        [Space]
        [SerializeField] private float VelocyForDestroy = 3f;
        [SerializeField] private int Damage = 15;
        [Space]
        [SerializeField] private GameObject _destroyEffect;

        private InteractObject _interactObject;

        private float _magnitude = 0;

        private void Awake()
        {
            _interactObject = GetComponent<InteractObject>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_interactObject.IsGrab || _interactObject.IsOpen) return;

            _magnitude = _interactObject.Rigidbody.linearVelocity.magnitude;

            if (_magnitude > VelocyForDestroy)
            {
                Debug.Log($"Collision damage: {-(int)(Damage * _magnitude)}");
                Strength -= (int)(Damage * _magnitude);
            }
        }

        public void OnDestroyble()
        {
            Instantiate(_destroyEffect, transform.position, Quaternion.identity);
        }

        public void TakeDamage(int damage)
        {
            Strength -= damage;
        }

        public void TakeHeal(int heal)
        {
            Strength += heal;
        }
    }
}