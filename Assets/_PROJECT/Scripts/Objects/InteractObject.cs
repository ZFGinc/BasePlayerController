using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using ZFGinc.InventoryItems;
using ZFGinc.Objects.Audio;
using ZFGinc.Objects.Utils;
using ZFGinc.Player;

namespace ZFGinc.Objects
{
    [RequireComponent(typeof(ObjectSound))]
    public class InteractObject : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS
        [Foldout("State")] public bool InWater = false;
        [Foldout("State")] public bool IsOpen = false;
        [Foldout("State")] public bool IsHold = false;
        [Foldout("State")] public bool IsGrab = false;
        [Foldout("State")] public bool IsHover = false;
        [Foldout("State")] public bool CanPhysic = true;
        [Foldout("State"), Space] public Transform InventoryPivot = null;
        [Foldout("State")] public bool SmoothPosition = true;
        [Foldout("State")] public bool SmoothRotation = true;
        [Foldout("State")] public float SmoothPositionSpeed = 10f;
        [Foldout("State")] public float SmoothRotationSpeed = 10f;

        public bool InvertWaterLayer = false;
        public LayerMask WaterLayer;

        [Space]
        [Expandable]
        [SerializeField] protected ObjectInformation _objectInfo;

        [Space(30)]
        [SerializeField] private Collider _collider;
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Renderer[] _renderersForInventory;

        [Space(15)]
        [Header("InteractMenu")]
        [SerializeField] protected string _displayName;
        [SerializeField] protected Texture _textureOutline;
        [SerializeField] protected Texture _textureBackground;
        [SerializeField] protected float _deltaSize = 10f;
        [SerializeField] protected float _outlineSize = 7f;

        [Space]
        [SerializeField, ShowIf(nameof(CanHoldObject))] protected string _textKeyHoldGUI = "TEXT_HOLD";
        [SerializeField, ShowIf(nameof(CanGrabObject))] protected string _textKeyGrabGUI = "TEXT_GRAB";
        [SerializeField, ShowIf(nameof(CanEquipObject))] protected string _textKeyEquipGUI = "TEXT_EQUIP";
        [SerializeField, ShowIf(nameof(CanUseObject))] protected string _textKeyUseGUI = "TEXT_USE";

        [Header("Events")]
        [Space(15), ShowIf(nameof(CanHoldObject))]
        [SerializeField] protected UnityEvent _eventHold;
        [Space(15), ShowIf(nameof(CanGrabObject))]
        [SerializeField] protected UnityEvent _eventGrab;
        [Space(15), ShowIf(nameof(CanGrabObject))]
        [SerializeField] protected UnityEvent _eventDrop;
        [Space(15), ShowIf(nameof(CanUseObject))]
        [SerializeField] protected UnityEvent _eventUse;
        [Space(15), ShowIf(nameof(CanEquipObject))]
        [SerializeField] protected UnityEvent _eventEquip;
        [Space(15), ShowIf(nameof(CanEquipObject))]
        [SerializeField] protected UnityEvent _eventUnequip;
        [Space(15), ShowIf(nameof(CanUseInInventory))]
        [SerializeField] protected UnityEvent _eventUseInInventory;
        [Space(15), ShowIf(nameof(CanActionObject))]
        [SerializeField] protected UnityEvent _eventAction;
        [Space(15), ShowIf(nameof(CanScroll))]
        [SerializeField] protected UnityEvent _eventScroll;
        [SerializeField, ShowIf(nameof(ShowSpeedRotateMultiplayer))] protected float _speedRotationMultiplayer = 1;
        [Space(15), SerializeField, ShowIf(nameof(CanScroll))] protected Vector2 _minMaxValue = new(-1, 1);
        #endregion

        #region PROPERTIES
        protected Rect _rect;

        private Item _item;
        private ObjectSound _objectAudio;
        private Rigidbody _rigidbody;
        private float _value = 0;

