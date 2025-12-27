using UnityEngine;
using ZFGinc.InventoryItems;
using ZFGinc.Utils;

namespace ZFGinc.Player
{
    public class PlayerInitialization : MonoBehaviour
    {
        public bool InitializeOnStart = true;
        public bool IsInit = false;
        [Space]
        public GameObject UI;
        public GameObject Camera;
        public GameObject Skin;
        [Space]
        public InputBinding InputBinding;
        public CharacterInput CharacterInput;
        public CharacterMovement CharacterMovement;
        public FirstPersonCharacter FirstPersonCharacter;
        public FirstPersonCharacterLookInput FirstPersonCharacterLookInput;
        public Inventory Inventory;
        public LookingObjectRay LookingObjectRay;
        public HoldObjects HoldObjects;
        public GrabObjects GrabObjects;
        public UseObjects UseObjects;
        public EquipObjects EquipObjects;
        public MovementAudio MovementAudio;
        public ZoomAbility ZoomAbility;
        public OpenClosePauseMenu OpenClosePauseMenu;
        public OnTriggerListener OnTriggerListener;
        public AnimationController AnimationController;

        public static PlayerInitialization Instance = null;

        private void Awake()
        {
            if (!InitializeOnStart) return;

            if (Instance == null) Instance = this;
            else
            {
                Destroy(Instance.gameObject);
                Instance = this;
            }
        }

        private void Start()
        {
            if (!InitializeOnStart) return;

            UI.SetActive(true);
            UI.transform.parent = null;
            Camera.SetActive(true);
            Skin.SetActive(false);

            InputBinding.enabled = true;
            CharacterInput.enabled = true;
            CharacterMovement.enabled = true;
            FirstPersonCharacter.enabled = true;
            FirstPersonCharacterLookInput.enabled = true;
            Inventory.enabled = true;
            LookingObjectRay.enabled = true;
            HoldObjects.enabled = true;
            GrabObjects.enabled = true;
            UseObjects.enabled = true;
            EquipObjects.enabled = true;
            MovementAudio.enabled = true;

            ZoomAbility.enabled = true;
            OpenClosePauseMenu.enabled = true;
            OnTriggerListener.enabled = true;
            AnimationController.enabled = true;

            CharacterInput.Initialize(FirstPersonCharacter, InputBinding);
            FirstPersonCharacterLookInput.Initialize(FirstPersonCharacter, InputBinding);
            Inventory.Initialize(InputBinding, HoldObjects);
            LookingObjectRay.Initialize(InputBinding);
            GrabObjects.Initialize(InputBinding, LookingObjectRay, Inventory);
            HoldObjects.Initialize(InputBinding, LookingObjectRay, FirstPersonCharacter);
            UseObjects.Initialize(InputBinding, LookingObjectRay);
            EquipObjects.Initialize(InputBinding, LookingObjectRay, Inventory);
            MovementAudio.Initialize(FirstPersonCharacter, CharacterMovement);

            IsInit = true;
        }
    }
}