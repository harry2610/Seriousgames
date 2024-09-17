using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Linq;
using System;


/// <summary>
/// Controls the inventory UI elements.
/// Must have if you use the inventory bar in your UI.
/// </summary>
public class UIControllerInventory : MonoBehaviour
{
    private VisualElement m_BottomContainer; // The container for the bottom UI elements.
    private Button m_OpenButton; // The button to open the bottom sheet.
    private Button m_CloseButton; // The button to close the bottom sheet.
    private Button m_CloseButton2; // The button to close the bottom sheet (used for clicking outside the bottom sheet).
    private VisualElement m_BottomSheet; // The visual element representing the bottom sheet.
    private VisualElement m_Scrim; // The visual element representing the scrim overlay.
    private Label m_CoinAmountPopout; // The label for displaying the coin amount.
    private Button m_QuizButton; // The button to open the quiz.
    private Label m_timeForCoins; // The label for displaying the time until the next coins.
    private VisualElement m_ItemInfoContainer; // The container for the item info box.
    private VisualElement m_ItemInfoIcon; // The icon for the item info box.
    private Label m_ItemInfoName; // The name label for the item info box.
    private Label[] m_ItemInfoStats = new Label[6]; // The array of item stats for the item info box.
    private Button[] m_Items = new Button[20]; // The array of item buttons for the big inventory.
    private Button m_DraggedItem; // The button that is used when dragging an item to show the dragged item.
    private Dictionary<Button, int> m_ItemsDict = new Dictionary<Button, int>(); // Maps the item buttons to their slot.
    private Button[] m_RItems = new Button[5]; // The array of item buttons for the small inventory.
    private Label[] m_RItemAmounts = new Label[5]; // The array of item amounts for the small inventory.
    private List<ItemSO> m_PlacedInventoryItems = new List<ItemSO>(); // The list of inventory items.
    private VisualElement m_Panel; // The panel for the General UI.
    private VisualElement m_ItemUsedPanel; // The panel for the used items.
    private VisualElement m_ItemUsedBackButton; // The back button for the used items.
    private VisualElement m_ItemCameraControlsPanel; // The Panel containing camera controls 
    private Button m_CameraButtonLeft; // The button to rotate the camera left in focus mode
    private Button m_CameraButtonRight; // The button to rotate the camera right in focus mode

    /// <summary>
    /// The inventory list ScriptableObject.
    /// </summary>
    public PlayerInventoryListSO inventoryList;

    public QuicktipSO tipForThrowables; // The quick tip for throwables.
    public QuicktipSO tipForPlaceables; // The quick tip for placeables.
    public QuicktipSO tipForConsumables; // The quick tip for consumables.
    public QuicktipSO tipForNoBowlPlaced; // The quick tip for no bowl placed.
    public QuicktipSO tipForPooDeleter; // The quick tip for poo deleter.
    public QuicktipSO tipForNoPoo; // The quick tip for no poo.
    public QuicktipSO tipForInventoryButton; // The quick tip for the inventory button.
    public QuicktipSO tipForFirstStart; // The quick tip for the first start.
    public QuicktipSO tipForScissor; // The quick tip when the scissor item is used.

