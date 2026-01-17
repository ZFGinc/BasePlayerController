using System;
using System.Reflection;
using UnityEngine;
using ZFGinc.Player.Utils;

namespace ZFGinc.Player
{
    [Serializable]
    public enum MouseAxis : int
    {
        [StringValue("Mouse X")]
        MouseX = 1,
        [StringValue("Mouse Y")]
        MouseY = 2,
        [StringValue("Mouse ScrollWheel")]
        MouseScrollWheel = 3
    }

    public class InputBinding : MonoBehaviour
    {
        public KeyCode escapeButton = KeyCode.Escape;

        public KeyCode moveForwardButton = KeyCode.W;
        public KeyCode moveBackwardButton = KeyCode.S;
        public KeyCode moveLeftButton = KeyCode.A;
        public KeyCode moveRightButton = KeyCode.D;
        public KeyCode sprintButton = KeyCode.LeftShift;
        public KeyCode crouchButton = KeyCode.LeftControl;
        public KeyCode jumpButton = KeyCode.Space;

        public MouseAxis mouseX = MouseAxis.MouseX;
        public MouseAxis mouseY = MouseAxis.MouseY;
        [Range(0.1f, 3f)] public float mouseSensitivityX = 1.2f;
        [Range(0.1f, 3f)] public float mouseSensitivityY = 1.2f;
        public bool inversionX = false;
        public bool inversionY = false;
        public KeyCode zoomButton = KeyCode.Z;

        public KeyCode useButton = KeyCode.R;
        public KeyCode exitMenuKey = KeyCode.Tilde;
        public KeyCode holdButton = KeyCode.E;
        public KeyCode grabButton = KeyCode.F;
        public KeyCode equipButton = KeyCode.T;
        public KeyCode dropButton = KeyCode.G;
        public KeyCode reloadButton = KeyCode.R;
        public KeyCode LCM = KeyCode.Mouse0;
        public KeyCode RCM = KeyCode.Mouse1;
        public KeyCode WCM = KeyCode.Mouse2;
        public MouseAxis mouseScroll = MouseAxis.MouseScrollWheel;

        public KeyCode slot1 = KeyCode.Alpha1;
        public KeyCode slot2 = KeyCode.Alpha2;
        public KeyCode slot3 = KeyCode.Alpha3;
        public KeyCode slot4 = KeyCode.Alpha4;
        public KeyCode slot5 = KeyCode.Alpha5;
        public KeyCode slot6 = KeyCode.Alpha6;
        public KeyCode slot7 = KeyCode.Alpha7;
        public KeyCode slot8 = KeyCode.Alpha8;
        public KeyCode slot9 = KeyCode.Alpha9;

        public bool IsMenuOpened = false;
        public bool IsLockInput = false;
        public bool IsCanZoom = true;

        public event Action<int> OnMouseScrollWheel;
        public event Action<int> OnNumberButtonDown;

        public Vector2 GetMovementDirection()
        {
            if (IsMenuOpened || IsLockInput) return Vector2.zero;

            float left = Input.GetKey(moveLeftButton) ? -1 : 0;
            float right = Input.GetKey(moveRightButton) ? 1 : 0;
            float forward = Input.GetKey(moveForwardButton) ? 1 : 0;
            float backward = Input.GetKey(moveBackwardButton) ? -1 : 0;

            float horizontal = left + right;
            float vertical = backward + forward;

            return new Vector2(horizontal, vertical).normalized;
        }

        public Vector2 GetMouseDirection()
        {
            if (IsMenuOpened || IsLockInput) return Vector2.zero;

            float x = Input.GetAxisRaw(GetEnumName(mouseX)) * (inversionX ? -1 : 1) * mouseSensitivityX;
            float y = Input.GetAxisRaw(GetEnumName(mouseY)) * (inversionY ? -1 : 1) * mouseSensitivityY;

            return new Vector2(x, y);
        }

        private void Update()
        {
            if (IsMenuOpened) return;

            CheckInventoryActions();
            CheckScrollWheel();
            CheckInputNumber();
        }

        private void CheckScrollWheel()
        {
            if (IsMenuOpened || IsLockInput) return;

            if (Input.GetAxis(GetEnumName(mouseScroll)) > 0f) // forward
            {
                OnMouseScrollWheel?.Invoke(1);
            }
            else if (Input.GetAxis(GetEnumName(mouseScroll)) < 0f) // backwards
            {
                OnMouseScrollWheel?.Invoke(-1);
            }
        }

        private void CheckInputNumber()
        {
            if (IsMenuOpened || IsLockInput) return;

            if (Input.GetKeyDown(slot9))
            {
                Debug.Log("Pressed: " + slot9.ToString()); OnNumberButtonDown?.Invoke(8);
            }
            if (Input.GetKeyDown(slot8))
            {
                Debug.Log("Pressed: " + slot8.ToString()); OnNumberButtonDown?.Invoke(7);
            }
            if (Input.GetKeyDown(slot7))
            {
                Debug.Log("Pressed: " + slot7.ToString()); OnNumberButtonDown?.Invoke(6);
            }
            if (Input.GetKeyDown(slot6))
            {
                Debug.Log("Pressed: " + slot6.ToString()); OnNumberButtonDown?.Invoke(5);
            }
            if (Input.GetKeyDown(slot5))
            {
                Debug.Log("Pressed: " + slot5.ToString()); OnNumberButtonDown?.Invoke(4);
            }
            if (Input.GetKeyDown(slot4))
            {
                Debug.Log("Pressed: " + slot4.ToString()); OnNumberButtonDown?.Invoke(3);
            }
            if (Input.GetKeyDown(slot3))
            {
                Debug.Log("Pressed: " + slot3.ToString()); OnNumberButtonDown?.Invoke(2);
            }
            if (Input.GetKeyDown(slot2))
            {
                Debug.Log("Pressed: " + slot2.ToString()); OnNumberButtonDown?.Invoke(1);
            }
            if (Input.GetKeyDown(slot1))
            {
                Debug.Log("Pressed: " + slot1.ToString()); OnNumberButtonDown?.Invoke(0);
            }
        }

        private void CheckInventoryActions()
        {
            if (IsMenuOpened || IsLockInput) return;

            if (Input.GetKeyDown(LCM))
            {
                Debug.Log("Pressed: " + LCM.ToString());
                //PlayerInitialization.Instance.Inventory.ActionInventoryItem();
            }
            if (Input.GetKeyDown(RCM))
            {
                Debug.Log("Pressed: " + RCM.ToString());
                //PlayerInitialization.Instance.Inventory.UseInventoryItem();
            }
            if (Input.GetKeyDown(WCM))
            {
                Debug.Log("Pressed: " + WCM.ToString());
                return;
            }
        }

        private string GetEnumName(Enum enumVal)
        {
            MemberInfo[] memInfo = enumVal.GetType().GetMember(enumVal.ToString());
            StringValueAttribute attribute = CustomAttributeExtensions.GetCustomAttribute<StringValueAttribute>(memInfo[0]);
            return attribute.StringValue;
        }
    }
}