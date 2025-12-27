using UnityEngine;
using ZFGinc.Objects;

namespace ZFGinc.Player
{
    [RequireComponent(typeof(InputBinding))]
    [RequireComponent(typeof(LookingObjectRay))]
    public class UseObjects : MonoBehaviour
    {
        private InputBinding _inputBinding;
        private LookingObjectRay _lookingObjectRay;

        private InteractObject _cachedInteractObject;

        public void Initialize(InputBinding inputBinding, LookingObjectRay lookingObjectRay)
        {
            _inputBinding = inputBinding;
            _lookingObjectRay = lookingObjectRay;
        }

        private void Update()
        {
            PlayerInput();
        }

        private void PlayerInput()
        {
            if (_inputBinding.IsMenuOpened) return;

            if (Input.GetKeyDown(_inputBinding.useButton))
            {
                CacheInteractObject();
                UseObject();
            }

            if (Input.GetKeyDown(_inputBinding.exitMenuKey))
            {
                CacheInteractObject();
            }

            _cachedInteractObject = null;
        }

        private void CacheInteractObject()
        {
            if (_lookingObjectRay.GetInteractObject() != null)
            {
                _cachedInteractObject = _lookingObjectRay.GetInteractObject();

                if (_cachedInteractObject == null || _cachedInteractObject.IsHold || !_cachedInteractObject.CanUseObject)
                {
                    _cachedInteractObject.InteractAudio(InteractionCode.Error);
                    return;
                }
            }
        }

        private void UseObject()
        {
            if (_cachedInteractObject == null) return;
            if (!_cachedInteractObject.CanUseObject) return;

            _cachedInteractObject.Use();
        }
    }
}