    void Start()
    {
        // Get the root visual element.
        var root = GetComponent<UIDocument>().rootVisualElement;


        // Find and assign the UI elements.
        m_BottomContainer = root.Q<VisualElement>("Container_Bottom");
        m_OpenButton = root.Q<Button>("arrow_open");
        m_CloseButton = root.Q<Button>("arrow_close");
        m_CloseButton2 = root.Q<Button>("close_popout2");
        m_BottomSheet = root.Q<VisualElement>("inv_popout");
        m_Scrim = root.Q<VisualElement>("Scrim");
        m_DraggedItem = root.Q<Button>("dragitem");

        m_ItemInfoContainer = root.Q<VisualElement>("Item_Info_Box");
        m_ItemInfoContainer.style.display = DisplayStyle.None;
        m_ItemInfoContainer.style.opacity = 0f;
        m_ItemInfoIcon = root.Q<VisualElement>("Item_Icon_Info");
        m_ItemInfoName = root.Q<Label>("Item_Name_Info");
        m_QuizButton = root.Q<Button>("QuizOpenButton");
        m_timeForCoins = root.Q<Label>("timeForCoins");

        m_Panel = root.Q<VisualElement>("Panel");
        m_ItemUsedPanel = root.Q<VisualElement>("ItemUsePanel");
        m_ItemUsedBackButton = root.Q<Button>("ItemBackButton");
        m_ItemCameraControlsPanel = root.Q<VisualElement>("CameraControlPanel");
        m_CameraButtonLeft = root.Q<Button>("CameraLeftButton");
        m_CameraButtonRight = root.Q<Button>("CameraRightButton");

        //array of item stats
        for (int i = 0; i < m_ItemInfoStats.Length; i++)
        {
            string name = "Item_Info_Stat" + i;
            m_ItemInfoStats[i] = root.Q<Label>(name);
        }

        // Hide the bottom container.
        m_BottomContainer.style.display = DisplayStyle.None;
        m_ItemUsedPanel.style.display = DisplayStyle.None;

        // Register click event callbacks.
        m_OpenButton.RegisterCallback<PointerDownEvent>(OnOPenButtonClicked, TrickleDown.TrickleDown);
        m_OpenButton.RegisterCallback<PointerMoveEvent>(OnOpenButtonMove);
        m_OpenButton.RegisterCallback<PointerUpEvent>(OnOpenButtonUp, TrickleDown.TrickleDown);
        m_CloseButton.RegisterCallback<PointerDownEvent>(OnCloseButtonPointerDown, TrickleDown.TrickleDown);
        m_CloseButton.RegisterCallback<PointerMoveEvent>(OnOpenButtonMove);
        m_CloseButton.RegisterCallback<PointerUpEvent>(OnCloseButtonPointerUp, TrickleDown.TrickleDown);
        m_CloseButton2.RegisterCallback<ClickEvent>(OnCloseButtonClicked);
        m_QuizButton.RegisterCallback<ClickEvent>(OnQuizOpenButtonClicked);
        m_ItemUsedBackButton.RegisterCallback<ClickEvent>(OnItemUsedBackButtonClicked);
        m_CameraButtonLeft.RegisterCallback<PointerDownEvent>((PointerDownEvent evt) => { OnCameraButtonDown(evt, false); }, TrickleDown.TrickleDown);
        m_CameraButtonRight.RegisterCallback<PointerDownEvent>((PointerDownEvent evt) => { OnCameraButtonDown(evt, true); }, TrickleDown.TrickleDown);
        m_CameraButtonLeft.RegisterCallback<PointerUpEvent>((PointerUpEvent evt) => { OnCameraButtonUp(evt, false); }, TrickleDown.TrickleDown);
        m_CameraButtonRight.RegisterCallback<PointerUpEvent>((PointerUpEvent evt) => { OnCameraButtonUp(evt, true); }, TrickleDown.TrickleDown);
        m_CoinAmountPopout = root.Q<Label>("CoinAmountPopout");
        // Register transition end event callback.
        // m_BottomSheet.RegisterCallback<TransitionEndEvent>(OnBottomSheetDown);

        // DragAndDrbject"));

        // Register transition end event callback.
        // _infoBubble.RegisterCallback<TransitionEndEvent>(OnInfoBubbleRight);

        //array of items
        for (int i = 0; i < m_Items.Length; i++)
        {
            string name = "item" + i;
            m_Items[i] = root.Q<Button>(name);
            m_ItemsDict.Add(m_Items[i], i);
        }

        //array of items
        for (int i = 0; i < m_RItems.Length; i++)
        {
            string name = "ritem" + i;
            m_RItems[i] = root.Q<Button>(name);
            m_RItems[i].RegisterCallback<ClickEvent>(RItemClicked);

            string nameAmount = "ItemCounter" + i;
            m_RItemAmounts[i] = root.Q<Label>(nameAmount);
        }

        // foreach (InventoryItem item in m_InventoryItems)
        // {
        // m_InventoryItemsDict.Add(item.itemSlot, item);
        // }

        Vibration.Init();

        StartCoroutine(ShowFirstStartTip());

        inventoryList.onInventoryChanged.AddListener(InitiateItems);

        inventoryList.LoadInventory();
        inventoryList.onInventoryChanged.AddListener(inventoryList.SaveInventory);
        inventoryList.UpdateInventoryDictionary();

        // Update the coin amount popout.
        m_CoinAmountPopout.text = GameStateManager.Instance.gameState.inventory.coins.ToString();
        // UpdateCoinAmountPopout(MoneySystem.Instance.Coins);
        MoneySystem.instance.onCoinsChanged.AddListener(UpdateCoinAmountPopout);

        AddPlacedItemsToInventory();
        InitiateItems();
    }

