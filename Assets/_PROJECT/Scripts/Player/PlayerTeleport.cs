using UnityEngine;

namespace ZFGinc.Player
{
    public class PlayerTeleport : MonoBehaviour
    {
        [SerializeField] private Vector3 _newPosition;
        [SerializeField] private Vector3 _newRotation;

        private Rigidbody _plaeyrRigidbody;

        public void Teleport()
        {
            if (_plaeyrRigidbody == null) PlayerInitialization.Instance.FirstPersonCharacter.gameObject.TryGetComponent(out _plaeyrRigidbody);

            _plaeyrRigidbody.position = _newPosition;
            _plaeyrRigidbody.rotation = Quaternion.Euler(_newRotation);
        }

        public void NewPosition(Vector3 newPosition)
        {
            if (_plaeyrRigidbody == null) PlayerInitialization.Instance.FirstPersonCharacter.gameObject.TryGetComponent(out _plaeyrRigidbody);

            _plaeyrRigidbody.position = newPosition;
        }

        public void NewRotation(Vector3 newRotation)
        {
            if (_plaeyrRigidbody == null) PlayerInitialization.Instance.FirstPersonCharacter.gameObject.TryGetComponent(out _plaeyrRigidbody);

            _plaeyrRigidbody.rotation = Quaternion.Euler(newRotation);
        }
    }
}
