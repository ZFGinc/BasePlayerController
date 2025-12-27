using NaughtyAttributes;
using System;
using UnityEngine;

namespace ZFGinc.Objects
{
    [Serializable]
    [CreateAssetMenu(fileName = "Object", menuName = "Objects/New Object")]
    public class ObjectInformation : ScriptableObject
    {
        public bool CanHoldObject = false;
        public bool CanRotateObject = false;
        public bool CanMoveObject = false;
        public bool CanScroll = false;
        public Vector3 AcessVectorRotate = Vector3.one;
        [Space]
        public bool CanGrabObject = false;
        public bool CanEquipObject = false;
        [ShowIf(nameof(CanEquipObject))]
        public EquipmentType EquipmentType = EquipmentType.Null;
        [Space]
        public bool CanUseObject = false;
        public bool CanActionObject = false;
        public bool CanUseInInventory = false;
    }
}