using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZFGinc.InventoryItems
{
    public class CellInventory : MonoBehaviour
    {
        [SerializeField] private bool _isSelected = false;

        [Space(15)]
        [SerializeField] private Image _imageItem;
        [SerializeField] private GameObject _selectedSquare;
        [SerializeField] private TMP_Text _key;

        public void SetImageItem(Sprite sprite)
        {
            _imageItem.sprite = sprite;
            if (sprite != null) _imageItem.gameObject.SetActive(true);
        }

        public void ResetImageItem()
        {
            _imageItem.sprite = null;
            ActiveSelfItemImage(false);
            SelectedItem(false);
        }

        public void SetNewKey(string key)
        {
            _key.text = key;
        }

        public void ActiveSelfItemImage(bool value)
        {
            _imageItem.gameObject.SetActive(value);
        }

        public void SelectedItem(bool value)
        {
            _isSelected = value;
            _selectedSquare.SetActive(value);
        }
    }
}