    private IEnumerator ShowFirstStartTip()
    {
        yield return new WaitForSeconds(0.1f);
        GlobalEvents.Instance.triggerQuickTip.Invoke(tipForFirstStart);
    }

    // Event handler for the item in the item bar click event.
    private void RItemClicked(ClickEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        // MoneySystem.instance.AddCoins(10);
        Button item = (Button)evt.target;
        int itemSlot = item.name[item.name.Length - 1] - '0';
        InventorySlot inventorySlot = inventoryList.GetItem(itemSlot);
        m_CurrentConsumableItem = inventorySlot;
        AudioManager.Instance.PlaySoundEffectByIndex(0);
        if (inventorySlot == null) return;
        switch (inventorySlot.Item.type)
        {
            case ItemType.Throwable when inventorySlot.IsInUse:
                inventorySlot.OnClick();
                item.RemoveFromClassList("item_used");
                inventorySlot.IsInUse = false;
                break;
            case ItemType.Throwable:
                GlobalEvents.Instance.triggerQuickTip.Invoke(tipForThrowables);
                ThrowableItemSO throwableItemSO = (ThrowableItemSO)inventorySlot.Item;
                if (throwableItemSO.consumable)
                {
                    inventorySlot.OnClick(true);
                    SetItemAmount(m_RItemAmounts[itemSlot], inventorySlot.Amount - 1);
                    inventoryList.RemoveItem(itemSlot, 1);
                }
                else
                {
                    inventorySlot.OnClick();
                    item.AddToClassList("item_used");
                    inventorySlot.IsInUse = true;
                }
                break;
            case ItemType.Placeable:
                inventorySlot.OnClick();
                GlobalEvents.Instance.triggerQuickTip.Invoke(tipForPlaceables);
                if (((PlacableItemSO)inventorySlot.Item).TimeToLive > 0)
                    StartCoroutine(ReturnPlacableItem(((PlacableItemSO)inventorySlot.Item).TimeToLive, inventorySlot));
                GameStateManager.Instance.gameState.inventory.placedItems.Add(inventorySlot.Item);
                inventoryList.RemoveItem(itemSlot, 1);
                break;
            case ItemType.Care:
                if (inventorySlot.Item.name == "Razor")
                {
                    GlobalEvents.Instance.triggerQuickTip.Invoke(tipForScissor);
                }
                inventorySlot.OnClick();
                m_Cam = Camera.main.GetComponent<StaticCam>();
                m_Panel.style.display = DisplayStyle.None;
                m_ItemUsedPanel.style.display = DisplayStyle.Flex;
                m_ItemCameraControlsPanel.style.display = DisplayStyle.Flex;
                break;
            case ItemType.Consumable:
                ConsumableItemSO itemSO = (ConsumableItemSO)inventorySlot.Item;
                if (itemSO.bowlType != BowlType.None)
                {
                    Bowl[] activeBowls = FindObjectsOfType<Bowl>();
                    Bowl bowlScript = Array.Find(activeBowls, bowl => bowl.bowlType == itemSO.bowlType);

                    if (bowlScript == null)
                    {
                        GlobalEvents.Instance.triggerQuickTip.Invoke(tipForNoBowlPlaced);
                        break;
                    }
                    Placeable bowlScriptPlacable = bowlScript.GetComponent<Placeable>();
                    Debug.Log(bowlScriptPlacable);
                    if (bowlScriptPlacable != null && !bowlScriptPlacable.IsPlaced)
                    {
                        GlobalEvents.Instance.triggerQuickTip.Invoke(tipForNoBowlPlaced);
                        break;
                    }
                    GlobalEvents.Instance.triggerQuickTip.Invoke(tipForConsumables);
                }
                SetItemAmount(m_RItemAmounts[itemSlot], inventorySlot.Amount - 1);
                inventoryList.RemoveItem(itemSlot, 1);
                inventorySlot.OnClick();
                m_ItemCameraControlsPanel.style.display = DisplayStyle.None;
                m_Panel.style.display = DisplayStyle.None;
                m_ItemUsedPanel.style.display = DisplayStyle.Flex;
                break;
            case ItemType.PooDeleter:
                PooDeleterItemSO pooDeleterItemSO = (PooDeleterItemSO)inventorySlot.Item;

                ExcretionInstance[] excretions = FindObjectsOfType<ExcretionInstance>();
                ExcretionInstance excretion = Array.Find(excretions, (ExcretionInstance e) => e.requiresWiping == pooDeleterItemSO.requireWiping);

                if (excretion == null)
                {
                    GlobalEvents.Instance.triggerQuickTip.Invoke(tipForNoPoo);
                    break;
                }
                GlobalEvents.Instance.triggerQuickTip.Invoke(tipForPooDeleter);
                SetItemAmount(m_RItemAmounts[itemSlot], inventorySlot.Amount - 1);
                inventoryList.RemoveItem(itemSlot, 1);
                inventorySlot.OnClick();
                m_ItemCameraControlsPanel.style.display = DisplayStyle.None;
                m_Panel.style.display = DisplayStyle.None;
                m_ItemUsedPanel.style.display = DisplayStyle.Flex;
                break;
            case ItemType.Minigame:
                inventorySlot.OnClick();
                break;
            default:
                return;
        }
    }

