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
        [SerializeField] private GameObject _preview;

        public void SetImageItem(Sprite sprite)
        {
            _imageItem.sprite = sprite;
            _preview.SetActive(false);
            if (sprite != null) _imageItem.gameObject.SetActive(true);
        }

        public void ResetImageItem()
        {
            _imageItem.sprite = null;
            _preview.SetActive(true);
            ActiveSelfItemImage(false);
            SelectedItem(false);
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