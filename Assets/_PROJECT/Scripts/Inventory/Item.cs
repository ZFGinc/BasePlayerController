using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using ZFGinc.Objects;

namespace ZFGinc.InventoryItems
{
    [RequireComponent(typeof(InteractObject))]
    public class Item : MonoBehaviour
    {
        [Expandable, SerializeField] private ItemInfo _info;
        [SerializeField, Layer] private int _baseLayer;
        [Space]
        [SerializeField] private Vector3 _positionOffcet;
        [SerializeField] private Vector3 _rotationOffcet;
        [Space]
        [SerializeField] private UnityEvent OnShowItem;
        [SerializeField] private UnityEvent OnHideItem;
        [SerializeField] private UnityEvent OnDropItem;

        private InteractObject _interactObject;

        public ItemInfo Info => _info;
        public InteractObject Object => _interactObject;
        public Vector3 PositionOffcet => _positionOffcet;
        public Vector3 RotationOffcet => _rotationOffcet;

        protected virtual void Awake()
        {
            _interactObject = GetComponent<InteractObject>();
        }

        protected virtual void Start()
        {
            Object.SetDisplayName(_info.Name);
        }

        public virtual void ChangeVisible(bool visible)
        {
            Debug.Log("Item change visible: " + visible);
            foreach (Renderer render in Object.RenderersForInventory)
            {
                render.enabled = visible;
            }

            if (visible) OnShowItem?.Invoke();
            else OnHideItem?.Invoke();
        }

        public virtual void ChangeIsStorage(bool isStorage)
        {
            Debug.Log("Item change is storage: " + isStorage);

            ChangeVisible(!isStorage);
            if (isStorage)
            {
                Object.DisablePhysic();
                Object.DisableCollision();
            }
            else
            {
                Object.EnableCollision();
                Object.EnablePhysic();
            }
        }

        public virtual void SetLayer(int layer)
        {
            foreach (Renderer render in Object.RenderersForInventory)
            {
                render.gameObject.layer = layer;
            }
        }

        public virtual void ResetLayer()
        {
            foreach (Renderer render in Object.RenderersForInventory)
            {
                render.gameObject.layer = _baseLayer;
            }

            OnDropItem?.Invoke();
        }
    }
}