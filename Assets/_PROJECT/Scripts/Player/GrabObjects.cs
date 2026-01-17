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
    public class GrabObjects : MonoBehaviour
    {
        [SerializeField] private Transform _handPivot;

        private InputBinding _inputBinding;
        private LookingObjectRay _lookingObjectRay;
        private Inventory _inventory;

        private InteractObject _cachedInteractObject;
        private int _cachedIndexItem = 0;

        public void Initialize(InputBinding inputBinding, LookingObjectRay lookingObjectRay, Inventory inventory)
        {
            _inputBinding = inputBinding;
            _lookingObjectRay = lookingObjectRay;
            _inventory = inventory;

            //_inventory.OnRedrawInventory += OnRedrawInventory;
        }

        private void Update()
        {
            PlayerInput();
        }

        private void OnRedrawInventory(int value)
        {
            _cachedIndexItem = value;
        }

        private void PlayerInput()
        {
            if (_inputBinding.IsMenuOpened) return;

            if (Input.GetKeyDown(_inputBinding.grabButton))
            {
                StartCoroutine(TryGrabItem());
            }

            if (Input.GetKeyDown(_inputBinding.dropButton))
            {
                TryDropItem();
            }
        }

        private IEnumerator TryGrabItem()
        {
            if (_lookingObjectRay.GetInteractObject() != null)
            {
                _cachedInteractObject = _lookingObjectRay.GetInteractObject();

                if (!_cachedInteractObject.CanGrabObject || _cachedInteractObject.IsHold)
                {
                    _cachedInteractObject.InteractAudio(InteractionCode.Error);
                    yield break;
                }

                //if (_inventory.TryAddItemInventory(_cachedInteractObject.Item))
                //{
                //    OnAddItemInInventory(_cachedInteractObject);
                //}
                else _cachedInteractObject.InteractAudio(InteractionCode.Error);

                _cachedInteractObject = null;
            }
        }

        public void OnAddItemInInventory(InteractObject interactObject, bool isPlayAudio = true)
        {
            if (isPlayAudio) interactObject.InteractAudio(InteractionCode.Grab);
            if (interactObject.gameObject.TryGetComponent(out ObjectToPlace obj)) obj.ReleseObject();

            interactObject.Grab(_handPivot);
            interactObject.DisableCollision();
            interactObject.DisablePhysic();

        }

        public bool TryDropItem()
        {
            Item item = null;//_inventory.CurrentItem;

            if (item == null) return false;

            //if (!_inventory.TryRemoveItemInventory()) return false;

            item.Object.Drop();
            item.Object.EnablePhysic();
            item.Object.EnableCollision();
            item.Object.InteractAudio(InteractionCode.Drop);

            OnRedrawInventory(_cachedIndexItem);

            return true;
        }
    }
}