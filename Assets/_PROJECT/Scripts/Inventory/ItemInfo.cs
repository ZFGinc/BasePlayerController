using NaughtyAttributes;
using System;
using UnityEngine;

namespace ZFGinc.InventoryItems
{
    [Serializable]
    [CreateAssetMenu(fileName = "Item", menuName = "Inventoy/New Item")]
    public class ItemInfo : ScriptableObject
    {
        [Header("Properties")]
        //public string KeyLocalization;
        public string Name;
        public string Description;

        [Space]
        public int Price = 30;
        public int Count = 1;
        public GameObject Prefab;

        [Space(30)]
        [ShowAssetPreview(96, 96)]
        public Sprite Icon;

        public string PrefabName => Prefab.name;
        //public string Name => LocalizationSettings.StringDatabase.GetLocalizedString("Items", KeyLocalization + "_NAME");
        //public string Description => LocalizationSettings.StringDatabase.GetLocalizedString("Items", KeyLocalization + "_DESC");
    }
}