using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

enum MenuState
{
    Root,
    Browsing,
    EquipPopup
}

public class MenuController : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject equipIzquierdaContainer;
    public GameObject equipDerechaContainer;
    public GameObject descriptionContainer;
    public Transform menuContainer;
    public Transform itemContainer;
    public GameObject categoryPrefab;
    public GameObject itemPrefab;
    public GameObject lightContainer;
    public Sprite lightOnSprite;
    public Sprite lightOffSprite;
    public Transform popupCanvasRoot;

    [Header("Referencias Combate")]
    [SerializeField] private PlayerControllerV2 playerController;

    [Header("Prefab para minimenú de dirección")]
    public GameObject equipDirectionMenuPrefab;

    [Header("Configuración")]
    public Color selectedColor = Color.yellow;
    public Color unselectedColor = Color.green;

    private List<GameObject> dynamicCategories = new();
    private List<GameObject> dynamicItems = new();

    private MenuState menuState = MenuState.Root;

    private readonly string[] horizontalTabs = {
        "Mano izquierda", "Mano derecha",
        "Todos", "Llaves", "Armas", "Ropa"
    };

    private List<InventoryItem> visibleItems = new();
    private int selectedCategoryIndex = 0;
    private int selectedItemIndex = 0;
    private int startIndex = 0;
    private const int maxVisibleItems = 8;

    private const string defaultDescriptionText = "Selecciona un ítem para ver su descripción.";

    private TextMeshProUGUI equipIzqText;
    private TextMeshProUGUI equipDerText;
    private TextMeshProUGUI descriptionText;

    private bool isInventoryOpen = false;
    public bool IsInventoryOpen => isInventoryOpen;

    private bool isBrowsing = false;
    
    private bool inCombatMode = false;

    private GameObject equipDirectionMenuInstance;
    private InventoryItem itemPendingEquip;

    private void Awake()
    {
        equipIzqText = equipIzquierdaContainer.GetComponentInChildren<TextMeshProUGUI>();
        equipDerText = equipDerechaContainer.GetComponentInChildren<TextMeshProUGUI>();
        descriptionText = descriptionContainer.GetComponentInChildren<TextMeshProUGUI>();
        playerController = FindFirstObjectByType<PlayerControllerV2>();
    }

    void Start()
    {
        ResetMenu();
        //combatSelectorController.OnStopCombatMode += () => SetCombatMode(false);
    }

    private void UpdateVisibleItems()
    {
        visibleItems.Clear();

        if (selectedCategoryIndex < 2)
            return;

        string category = horizontalTabs[selectedCategoryIndex];

        if (category == "Todos")
            visibleItems = Inventory.Instance.GetAllItems();
        else if (category == "Llaves")
            visibleItems = Inventory.Instance.GetKeyItems();
        else if (category == "Armas")
            visibleItems = Inventory.Instance.GetAllItems().FindAll(i => i.itemData is WeaponSO);
        else if (category == "Ropa")
            visibleItems = Inventory.Instance.GetAllItems().FindAll(i => i.itemData is ClothingSO);
    }

    private void RedrawMenu()
    {
        ClearList(dynamicCategories);
        ClearList(dynamicItems);

        SetTabText(equipIzqText, Inventory.Instance.leftHandItem?.itemData.itemName ?? "Mano Izquierda", menuState == MenuState.Root && selectedCategoryIndex == 0);
        SetTabText(equipDerText, Inventory.Instance.rightHandItem?.itemData.itemName ?? "Mano Derecha", menuState == MenuState.Root && selectedCategoryIndex == 1);

        for (int i = 2; i < horizontalTabs.Length; i++)
        {
            GameObject go = Instantiate(categoryPrefab, menuContainer);
            var text = go.GetComponentInChildren<TextMeshProUGUI>();
            text.text = horizontalTabs[i];
            text.color = (menuState == MenuState.Root && selectedCategoryIndex == i) ? selectedColor : unselectedColor;
            dynamicCategories.Add(go);
        }

        if (menuState == MenuState.Browsing)
        {
            int endIndex = Mathf.Min(startIndex + maxVisibleItems, visibleItems.Count);
            for (int i = startIndex; i < endIndex; i++)
            {
                GameObject go = Instantiate(itemPrefab, itemContainer);
                var text = go.GetComponentInChildren<TextMeshProUGUI>();
                var currentItem = visibleItems[i];

                bool isEquipped = Inventory.Instance.leftHandItem?.itemData == currentItem.itemData
                               || Inventory.Instance.rightHandItem?.itemData == currentItem.itemData
                               || Inventory.Instance.currentClothingItem == currentItem.itemData;

                text.text = currentItem.itemData.itemName + (isEquipped ? " (equipado)" : "");
                text.color = (i == selectedItemIndex) ? selectedColor : unselectedColor;
                dynamicItems.Add(go);
            }
        }
    }

    private void SetTabText(TextMeshProUGUI textComponent, string text, bool selected)
    {
        textComponent.text = text;
        textComponent.color = selected ? selectedColor : unselectedColor;
    }

    private void ClearList(List<GameObject> list)
    {
        foreach (var go in list)
            Destroy(go);
        list.Clear();
    }

    public void OnNavigateHorizontal(int direction)
    {
        if (inCombatMode || menuState != MenuState.Root) return;

        selectedCategoryIndex = (selectedCategoryIndex + direction + horizontalTabs.Length) % horizontalTabs.Length;
        selectedItemIndex = 0;
        startIndex = 0;

        UpdateVisibleItems();
        RedrawMenu();
        ClearDescription();
    }

    public void OnNavigateVertical(int direction)
    {
        if (inCombatMode) return;

        if (menuState == MenuState.EquipPopup)
        {
            if (direction < 0)
                EquipToLeftHand(itemPendingEquip);
            else if (direction > 0)
                EquipToRightHand(itemPendingEquip);

            CloseEquipDirectionMenu();
            return;
        }

        if (selectedCategoryIndex < 2) return;

        if (menuState == MenuState.Root)
        {
            if (visibleItems.Count > 0)
            {
                menuState = MenuState.Browsing;
                selectedItemIndex = 0;
                startIndex = 0;
                UpdateDescription(selectedItemIndex);
                RedrawMenu();
            }
        }
        else if (menuState == MenuState.Browsing)
        {
            selectedItemIndex = Mathf.Clamp(selectedItemIndex + direction, 0, visibleItems.Count - 1);

            if (selectedItemIndex < startIndex)
                startIndex = selectedItemIndex;
            else if (selectedItemIndex >= startIndex + maxVisibleItems)
                startIndex = selectedItemIndex - maxVisibleItems + 1;

            UpdateDescription(selectedItemIndex);
            RedrawMenu();
        }
    }

    private void UpdateDescription(int itemIndex)
    {
        if (itemIndex >= 0 && itemIndex < visibleItems.Count)
            descriptionText.text = visibleItems[selectedItemIndex].itemData.description;
        else
            ClearDescription();
    }

    private void ClearDescription()
    {
        descriptionText.text = defaultDescriptionText;
    }

    public void Submit()
    {
        if (inCombatMode) return;

        if (menuState == MenuState.Browsing)
        {
            var selectedItem = visibleItems[selectedItemIndex];

            if (selectedItem.itemData is WeaponSO)
            {
                ShowEquipDirectionMenu(selectedItem);
            }
            else if (selectedItem.itemData is ClothingSO clothingSo)
            {
                if (Inventory.Instance.currentClothingItem == clothingSo)
                {
                    Debug.Log("Este ítem ya está equipado como ropa.");
                }
                else
                {
                    if (Inventory.Instance.currentClothingItem != null)
                        Inventory.Instance.currentClothingItem.isInUse = false;

                    Inventory.Instance.currentClothingItem = clothingSo;
                    clothingSo.isInUse = true;
                    Debug.Log($"Equipado como ropa: {clothingSo.itemName}");
                    RedrawMenu();
                }
            }
            else
            {
                Debug.Log("Ítem no equipable directamente.");
            }
        }
        else
        {
            if (selectedCategoryIndex < 2)
            {
                InventoryItem equippedItem = selectedCategoryIndex == 0 ? Inventory.Instance.leftHandItem : Inventory.Instance.rightHandItem;

                if (equippedItem != null && equippedItem.itemData is WeaponSO weapon)
                {
                    Vector2Int playerGridPos = PlayerControllerV2.Instance.GridPosition;

                    playerController.combatSelector.EnterCombatMode(playerGridPos, weapon);
                    playerController.EnterCombatMode();

                    Debug.Log($"Modo combate iniciado con {weapon.itemName}");
                }
                else
                {
                    Debug.Log("No hay arma equipada en esta mano para iniciar combate.");
                }
            }
            else if (visibleItems.Count > 0)
            {
                menuState = MenuState.Browsing;
                selectedItemIndex = 0;
                startIndex = 0;
                UpdateDescription(selectedItemIndex);
                RedrawMenu();
            }
        }
    }

    public void Cancel()
    {
        if (inCombatMode) return;

        switch (menuState)
        {
            case MenuState.Root:
                isInventoryOpen = false;
                ClearDescription();
                ExitInventory();
                var input = playerController.GetComponent<PlayerInput>();
                if (input != null)
                    input.SwitchCurrentActionMap("Player");

                break;
            case MenuState.EquipPopup:
                CloseEquipDirectionMenu();
                break;
            
            case MenuState.Browsing:
                menuState = MenuState.Root;
                //isBrowsing = false;
                selectedItemIndex = 0;
                startIndex = 0;
                UpdateVisibleItems();
                RedrawMenu();
                ClearDescription();
                break;
        }
    }

    public void ResetMenu()
    {
        isInventoryOpen = true;
        selectedCategoryIndex = 0;
        selectedItemIndex = 0;
        startIndex = 0;
        menuState = MenuState.Root;
        UpdateVisibleItems();
        RedrawMenu();
        ClearDescription();
    }

    private void ShowEquipDirectionMenu(InventoryItem item)
    {
        if (equipDirectionMenuInstance != null)
            Destroy(equipDirectionMenuInstance);

        Vector3 spawnPos = dynamicItems[selectedItemIndex].transform.position;
        equipDirectionMenuInstance = Instantiate(equipDirectionMenuPrefab, popupCanvasRoot);
        equipDirectionMenuInstance.transform.position = spawnPos;

        itemPendingEquip = item;
        menuState = MenuState.EquipPopup;
    }

    public void CloseEquipDirectionMenu()
    {
        if (equipDirectionMenuInstance != null)
        {
            Destroy(equipDirectionMenuInstance);
            equipDirectionMenuInstance = null;
        }

        itemPendingEquip = null;
        menuState = MenuState.Browsing;
        RedrawMenu();
    }

    private void EquipToLeftHand(InventoryItem item)
    {
        if (!(item.itemData is WeaponSO)) return;
        if (Inventory.Instance.leftHandItem == item || Inventory.Instance.rightHandItem == item) return;

        if (Inventory.Instance.leftHandItem != null)
            Inventory.Instance.leftHandItem.itemData.isInUse = false;

        Inventory.Instance.leftHandItem = item;
        item.itemData.isInUse = true;
        RedrawMenu();
    }

    private void EquipToRightHand(InventoryItem item)
    {
        if (!(item.itemData is WeaponSO)) return;
        if (Inventory.Instance.rightHandItem == item || Inventory.Instance.leftHandItem == item) return;

        if (Inventory.Instance.rightHandItem != null)
            Inventory.Instance.rightHandItem.itemData.isInUse = false;

        Inventory.Instance.rightHandItem = item;
        item.itemData.isInUse = true;
        RedrawMenu();
    }

    public void EnterInventory()
    {
        equipIzqText.text = Inventory.Instance.leftHandItem?.itemData.itemName ?? "Mano Izquierda";
        equipDerText.text = Inventory.Instance.rightHandItem?.itemData.itemName ?? "Mano Derecha";
        lightContainer.GetComponent<Image>().sprite = lightOnSprite;
        isInventoryOpen = true;
    }

    public void ExitInventory()
    {
        lightContainer.GetComponent<Image>().sprite = lightOffSprite;
        isInventoryOpen = false;
    }

    public bool IsAtRootLevel() => menuState == MenuState.Root;
    public bool IsBrowsingItems() => menuState == MenuState.Browsing;
    public bool IsShowingEquipPopup() => menuState == MenuState.EquipPopup;
}
