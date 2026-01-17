using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ZFGinc.Objects;
using ZFGinc.Player;

namespace ZFGinc.InventoryItems
{
    public class Inventory : MonoBehaviour
    {
        public event Action<int> OnRedrawInventory;
        [SerializeField] private GameObject _cameraForItems;
        [Space(30)]

        [Header("Parameters")]
        [SerializeField] private int _copacityInventory = 4;
        [SerializeField] private int _copacityEquipment = 3;
        [Space]
        [SerializeField] private List<Item> _items = new();
        [SerializeField] private List<Item> _equipment = new();
        [SerializeField, Layer] private int _layerMaskItems;

        [Space(15)]
        [Header("Interface")]
        [SerializeField] private GameObject _prefabCell;
        [SerializeField] private Transform _pivotForCellsInventory;
        [SerializeField] private Transform _pivotForCellsEquipment;
        [Space(15)]
        [SerializeField] private TMP_Text _itemNameText;

        private List<CellInventory> _cellsIventory = new();
        private List<CellInventory> _cellsEquipment = new();
        private int _currentIndexItem = 0;

        private InputBinding _inputBinding;
        private HoldObjects _holdObjects;

        private Coroutine _showItemName;

        public int Count => _items.Count;
        public int Copacity => _copacityInventory;
        public List<Item> Items => _items;
        public LayerMask LayerMaskItems => _layerMaskItems;
        public Item CurrentItem => _currentIndexItem >= _items.Count ? null : _items[_currentIndexItem];
        public InteractObject CurrentObject => CurrentItem == null ? null : CurrentItem.Object;

        public void Initialize(InputBinding inputBinding, HoldObjects holdObjects)
        {
            _inputBinding = inputBinding;
            _holdObjects = holdObjects;

            _inputBinding.OnMouseScrollWheel += OnMouseScrollWheel;
            _inputBinding.OnNumberButtonDown += OnSelectedNumberSlotInventory;

            InitializeInventory();
        }

        public bool TryAddItemInventory(Item item)
        {
            Debug.Log("Try add item in inventory: " + item.Info.Name);
            if (item == null)
            {
                Debug.Log("Item is null");
                return false;
            }
            if (_items.Count + 1 > _copacityInventory)
            {
                Debug.Log("Inventory copacity received");
                return false;
            }

            Debug.Log("Item added in inventory");
            _items.Add(item);

            if (item.Info.ForTwoHands)
                _currentIndexItem = _items.Count - 1;

            item.SetLayer(LayerMaskItems);

            ReDrawInventory();

            return true;
        }

        public bool TryAddItemEquipment(Item item)
        {
            Debug.Log("Try add item in equipment: " + item.Info.Name);
            if (item == null)
            {
                Debug.Log("Item is null");
                return false;
            }
            if (_equipment.Count + 1 > _copacityEquipment)
            {
                Debug.Log("Equipment copacity received");
                return false;
            }

            Debug.Log("Item added in inventory");
            _equipment.Add(item);

            ReDrawEquipment();

            return true;
        }

        public bool TryRemoveItemInventory()
        {
            Debug.Log("Try remove current item: " + CurrentItem.Info.Name);
            if (CurrentItem == null)
            {
                Debug.Log("Item not selected in inventory");
                return false;
            }

            CurrentItem.ResetLayer();

            _items.RemoveAt(_currentIndexItem);
            Debug.Log("Item removerd from inventory");

            ReDrawInventory();

            return true;
        }

        public bool TryRemoveItemEquipment(int i)
        {
            Item equip = _equipment[i];
            Debug.Log("Try remove current item: " + equip.Info.Name);
            if (equip == null)
            {
                Debug.Log("Item not selected in inventory");
                return false;
            }

            _equipment.RemoveAt(i);
            Debug.Log("Item removerd from inventory");

            ReDrawEquipment();

            return true;
        }

        public Item GetEquipment(int index) => index >= _equipment.Count ? null : _equipment[index];

        public void SelectItem()
        {
            Debug.Log("Start redraw inventory");
            for (int i = 0; i < _cellsIventory.Count; i++)
            {
                _cellsIventory[i].SelectedItem(i == _currentIndexItem);
            }

            ShowItemName();
        }

        private void ShowItemName()
        {
            if (_showItemName != null) StopCoroutine(_showItemName);

            _showItemName = StartCoroutine(ShowItemNameCoroutine());
        }

