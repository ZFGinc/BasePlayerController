using System.Collections;
using UnityEngine;
using ZFGinc.InventoryItems;
using ZFGinc.Objects;
using ZFGinc.Objects.Placement;

namespace ZFGinc.Player
{
    [RequireComponent(typeof(InputBinding))]
    [RequireComponent(typeof(LookingObjectRay))]
    [RequireComponent(typeof(Inventory))]
    public class EquipObjects : MonoBehaviour
    {
        [SerializeField] private Transform _headPivot;

        private InputBinding _inputBinding;
        private LookingObjectRay _lookingObjectRay;
        private Inventory _inventory;

        private InteractObject _cachedInteractObject;

        public void Initialize(InputBinding inputBinding, LookingObjectRay lookingObjectRay, Inventory inventory)
        {
            _inputBinding = inputBinding;
            _lookingObjectRay = lookingObjectRay;
            _inventory = inventory;
        }

        private void Update()
        {
            PlayerInput();
        }

        private void PlayerInput()
        {
            if (_inputBinding.IsMenuOpened) return;

            if (Input.GetKeyDown(_inputBinding.equipButton))
            {
                StartCoroutine(TryEquipItem());
            }

            if (Input.GetKeyDown(_inputBinding.slot7)) TryUnequipItem(0);
            if (Input.GetKeyDown(_inputBinding.slot8)) TryUnequipItem(1);
            if (Input.GetKeyDown(_inputBinding.slot9)) TryUnequipItem(2);
        }

        private IEnumerator TryEquipItem()
        {
            if (_lookingObjectRay.GetInteractObject() != null)
            {
                _cachedInteractObject = _lookingObjectRay.GetInteractObject();

                if (!_cachedInteractObject.CanEquipObject || _cachedInteractObject.IsHold)
                {
                    _cachedInteractObject.InteractAudio(InteractionCode.Error);
                    yield break;
                }

                if (_inventory.TryAddItemEquipment(_cachedInteractObject.Item))
                {
                    OnAddItemInEquipment(_cachedInteractObject);
                }
                else _cachedInteractObject.InteractAudio(InteractionCode.Error);

                _cachedInteractObject = null;
            }
        }

        public void OnAddItemInEquipment(InteractObject interactObject, bool isPlayAudio = true)
        {
            if (interactObject.Item != null)
            {
                switch (interactObject.EquipmentType)
                {
                    case EquipmentType.Head: interactObject.EquipObject(_headPivot); break;
                    default: return;
                }
            }

            if (interactObject.gameObject.TryGetComponent(out ObjectToPlace obj)) obj.ReleseObject();
            if (isPlayAudio) interactObject.InteractAudio(InteractionCode.Grab);

            interactObject.DisableCollision();
            interactObject.DisablePhysic();
        }

        public bool TryUnequipItem(int index)
        {
            Item item = _inventory.GetEquipment(index);

            if (item == null) return false;
            if (!_inventory.TryRemoveItemEquipment(index)) return false;

            item.Object.UnequipObject();
            item.Object.EnablePhysic();
            item.Object.EnableCollision();
            item.Object.InteractAudio(InteractionCode.Drop);

            return true;
        }
    }
}