        protected string _textHoldGUI = "KEY_HOLD";
        protected string _textGrabGUI = "KEY_GRAB";
        protected string _textUseGUI = "KEY_USE";
        protected string _textEquipGUI = "KEY_EQUIP";

        public ObjectInformation ObjectInformation => _objectInfo;
        public float Scroll => _value;
        public Renderer[] RenderersForInventory => _renderersForInventory == null || _renderersForInventory.Length == 0 ? new Renderer[] { Renderer } : _renderersForInventory;

        public bool CanRotateObject => _objectInfo != null && _objectInfo.CanRotateObject;
        public bool CanMoveObject => _objectInfo != null && _objectInfo.CanMoveObject;
        public bool CanHoldObject => _objectInfo != null && _objectInfo.CanHoldObject;
        public bool CanGrabObject => _objectInfo != null && _objectInfo.CanGrabObject;
        public bool CanEquipObject => _objectInfo != null && _objectInfo.CanEquipObject;
        public bool CanUseObject => _objectInfo != null && _objectInfo.CanUseObject;
        public bool CanActionObject => _objectInfo != null && _objectInfo.CanActionObject;
        public bool CanUseInInventory => _objectInfo != null && _objectInfo.CanUseInInventory;
        public EquipmentType EquipmentType => _objectInfo != null ? _objectInfo.EquipmentType : EquipmentType.Null;
        public Vector3 AcessVectorRotate => _objectInfo.AcessVectorRotate;
        public bool CanScroll => _objectInfo != null && _objectInfo.CanScroll;
        public float SpeedRotateMultiplayer => _speedRotationMultiplayer;
        public bool ShowSpeedRotateMultiplayer => CanScroll || CanRotateObject;

        public Rigidbody Rigidbody
        {
            get
            {
                if (_rigidbody == null) TryGetRigidbody();
                return _rigidbody;
            }
        }

        public Collider Collider
        {
            get
            {
                if (_collider == null) TryGetCollider();
                return _collider;
            }
        }

        public Renderer Renderer
        {
            get
            {
                if (_renderer == null) TryGetRenderer();
                return _renderer;
            }
        }

        public Item Item
        {
            get
            {
                if (_item == null) TryGetItem();
                return _item;
            }
        }

        public ObjectSound ObjectSound
        {
            get
            {
                if (_objectAudio == null) TryGetObjectAudio();
                return _objectAudio;
            }
        }

        #endregion

        #region METHODS
        public void SetObjectInformation(ObjectInformation info)
        {
            _objectInfo = null;
            _objectInfo = info;
        }

        public void SetDisplayName(string displayName)
        {
            _displayName = displayName;
        }

        public void InteractAudio(InteractionCode code)
        {
            Debug.Log($"Play audio by code: {code}");
            switch (code)
            {
                case InteractionCode.Grab: _objectAudio.GrabAudio(); break;
                case InteractionCode.Drop: _objectAudio.DropAudio(); break;
                case InteractionCode.Hold: _objectAudio.HoldAudioStart(); break;
                case InteractionCode.Unhold: _objectAudio.HoldAudioEnd(); break;
                case InteractionCode.Use: _objectAudio.UseAudio(); break;
                case InteractionCode.Error: _objectAudio.InteractAudioError(); break;
                case InteractionCode.Push: _objectAudio.PushAudio(); break;
                case InteractionCode.UseAction: _objectAudio.UseActionAudio(); break;
                case InteractionCode.Action: _objectAudio.ActionAudio(); break;
                case InteractionCode.Equip: _objectAudio.EquipAudio(); break;
                case InteractionCode.Unequip: _objectAudio.UnequipAudio(); break;
                default: break;
            }
        }

        public virtual void ShowInteractMenu()
        {
            if (Renderer == null) return;
            _rect = GUIAroundObejct.GuiRect3DObject(Renderer);

            if (Item != null) _displayName = Item.Info.Name;

            _rect.xMin -= _deltaSize;
            _rect.yMin -= _deltaSize;
        }