    public void ActivateUI(bool activate)
    {
        if (activate)
        {
            m_ItemCameraControlsPanel.style.display = DisplayStyle.Flex;
            m_Panel.style.display = DisplayStyle.Flex;
        }
        else
        {
            m_ItemCameraControlsPanel.style.display = DisplayStyle.None;
            m_Panel.style.display = DisplayStyle.None;
        }
    }

    private void SetItemAmount(Label label, int amount)
    {
        label.text = amount.ToString();
        if (amount > 1)
        {
            label.style.display = DisplayStyle.Flex;
        }
        else
        {
            label.style.display = DisplayStyle.None;
        }
    }

    private InventorySlot m_CurrentConsumableItem; // The current consumable item.
    // Event handler for the back button click event.
    private void OnItemUsedBackButtonClicked(ClickEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        m_Panel.style.display = DisplayStyle.Flex;
        m_ItemUsedPanel.style.display = DisplayStyle.None;
        m_CurrentConsumableItem.OnClick();
    }

    // Coroutine to return the placable item to the inventory after a delay.
    private IEnumerator ReturnPlacableItem(float time, InventorySlot itemSlot)
    {
        yield return new WaitForSeconds(time);
        itemSlot.OnClick();
        int slot = inventoryList.AddItem(itemSlot.Item, 1);
        if (slot <= 4)
        {
            m_RItems[slot].Focus();
        }
    }

