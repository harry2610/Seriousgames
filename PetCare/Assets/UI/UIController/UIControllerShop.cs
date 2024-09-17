using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Linq;

/// <summary>
/// Controls the shop UI elements.
/// </summary>
public class UIControllerShop : MonoBehaviour
{
    private VisualElement m_ShopScreen; // The shop screen.
    private VisualElement m_ShopScrim; // The scrim for the shop screen.
    private Button m_ShopOpenButton1; // The button to open the shop No. 1.
    private Button m_ShopOpenButton2; // The button to open the shop No. 2.
    private Button m_ShopOpenButton3; // The button to open the shop No. 3.
    private Button m_ShopOpenButton4; // The button to open the shop No. 4.
    private Button m_ShopExitButton; // The button to exit the shop.
    private Button m_ShopNextButton; // The button to go to the next shop.
    private Button m_ShopPreviousButton; // The button to go to the previous shop.
    private ScrollView m_ShopListScrollView; // The scroll view for the shop list.
    private VisualElement m_Panel; // The panel for the shop screen.
    private VisualElement m_TopLeftPanel; // The top left panel for the shop screen.
    private VisualElement m_TopRightPanel; // The top right panel for the shop screen.
    private Label m_CoinAmountLabel; // The label for the coin amount.
    private Button m_InfoButton; // The info button.
    private int m_CurrentShopInventoryListIndex = 0; // The current shop inventory list index.
    private float m_ShopListAnimationDuration = 0.5f; // The duration of the shop list animation.

    /// <summary>
    /// The visual tree asset for the shop list button.
    /// </summary>
    public VisualTreeAsset myVisualTreeAsset;
    /// <summary>
    /// The player's inventory list.
    /// </summary>
    public PlayerInventoryListSO PlayerInventoryList;
    /// <summary>
    /// The shop inventory lists. Must be 4 Inventory Lists.
    /// </summary>
    [Header("Must be 4 Inventory Lists.")]
    public List<InventoryListSO> ShopInventoryLists;

    public QuicktipSO tipForShop; // The quick tip for the shop.
    public QuicktipSO tipForShopButton; // The quick tip for the shop button.
    public QuicktipSO tipForNotEnoughCoins; // The quick tip for not enough coins.

    void Start()
    {
        // Get the root visual element.
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Get the shop UI elements.
        m_ShopScreen = root.Q<VisualElement>("Shop_Screen");   
        m_ShopScrim = root.Q<VisualElement>("Scrim_Shop");
        m_ShopOpenButton1 = root.Q<Button>("ShopOpenButton1");
        m_ShopOpenButton2 = root.Q<Button>("ShopOpenButton2");
        m_ShopOpenButton3 = root.Q<Button>("ShopOpenButton4"); // change this when using 4 shops
        m_ShopOpenButton4 = root.Q<Button>("ShopOpenButton3"); // change this when using 4 shops
        m_ShopExitButton = root.Q<Button>("ExitShop");
        m_ShopNextButton = root.Q<Button>("NextShop");
        m_ShopPreviousButton = root.Q<Button>("PrevShop");
        m_ShopListScrollView = root.Q<ScrollView>("ShopList");
        m_Panel = root.Q<VisualElement>("Panel");
        m_TopLeftPanel = root.Q<VisualElement>("topleft");
        m_TopRightPanel = root.Q<VisualElement>("topright");
        m_CoinAmountLabel = root.Q<Label>("ShopListElementPrice");
        m_InfoButton = root.Q<Button>("Info");

        // Hide the shop screen.
        m_ShopScreen.style.display = DisplayStyle.None;
        m_InfoButton.style.visibility = Visibility.Hidden;

        // Register the button callbacks.
        m_ShopOpenButton1.RegisterCallback<ClickEvent>(evt => OnShopOpenButton1Clicked(evt, 0));
        m_ShopOpenButton2.RegisterCallback<ClickEvent>(evt => OnShopOpenButton1Clicked(evt, 1));
        m_ShopOpenButton3.RegisterCallback<ClickEvent>(evt => OnShopOpenButton1Clicked(evt, 2));
        m_ShopOpenButton4.RegisterCallback<ClickEvent>(evt => OnShopOpenButton1Clicked(evt, 3));
        m_ShopExitButton.RegisterCallback<ClickEvent>(OnShopExitButtonClicked);
        m_ShopNextButton.RegisterCallback<ClickEvent>(OnShopNextButtonClicked);
        m_ShopPreviousButton.RegisterCallback<ClickEvent>(OnShopPreviousButtonClicked);

        m_CoinAmountLabel.text = GameStateManager.Instance.gameState.inventory.coins.ToString();
        MoneySystem.instance.onCoinsChanged.AddListener(UpdateCoinAmountLabel);

        StartCoroutine(ShowShopTip());
    }