        public virtual void HideInteractMenu()
        {
            _rect = new Rect(0, 0, 0, 0);
        }

        public virtual void Hold()
        {
            if (!CanHoldObject) return;

            Debug.Log($"Interact Object {gameObject.name} holded!");
            _eventHold?.Invoke();
        }

        public virtual void Grab(Transform pivot)
        {
            if (!CanGrabObject) return;
            IsGrab = true;
            InventoryPivot = pivot;

            Debug.Log($"Interact Object {gameObject.name} grabed!");
            _eventGrab?.Invoke();
        }

        public virtual void Drop()
        {
            if (!CanGrabObject) return;
            IsGrab = false;
            InventoryPivot = null;

            Debug.Log($"Interact Object {gameObject.name} dropped!");
            _eventDrop?.Invoke();
        }

        public virtual void Use()
        {
            if (!CanUseObject) return;
            if (PlayerInitialization.Instance.InputBinding.IsLockInput) return;
            Debug.Log($"Interaction Object {gameObject.name} used!");
            _eventUse?.Invoke();
        }

        public virtual void UseInInventory()
        {
            if (!CanUseInInventory) return;

            Debug.Log($"Interaction Object {gameObject.name} used in invetory!");
            _eventUseInInventory?.Invoke();
        }

        public virtual void ActionInInventory()
        {
            if (!CanActionObject) return;

            Debug.Log($"Interaction Object {gameObject.name} action!");
            _eventAction?.Invoke();
        }

        public virtual void EquipObject(Transform pivot)
        {
            if (!CanEquipObject) return;
            IsGrab = true;
            InventoryPivot = pivot;

            Debug.Log($"Interaction Object {gameObject.name} equip!");
            _eventEquip?.Invoke();
        }

        public virtual void UnequipObject()
        {
            if (!CanEquipObject) return;
            IsGrab = false;
            InventoryPivot = null;

            Debug.Log($"Interact Object {gameObject.name} unequip!");
            _eventUnequip?.Invoke();
        }

        public virtual void OnScroll(float value)
        {
            if (!CanScroll) return;
            _value += value;

            if (_value > _minMaxValue.y) _value = _minMaxValue.y;
            if (_value < _minMaxValue.x) _value = _minMaxValue.x;

            Debug.Log($"Interaction Object {gameObject.name} scroll value: {_value}");
            transform.rotation = Quaternion.Euler(new(0, _value * _speedRotationMultiplayer, 0));

            _eventScroll?.Invoke();
        }

        protected bool TryGetItem()
        {
            if (_item != null) return true;

            if (TryGetComponent(out Item item))
            {
                _item = item;
                return true;
            }

            return false;
        }

        protected bool TryGetObjectAudio()
        {
            if (_objectAudio != null) return true;

            if (TryGetComponent(out ObjectSound objectAudio))
            {
                _objectAudio = objectAudio;
                return true;
            }

            return false;
        }

        public bool TryGetRigidbody()
        {
            if (_rigidbody != null) return true;

            if (TryGetComponent(out _rigidbody))
            {
                return true;
            }

            return false;
        }

        public bool TryGetRenderer()
        {
            if (_renderer != null) return true;

            if (TryGetComponent(out _renderer))
            {
                return true;
            }

            return false;
        }

        public bool TryGetCollider()
        {
            if (_collider != null) return true;

            if (TryGetComponent(out _collider))
            {
                return true;
            }

            return false;
        }

        public void SetPosition(Vector3 position)
        {
            Debug.Log($"Interaction Object set position {position}");
            if (Rigidbody == null) transform.position = position;
            else Rigidbody.position = position;
        }

        public void SetLocalPosition(Vector3 position, bool forceTransform = false)
        {
            Debug.Log($"Interaction Object set local position {position}");
            if (Rigidbody == null || forceTransform) transform.localPosition = position;
            else Rigidbody.position = position;
        }