    // Update the items in the item bar with the items in the inventory.
    private void UpdateRItems()
    {
        for (int i = 0; i < m_RItems.Length; i++)
        {
            m_RItems[i].style.backgroundImage = m_Items[i].style.backgroundImage;
            InventorySlot inventorySlot = inventoryList.GetItem(i);
            if (inventorySlot != null)
            {
                SetItemAmount(m_RItemAmounts[i], inventorySlot.Amount);
            }
            else
            {
                m_RItemAmounts[i].style.display = DisplayStyle.None;
            }
        }
    }

    // Update the items in the inventory with the items in the inventory list.
    private void InitiateItems()
    {
        for (int i = 0; i < m_Items.Length; i++)
        {
            m_Items[i].style.backgroundImage = null;
        }
        foreach (InventorySlot item in inventoryList.Inventory)
        {
            m_Items[item.SlotID].style.backgroundImage = (StyleBackground)item.Item.icon;
        }
        UpdateRItems();
    }

    // Event handler for the open button click event.
    private void OnOPenButtonClicked(PointerDownEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        GlobalEvents.Instance.triggerQuickTip.Invoke(tipForInventoryButton);
        // Show the bottom container.
        m_BottomContainer.style.display = DisplayStyle.Flex;

        // // Add classes to animate the bottom sheet and scrim overlay.
        // m_BottomSheet.AddToClassList("inv_popout_up");
        // m_Scrim.AddToClassList("scrim_faded");

        UpdateTimeForCoins();

        StartCoroutine(OnOpenButtonClickedDelayed(evt));
    }

    private void UpdateTimeForCoins()
    {
        long timeForCoins = TimeForCoinsPerDay();
        if (timeForCoins <= 0)
        {
            m_timeForCoins.text = "Coins verfügbar!";
        }
        else
        {
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(timeForCoins);
            if (timeSpan.Hours > 0)
            {
                m_timeForCoins.text = timeSpan.Hours + " Stunden";
            }
            else
            {
                m_timeForCoins.text = timeSpan.Minutes + " Minuten";
            }
        }
    }

    private long TimeForCoinsPerDay()
    {
        long lastCoinsPerDay = GameStateManager.Instance.gameState.inventory.lastCoinsPerDay;
        long simulationDayLength = ((long)GameStateManager.Instance.gameState.simulationDayLength) * 1000;
        long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        return lastCoinsPerDay + simulationDayLength - now;
    }

    private Vector3 m_BottomSheetOrigin; // The origin position of the bottom sheet.
    private Vector2 m_BottomSheetStartPosition; // The start position of the bottom sheet.

    // Event handler for the inventory open button click event.
    private IEnumerator OnOpenButtonClickedDelayed(PointerDownEvent evt)
    {
        yield return new WaitForSeconds(0.05f);

        m_BottomSheetOrigin = m_BottomSheet.transform.position;

        // Get the distance between the the bottom edge of the screeen and the mouse pointer.
        float distance = Input.mousePosition.y + 20;

        var newPosition = new Vector2(
            m_BottomSheet.transform.position.x,
            m_BottomSheet.transform.position.y - distance
        );

        m_BottomSheet.transform.position = newPosition;

        m_BottomSheetStartPosition = newPosition;
        m_pointerStartPosition = Input.mousePosition;
        evt.target.CapturePointer(evt.pointerId);
        m_enabled = true;
    }

    // Event handler for the inventory open button move event.
    private void OnOpenButtonMove(PointerMoveEvent evt)
    {
        if (m_enabled)
        {
            Vector3 pointerDelta = Input.mousePosition - m_pointerStartPosition;
            Vector2 newTargetPosition = new Vector2(
                m_BottomSheetStartPosition.x,
                m_BottomSheetStartPosition.y - pointerDelta.y
            );
            m_BottomSheet.transform.position = newTargetPosition.y < 0 ? new Vector2(newTargetPosition.x, 0) : newTargetPosition;
        }
    }