    private IEnumerator ShowShopTip()
    {
        yield return new WaitForSeconds(0.1f);
        GlobalEvents.Instance.triggerQuickTip.Invoke(tipForShop);
    }

    // Event handler for the shop open button click event.
    private void OnShopOpenButton1Clicked(ClickEvent evt, int inventoryListIndex)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        GlobalEvents.Instance.triggerQuickTip.Invoke(tipForShopButton);
        m_TopLeftPanel.style.visibility = Visibility.Hidden;
        m_TopRightPanel.style.visibility = Visibility.Hidden;
        m_ShopScreen.style.display = DisplayStyle.Flex;
        m_ShopScrim.AddToClassList("scrim_faded");
        m_CurrentShopInventoryListIndex = inventoryListIndex;
        PopulateShopList(ShopInventoryLists[inventoryListIndex]);
        m_ShopListScrollView.RemoveFromClassList("shopList_left");
        m_ShopListAnimationDuration = m_ShopListScrollView.resolvedStyle.transitionDuration.FirstOrDefault().value;

        StartCoroutine(ChangeOpenShopButtonOpacity(0.1f, 0));
    }

    // Event handler for the shop exit button click event.
    private void OnShopExitButtonClicked(ClickEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        m_ShopScrim.RemoveFromClassList("scrim_faded");
        m_ShopListScrollView.AddToClassList("shopList_left");
        Debug.Log(m_ShopListAnimationDuration);
        StartCoroutine(FadeOutShop(m_ShopListAnimationDuration / 1.7f));

        StartCoroutine(ChangeOpenShopButtonOpacity(m_ShopListAnimationDuration / 1.7f, 1));
    }

    private IEnumerator ChangeOpenShopButtonOpacity(float time, float opacity)
    {
        yield return new WaitForSeconds(time);
        m_ShopOpenButton1.style.opacity = opacity;
        m_ShopOpenButton2.style.opacity = opacity;
        m_ShopOpenButton3.style.opacity = opacity;
        m_ShopOpenButton4.style.opacity = opacity;
    }

    // Coroutine to fade out the shop screen after a delay.
    private IEnumerator FadeOutShop(float time)
    {
        yield return new WaitForSeconds(time);
        m_TopLeftPanel.style.visibility = Visibility.Visible;
        m_TopRightPanel.style.visibility = Visibility.Visible;
        ClearShopList();
        m_ShopScreen.style.display = DisplayStyle.None;
    }

    // Event handler for the shop next button click event.
    private void OnShopNextButtonClicked(ClickEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        // Get next shop inventory list.
        int nextIndex = m_CurrentShopInventoryListIndex + 1;
        if (nextIndex >= ShopInventoryLists.Count)
        {
            nextIndex = 0;
        }
        Debug.Log("Next Index: " + nextIndex);
        m_CurrentShopInventoryListIndex = nextIndex;
        m_ShopListScrollView.AddToClassList("shopList_right");
        StartCoroutine(RemoveShopListRightClass(m_ShopListAnimationDuration / 1.7f, nextIndex));
    }

    // Coroutine to remove the shop list right class after a delay for smooth animation.
    private IEnumerator RemoveShopListRightClass(float time, int index)
    {
        yield return new WaitForSeconds(time);
        m_ShopListScrollView.RemoveFromClassList("shopList_right");
        ClearShopList();
        PopulateShopList(ShopInventoryLists[index]);
    }

    // Event handler for the shop previous button click event.
    private void OnShopPreviousButtonClicked(ClickEvent evt)
    {
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        // Get previous shop inventory list.
        int previousIndex = m_CurrentShopInventoryListIndex - 1;
        if (previousIndex < 0)
        {
            previousIndex = ShopInventoryLists.Count - 1;
        }
        m_CurrentShopInventoryListIndex = previousIndex;
        m_ShopListScrollView.AddToClassList("shopList_left");
        StartCoroutine(RemoveShopListLeftClass(m_ShopListAnimationDuration / 1.7f, previousIndex));
    }

    // Coroutine to remove the shop list left class after a delay for smooth animation.
    private IEnumerator RemoveShopListLeftClass(float time, int index)
    {
        yield return new WaitForSeconds(time);
        m_ShopListScrollView.RemoveFromClassList("shopList_left");
        ClearShopList();
        PopulateShopList(ShopInventoryLists[index]);
    }

    // Populates the shop list with the given inventory list.
    private void PopulateShopList(InventoryListSO shopInventoryList)
    {
        if (shopInventoryList == null)
        {
            return;
        }
        shopInventoryList.SortInventoryBySlotID();
        foreach (InventorySlot inventorySlot in shopInventoryList.Inventory)
        {
            CreateShopListButton(inventorySlot);
        }
    }

    // Creates a shop list button for the given inventory slot.
    private void CreateShopListButton(InventorySlot inventorySlot)
    {
        TemplateContainer shopListButtonTemplate = myVisualTreeAsset.Instantiate();
        VisualElement shopListElementIcon = shopListButtonTemplate.Q<VisualElement>("ShopListElementPic");
        Label shopListElementName = shopListButtonTemplate.Q<Label>("ShopListElementName");
        Label shopListElementDescription = shopListButtonTemplate.Q<Label>("ShopListElementDescription");
        Label shopListElementPrice = shopListButtonTemplate.Q<Label>("ShopListElementPrice");
        Label shopListElementAmount = shopListButtonTemplate.Q<Label>("ShopListElementAmount");
        Button shopListElementButton = shopListButtonTemplate.Q<Button>("ShopListButton");
        VisualElement shopListElementScrim = shopListButtonTemplate.Q<VisualElement>("ShopListElementScrim");

        shopListElementIcon.style.backgroundImage = inventorySlot.Item.icon;
        shopListElementName.text = inventorySlot.Item.title;
        shopListElementDescription.text = inventorySlot.Item.description;
        shopListElementPrice.text = inventorySlot.Item.price.ToString();

        InventorySlot playerItem = PlayerInventoryList.Inventory.Find(x => x.Item == inventorySlot.Item);

        shopListElementAmount.text = (playerItem != null ? 
            (playerItem.Item.stackLimit - playerItem.Amount).ToString() : inventorySlot.Item.stackLimit.ToString()) + "x";

        if (playerItem != null && playerItem.Item.stackLimit == playerItem.Amount)
        {
            shopListElementScrim.style.display = DisplayStyle.Flex;
        } else
        {
            shopListElementScrim.style.display = DisplayStyle.None;
            shopListElementButton.RegisterCallback<ClickEvent>(evt => OnShopListButtonClicked(evt, inventorySlot));
        }

        m_ShopListScrollView.Add(shopListButtonTemplate);
    }

    // Clears the shop list.
    private void ClearShopList()
    {
        m_ShopListScrollView.Clear();
    }

    // Event handler for the shop list button click event.
    private void OnShopListButtonClicked(ClickEvent evt, InventorySlot inventorySlot)
    {
        Button button = (Button)evt.target;
        VisualElement shopListElementScrim = button.Q<VisualElement>("ShopListElementScrim");
        Label label = button.Q<Label>("ShopListElementScrimLabel");
        Label shopListElementAmount = button.Q<Label>("ShopListElementAmount");
        UIControllerHapticFeedback.AndroidVibrateTime(UIControllerHapticFeedback.BasicButtonVibrationTime);
        if (MoneySystem.instance.coins >= inventorySlot.Item.price)
        {
            MoneySystem.instance.RemoveCoins(inventorySlot.Item.price);
            PlayerInventoryList.AddItem(inventorySlot.Item, inventorySlot.Amount);
            shopListElementAmount.text = int.Parse(shopListElementAmount.text.Split('x')[0]) - 1 + "x";
        } else
        {
            GlobalEvents.Instance.triggerQuickTip.Invoke(tipForNotEnoughCoins);
            label.text = "Zu wenig Münzen!";
            shopListElementScrim.style.display = DisplayStyle.Flex;
            StartCoroutine(DisableShopListButtonScrim(shopListElementScrim));
        }

        InventorySlot playerItem = PlayerInventoryList.Inventory.Find(x => x.Item == inventorySlot.Item);

        if (playerItem != null && playerItem.Item.stackLimit == playerItem.Amount)
        {
            ClearShopList();
            PopulateShopList(ShopInventoryLists[m_CurrentShopInventoryListIndex]);
        }
    }

    // Coroutine to disable the shop list button scrim after a delay.
    private IEnumerator DisableShopListButtonScrim(VisualElement scrim)
    {
        yield return new WaitForSeconds(1);
        scrim.style.display = DisplayStyle.None;
    }

    // Updates the coin amount label.
    private void UpdateCoinAmountLabel(int amount)
    {
        m_CoinAmountLabel.text = MoneySystem.instance.coins.ToString();
    }
}