        public void TrySetVelocity(Vector3 vector)
        {
            if (Rigidbody == null) return;

            Rigidbody.linearVelocity = vector;
        }

        public virtual void EnableCollision()
        {
            if (Collider == null) return;

            Collider.isTrigger = false;
            Collider.enabled = true;
        }

        public virtual void DisableCollision()
        {
            if (Collider == null) return;

            Collider.isTrigger = true;
            Collider.enabled = false;
        }

        public virtual void EnablePhysic()
        {
            if (Rigidbody == null) return;

            Rigidbody.isKinematic = false;
            Rigidbody.useGravity = true;
            Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        public virtual void DisablePhysic()
        {
            if (Rigidbody == null) return;

            Rigidbody.isKinematic = true;
            Rigidbody.useGravity = false;
            Rigidbody.interpolation = RigidbodyInterpolation.None;
        }

        public void PushDropObject()
        {
            if (!PlayerInitialization.Instance.Inventory.CheckObjectInInventory(Item)) return;
            if (!PlayerInitialization.Instance.GrabObjects.TryDropItem()) return;
            if (Rigidbody == null) return;

            InteractAudio(InteractionCode.Push);
            Rigidbody.AddForce(transform.forward * 10, ForceMode.Impulse);
        }

        public void DebugText(string text)
        {
            Debug.Log(text);
        }

        protected void InventoryPositionAndRotation()
        {
            if (IsGrab && InventoryPivot != null)
            {
                if (SmoothPosition)
                    transform.position = Vector3.Lerp(transform.position, InventoryPivot.position + Item.PositionOffcet, Time.deltaTime * SmoothPositionSpeed);
                if (SmoothRotation)
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(InventoryPivot.eulerAngles + Item.RotationOffcet), Time.deltaTime * SmoothRotationSpeed);
            }
        }

        protected virtual void LocalizeGUI()
        {
            //_textHoldGUI = string.Format(
            //    LocalizationSettings.StringDatabase.GetLocalizedString("InteractionObject", _textKeyHoldGUI), InputBinding.Instance.holdButton);

            //_textGrabGUI = string.Format(
            //    LocalizationSettings.StringDatabase.GetLocalizedString("InteractionObject", _textKeyGrabGUI), InputBinding.Instance.grabButton);

            //_textUseGUI = string.Format(
            //    LocalizationSettings.StringDatabase.GetLocalizedString("InteractionObject", _textKeyUseGUI), InputBinding.Instance.useButton);

            _textHoldGUI = $" [{PlayerInitialization.Instance.InputBinding.holdButton}] {_textKeyHoldGUI}";
            _textGrabGUI = $" [{PlayerInitialization.Instance.InputBinding.grabButton}] {_textKeyGrabGUI}";
            _textUseGUI = $" [{PlayerInitialization.Instance.InputBinding.useButton}] {_textKeyUseGUI}";
            _textEquipGUI = $" [{PlayerInitialization.Instance.InputBinding.equipButton}] {_textKeyEquipGUI}";
        }

        protected bool CheckGravityTrigger()
        {
            if (!CanPhysic) return false;
            bool result = Physics.CheckSphere(transform.position, 1f, WaterLayer);
            if (InvertWaterLayer) return !result;
            return result;
        }
        #endregion

        #region MONOBEHAVIOUR
        protected virtual void Awake()
        {
            Debug.Log($"Initialization Interact Object: {gameObject.name}");
            TryGetRigidbody();
            TryGetCollider();
            TryGetRenderer();
            TryGetItem();
            TryGetObjectAudio();
        }

        protected virtual void Start()
        {
            HideInteractMenu();

            if (IsHold || IsGrab) DisablePhysic();
            if (IsGrab) DisableCollision();
        }