    // Event handler for the inventory open button release event.
    private void OnOpenButtonUp(PointerUpEvent evt)
    {
        m_enabled = false;
        evt.target.ReleasePointer(evt.pointerId);
        // Add classes to animate the bottom sheet and scrim overlay.
        if (m_BottomSheet.transform.position.y < m_BottomSheet.contentRect.height / 1.5f)
        {
            m_BottomSheet.transform.position = new Vector2(m_BottomSheet.transform.position.x, 0);
            m_BottomSheet.AddToClassList("inv_popout_up");
            m_Scrim.AddToClassList("scrim_faded");

            for (int i = 0; i < m_Items.Length; i++)
            {
                m_Items[i].RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
                m_Items[i].RegisterCallback<PointerMoveEvent>(OnPointerMove);
                m_Items[i].RegisterCallback<PointerUpEvent>(OnPointerUp, TrickleDown.TrickleDown);
            }
        }
        else
        {
            m_BottomSheet.transform.position = m_BottomSheetOrigin;
            m_BottomContainer.style.display = DisplayStyle.None;
        }
    }

    private Vector2 m_targetStartPosition; // The start position of the target item.
    private Vector2 m_targetStartPositionAbsolute; // The absolute start position of the target item.
    private Vector2 m_targetStartPositionWithOffset; // The start position of the target item with offset.
    private Vector3 m_pointerStartPosition; // The start position of the pointer.
    private Vector2 m_draggedItemStartPosition; // The start position of the dragged item.
    private bool m_enabled; // Whether or not an item is being dragged.

    // Event handler for the item button pointer down event.
    private void OnPointerDown(PointerDownEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        Button item = (Button)evt.target;

        InventorySlot inventoryItem = inventoryList.GetItem(m_ItemsDict.GetValueOrDefault(item));
        if (inventoryItem != null)
        {
            UpdateItemInfoBox(inventoryItem);
        }

        m_pointerStartPosition = evt.position;
        m_targetStartPositionAbsolute = item.worldBound.center;

        Vector2 itemdelta = new Vector2(
            item.worldBound.center.x - m_pointerStartPosition.x,
            item.worldBound.center.y - m_pointerStartPosition.y
        );
        m_targetStartPosition = item.transform.position;
        m_targetStartPositionWithOffset = new Vector2(
            item.transform.position.x - itemdelta.x,
            item.transform.position.y - itemdelta.y
        );
        item.transform.position = m_targetStartPositionWithOffset;
        item.CapturePointer(evt.pointerId);
        m_enabled = true;
        item.style.opacity = 0f;

        m_DraggedItem.style.backgroundImage = item.style.backgroundImage;
        m_DraggedItem.style.display = DisplayStyle.Flex;
        m_draggedItemStartPosition = new Vector2(
            m_pointerStartPosition.x - item.contentRect.width / 2,
            m_pointerStartPosition.y - item.contentRect.height / 2
        );
        m_DraggedItem.transform.position = m_draggedItemStartPosition;
    }

    // Update the item info box with the item info.
    private void UpdateItemInfoBox(InventorySlot item)
    {
        m_ItemInfoContainer.style.display = DisplayStyle.Flex;
        for (int i = 0; i < m_ItemInfoStats.Length; i++)
        {
            m_ItemInfoStats[i].text = string.Empty;
            m_ItemInfoStats[i].style.display = DisplayStyle.None;
        }
        m_ItemInfoContainer.style.opacity = 1f;
        m_ItemInfoIcon.style.backgroundImage = new StyleBackground(item.Item.icon);
        m_ItemInfoName.text = item.Item.title;
        m_ItemInfoStats[0].text = "Beschreibung: " + item.Item.description;
        m_ItemInfoStats[0].style.display = DisplayStyle.Flex;
        m_ItemInfoStats[1].text = "Anzahl: " + item.Amount;
        m_ItemInfoStats[1].style.display = DisplayStyle.Flex;
        m_ItemInfoStats[2].text = "Maximale Anzahl: " + item.Item.stackLimit;
        m_ItemInfoStats[2].style.display = DisplayStyle.Flex;
    }

