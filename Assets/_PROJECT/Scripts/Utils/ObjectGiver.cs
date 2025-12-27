using UnityEngine;
using ZFGinc.InventoryItems;
using ZFGinc.Objects;
using ZFGinc.Player;

namespace ZFGinc.Utils
{
    public class ObjectGiver : MonoBehaviour
    {
        [SerializeField] private ItemInfo _item;

        public void Give()
        {
            InteractObject obj = Instantiate(_item.Prefab).GetComponent<InteractObject>();

            PlayerInitialization.Instance.Inventory.TryAddItemInventory(obj.Item);
            PlayerInitialization.Instance.GrabObjects.OnAddItemInInventory(obj, true);
        }
    }
}