        protected virtual void Update()
        {
            InWater = CheckGravityTrigger();
            InventoryPositionAndRotation();

            if (IsGrab || IsHold) return;
            if (_rigidbody == null) return;
            if (!InWater)
            {
                Rigidbody.useGravity = true;
            }
            else
            {
                Rigidbody.useGravity = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
        }

        protected void OnTriggerExit(Collider other)
        {
        }

        protected virtual void OnGUI()
        {
            if (IsHover && !(IsHold || IsGrab || IsOpen))
            {
                if (PlayerInitialization.Instance.InputBinding.IsMenuOpened) return;

                LocalizeGUI();

                if (_rect.yMin < 25 + _deltaSize + _outlineSize) _rect.yMin = 25 + _deltaSize + _outlineSize;
                if (_rect.yMax >= Screen.height) _rect.yMax = Screen.height - _outlineSize * 2;

                GUI.skin.label.fontSize = 30;
                GUI.skin.label.fontStyle = FontStyle.Bold;

                // Label

                Rect label = _rect;
                //label.x += _deltaSize;
                label.y -= (35 + _outlineSize);
                label.xMax = _rect.xMax + _displayName.Length * 20;
                label.height = 25 + _deltaSize + _outlineSize;
                label.width = (_displayName.Length + 1) * 25;

                GUI.DrawTexture(label, _textureBackground, ScaleMode.StretchToFill, true, 0f);
                GUI.Label(label, $" {_displayName}");

                // Functional

                Rect funcLabel = _rect;

                funcLabel.x = _rect.xMax + _outlineSize;
                funcLabel.height = 50;

                if (CanHoldObject)
                {
                    funcLabel.width = (_textHoldGUI.Length + 1) * 25;
                    GUI.DrawTexture(funcLabel, _textureBackground, ScaleMode.StretchToFill, true, 0f);
                    GUI.Label(funcLabel, _textHoldGUI);
                    funcLabel.y += 50;
                }

                if (CanGrabObject)
                {
                    funcLabel.width = (_textGrabGUI.Length + 1) * 25;
                    GUI.DrawTexture(funcLabel, _textureBackground, ScaleMode.StretchToFill, true, 0f);
                    GUI.Label(funcLabel, _textGrabGUI);
                    funcLabel.y += 50;
                }

                if (CanUseObject)
                {
                    funcLabel.width = (_textUseGUI.Length + 1) * 25;
                    GUI.DrawTexture(funcLabel, _textureBackground, ScaleMode.StretchToFill, true, 0f);
                    GUI.Label(funcLabel, _textUseGUI);
                    funcLabel.y += 50;
                }

                if (CanEquipObject)
                {
                    funcLabel.width = (_textEquipGUI.Length + 1) * 25;
                    GUI.DrawTexture(funcLabel, _textureBackground, ScaleMode.StretchToFill, true, 0f);
                    GUI.Label(funcLabel, _textEquipGUI);
                    funcLabel.y += 50;
                }
                // Box

                Rect topLine = _rect;
                Rect bottomLine = _rect;
                Rect leftLine = _rect;
                Rect rightLine = _rect;

                topLine.height = _outlineSize;
                topLine.width += _outlineSize;
                bottomLine.height = _outlineSize;
                bottomLine.width += _outlineSize;
                bottomLine.y += _rect.height;

                GUI.DrawTexture(topLine, _textureOutline, ScaleMode.StretchToFill, true, 1f);
                GUI.DrawTexture(bottomLine, _textureOutline, ScaleMode.StretchToFill, true, 1f);

                leftLine.width = _outlineSize;
                leftLine.height += _outlineSize;
                rightLine.width = _outlineSize;
                rightLine.height += _outlineSize;
                rightLine.x += _rect.width;

                GUI.DrawTexture(leftLine, _textureOutline, ScaleMode.StretchToFill, true, 1f);
                GUI.DrawTexture(rightLine, _textureOutline, ScaleMode.StretchToFill, true, 1f);
            }
        }
        #endregion
    }
}