    private Button m_ClosestButton; // The closest button to the dragged item.
    // Event handler for the item button pointer move event.
    private void OnPointerMove(PointerMoveEvent evt)
    {
        Button item = (Button)evt.target;
        if (m_enabled)
        {
            Vector3 pointerDelta = evt.position - m_pointerStartPosition;

            Vector2 newTargetPosition = new Vector2(
                m_targetStartPositionWithOffset.x + pointerDelta.x,
                m_targetStartPositionWithOffset.y + pointerDelta.y
            );
            Button nextClosestButton = FindClosestButton(item);

            if (m_ClosestButton != nextClosestButton)
            {
                if (m_ClosestButton != item && m_ClosestButton != null)
                {
                    m_ClosestButton.style.opacity = 1f;
                }
                if (nextClosestButton != item)
                {
                    UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
                    nextClosestButton.style.opacity = 0.5f;
                }
                m_ClosestButton = nextClosestButton;
            }

            item.transform.position = newTargetPosition;

            m_DraggedItem.transform.position = new Vector2(
                m_draggedItemStartPosition.x + pointerDelta.x,
                m_draggedItemStartPosition.y + pointerDelta.y
            );
        }
    }

    // Event handler for the item button pointer up event.
    private void OnPointerUp(PointerUpEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        Button item = (Button)evt.target;
        item.ReleasePointer(evt.pointerId);
        Button closestButton = FindClosestButton(item);

        foreach (Button button in m_Items)
        {
            button.style.opacity = 1f;
        }

        m_enabled = false;
        item.transform.position = m_targetStartPosition;
        m_DraggedItem.style.display = DisplayStyle.None;
        if (closestButton != null)
        {
            SwapItems(item, closestButton);
        }
        item.Blur();
    }