        public void UseInventoryItem()
        {
            if (CurrentObject == null) return;
            Debug.Log("Use item in inventory");
            CurrentObject.UseInInventory();
        }

        public void ActionInventoryItem()
        {
            if (CurrentObject == null) return;
            Debug.Log("Action item in inventory");
            CurrentObject.ActionInInventory();
        }

        public bool CheckObjectInInventory(Item item)
        {
            if (item == null) return false;
            return _items.Contains(item);
        }

        private IEnumerator ShowItemNameCoroutine()
        {
            yield return null;

            float cooldown = .065f;
            Color color = _itemNameText.color;
            color.a = 0;

            yield return null;

            _itemNameText.color = color;
            _itemNameText.text = CurrentItem == null ? "" : CurrentItem.Info.Name;
            Debug.Log("Show item name: " + _itemNameText.text);

            for (int i = 0; i < 20; i++)
            {
                yield return new WaitForSeconds(cooldown);
                color.a += 0.05f;
                _itemNameText.color = color;
            }

            color.a = 1;
            yield return null;

            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < 20; i++)
            {
                yield return new WaitForSeconds(cooldown);
                color.a -= 0.05f;
                _itemNameText.color = color;
            }

            color.a = 0;
            yield return null;
        }

        private void OnSelectedNumberSlotInventory(int value)
        {
            if (value > 6 && value < 10) return;
            if (value >= _copacityInventory) return;
            if (CurrentItem && CurrentItem.Info.ForTwoHands) return;

            _currentIndexItem = value;

            ReDrawInventory();
        }

        private void OnMouseScrollWheel(int value)
        {
            if (_holdObjects.IsHold) return;
            if (CurrentItem && CurrentItem.Info.ForTwoHands) return;

            _currentIndexItem -= value;

            if (_currentIndexItem < 0) _currentIndexItem = _copacityInventory - 1;
            if (_currentIndexItem >= _copacityInventory) _currentIndexItem = 0;

            ReDrawInventory();
        }

        public void InitializeInventory()
        {
            CreateNewInventoryCells();

            ReDrawInventory();
            ReDrawEquipment();
        }

        private void ReDrawInventory()
        {
            Debug.Log("Start redraw inventory");

            ShowItemsInInventory();

            SelectItem();

            ReDrawInventoryObjects();

            Debug.Log("Invoke event OnRedrawInventory");
            OnRedrawInventory?.Invoke(_currentIndexItem);

            Debug.Log("End redraw inventory");

            _cameraForItems.SetActive(CurrentItem != null);
        }

        private void ReDrawInventoryObjects()
        {
            Debug.Log("Redraw items object");
            if (_items.Count == 0) return;

            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].ChangeVisible(i == _currentIndexItem);
            }
        }

        private void CreateNewInventoryCells()
        {
            Debug.Log("Create new inventory cells");
            for (int i = 0; i < _copacityInventory; i++)
            {
                Transform cell = Instantiate(_prefabCell, _pivotForCellsInventory, false).transform;
                cell.localScale = Vector3.one;
                _cellsIventory.Add(cell.GetComponent<CellInventory>());
            }
            for (int i = 0; i < _copacityEquipment; i++)
            {
                Transform cell = Instantiate(_prefabCell, _pivotForCellsEquipment, false).transform;
                cell.localScale = Vector3.one;
                _cellsEquipment.Add(cell.GetComponent<CellInventory>());
            }
        }

        private void ShowItemsInInventory()
        {
            Debug.Log("Show items in slots");
            for (int i = 0; i < _cellsIventory.Count; i++)
            {
                _cellsIventory[i].ResetImageItem();
                _cellsIventory[i].SetNewKey((i + 1).ToString());
            }

            for (int i = 0; i < _items.Count; i++)
            {
                CellInventory cell = _cellsIventory[i];
                cell.SetImageItem(_items[i].Info.Icon);
                cell.ActiveSelfItemImage(true);
            }
        }

        private void ReDrawEquipment()
        {
            Debug.Log("Show items in slots");
            for (int i = 0; i < _cellsEquipment.Count; i++)
            {
                _cellsEquipment[i].ResetImageItem();
                _cellsEquipment[i].SetNewKey((i + 7).ToString());
            }

            for (int i = 0; i < _equipment.Count; i++)
            {
                CellInventory cell = _cellsEquipment[i];
                cell.SetImageItem(_equipment[i].Info.Icon);
                cell.ActiveSelfItemImage(true);
            }
        }
    }
}