    // Find the closest button to the dragged item.
    private Button FindClosestButton(Button currentButton)
    {
        Button closestButton = null;
        float closestDistance = float.MaxValue;
        Vector2 currentButtonCenter = currentButton.worldBound.center;
        foreach (Button item in m_Items)
        {
            float distance;
            if (item == currentButton)
            {
                distance = Vector2.Distance(currentButtonCenter, m_targetStartPositionAbsolute);
            }
            else
            {
                distance = Vector2.Distance(currentButtonCenter, item.worldBound.center);
            }
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestButton = item;
            }
        }
        return closestButton;
    }

    // Swap the items in the inventory.
    private void SwapItems(Button item1, Button item2)
    {
        StyleBackground item1Background = item1.style.backgroundImage;
        StyleBackground item2Background = item2.style.backgroundImage;
        item1.style.backgroundImage = item2Background;
        item2.style.backgroundImage = item1Background;

        int item1Slot = m_ItemsDict.GetValueOrDefault(item1);
        int item2Slot = m_ItemsDict.GetValueOrDefault(item2);

        InventorySlot item1Inventory = inventoryList.GetItem(item1Slot);
        InventorySlot item2Inventory = inventoryList.GetItem(item2Slot);

        inventoryList.SwapItems(item1Slot, item2Slot);

        if (item1Inventory != null && item1Inventory.IsInUse)
        {
            m_RItems[item1Slot].RemoveFromClassList("item_used");
            m_RItems[item1Slot].RegisterCallback<ClickEvent>(RItemClicked);
            item1Inventory.IsInUse = false;
            item1Inventory.OnClick();

        }
        if (item2Inventory != null && item2Inventory.IsInUse)
        {
            m_RItems[item2Slot].RemoveFromClassList("item_used");
            m_RItems[item2Slot].RegisterCallback<ClickEvent>(RItemClicked);
            item2Inventory.IsInUse = false;
            item2Inventory.OnClick();
        }
    }

    // Event handler for the close button click event (user for clicking outside the bottom sheet).
    private void OnCloseButtonClicked(EventBase evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        m_ItemInfoContainer.style.opacity = 0f;
        // Remove classes to animate the bottom sheet and scrim overlay.
        m_Scrim.RemoveFromClassList("scrim_faded");
        m_BottomSheet.transform.position = m_BottomSheetOrigin;
        UpdateRItems();
        for (int i = 0; i < m_Items.Length; i++)
        {
            m_Items[i].UnregisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
            m_Items[i].UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            m_Items[i].UnregisterCallback<PointerUpEvent>(OnPointerUp, TrickleDown.TrickleDown);
        }
        float duration = m_BottomSheet.resolvedStyle.transitionDuration.FirstOrDefault().value;
        StartCoroutine(OnBottomSheetDown(duration));
    }

    // Event handler for the close button pointer down event (used for dragging the bottom sheet).
    private void OnCloseButtonPointerDown(PointerDownEvent evt)
    {
        // Show the bottom container.
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        m_ItemInfoContainer.style.opacity = 0f;
        m_BottomSheet.RemoveFromClassList("inv_popout_up");
        m_Scrim.RemoveFromClassList("scrim_faded");
        m_BottomSheetStartPosition = m_BottomSheet.transform.position;
        m_pointerStartPosition = Input.mousePosition;
        evt.target.CapturePointer(evt.pointerId);
        m_enabled = true;
    }

    // Event handler for the close button pointer move event.
    private void OnCloseButtonPointerUp(PointerUpEvent evt)
    {
        m_enabled = false;
        evt.target.ReleasePointer(evt.pointerId);
        // Add classes to animate the bottom sheet and scrim overlay.
        if (m_BottomSheet.transform.position.y < m_BottomSheet.contentRect.height / 4f)
        {
            m_BottomSheet.transform.position = new Vector2(m_BottomSheet.transform.position.x, 0);
            m_BottomSheet.AddToClassList("inv_popout_up");
            m_Scrim.AddToClassList("scrim_faded");
            m_ItemInfoContainer.style.opacity = 1f;
        }
        else
        {
            m_BottomSheet.transform.position = m_BottomSheetOrigin;
            m_BottomContainer.style.display = DisplayStyle.None;
            for (int i = 0; i < m_Items.Length; i++)
            {
                m_Items[i].UnregisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
                m_Items[i].UnregisterCallback<PointerMoveEvent>(OnPointerMove);
                m_Items[i].UnregisterCallback<PointerUpEvent>(OnPointerUp, TrickleDown.TrickleDown);
            }
        }
    }

    // Coroutine to animate the bottom sheet down after a delay.
    private IEnumerator OnBottomSheetDown(float time)
    {
        yield return new WaitForSeconds(time);
        m_BottomSheet.RemoveFromClassList("inv_popout_up");
        m_BottomContainer.style.display = DisplayStyle.None;
        m_ItemInfoContainer.style.display = DisplayStyle.None;
    }

    // Update the coin coin display in the inventory.
    private void UpdateCoinAmountPopout(int amount)
    {
        m_CoinAmountPopout.text = MoneySystem.instance.coins.ToString();
    }

    // Event handler for the quiz open button click event.
    private void OnQuizOpenButtonClicked(ClickEvent evt)
    {
        GameStateManager.LoadScene("Quiz");
    }

    private void AddPlacedItemsToInventory()
    {
        foreach (ItemSO placedItem in GameStateManager.Instance.gameState.inventory.placedItems)
        {
            inventoryList.AddItem(placedItem, 1);
        }
        Debug.Log("Count2:" + GameStateManager.Instance.gameState.inventory.placedItems.Count);
        GameStateManager.Instance.gameState.inventory.placedItems.Clear();
    }

    private StaticCam m_Cam;
    private void OnCameraButtonDown(PointerDownEvent evt, bool right)
    {
        m_Cam.setRotation(right ? -50f : 50f);
    }

    private void OnCameraButtonUp(PointerUpEvent evt, bool right)
    {
        m_Cam.setRotation(0f);
    }

    void Update()
    {
        //Debug.Log(m_BottomSheet.ClassListContains("inv_popout_up"));
    }
}