using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sonat;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;


public enum NotAffordableHandlerEnum
{
    OpenShopPopup,
    InvokeAction,
    ShowNotEnoughDialogThenOpenShop,
    InvokeButton,
    OpenShopAndInvokeAction,
    OpenAdsItemPopup,
    QuantityHandlerScript,
    None,
    InvokeActionLink,
    ShowNotEnoughDialogThenInvokeActionLink,
    OpenShopNextAndInvokeAction,
    ShowNotEnoughDialogOnly,
    ShowBuyBoosterPopup,
}

public interface IDisplayable
{
    bool IsDisplayable();
}

public abstract class GetShopItemView : BaseFrameworkView, IDisplayable
{
    [IndexAsEnum(BuiltInEnumType.ShopItemKey)] [SerializeField]
    protected int key;

    [ConditionalField(nameof(rewardTypeFlag), new object[] {64})] [SerializeField]
    protected CustomClaimScript customClaimScript;

    [IndexAsEnumFlag(BuiltInEnumType.RewardTypeFlag)] [SerializeField]
    protected int rewardTypeFlag = 1;

    public int Key() => key;

//    [SerializeField] [DisplayAsDisable]
    protected ShopItem ShopItem;
    public ShopItem GetShopItem() => ShopItem;


    [IndexAsEnum(BuiltInEnumType.ShopItemKey)]
    public int[] shopItemKeys;

    public IndexBindingScript[] keyRemapBindings;

    public void RemapBinding()
    {
        for (var i = 0; i < shopItemKeys.Length; i++)
            if (Key() == shopItemKeys[i])
                keyRemapBindings.OnChanged(i);
    }


    protected readonly CompositeDisposable CompositeDisposable = new CompositeDisposable();

    public void OverrideKey(int newKey)
    {
        key = newKey;
        ShopItem = RootView.rootView.ShopDatabase.GetShopItem(key).Clone();
        UpdateView();
        SubscribeView();
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        ShopItem = RootView.rootView.ShopDatabase.GetShopItem(key);
        RegisterView();
        UpdateView();
        SubscribeView();

        if (!Kernel.Resolve<BasePurchaser>().IsInitialized())
            Kernel.Resolve<BasePurchaser>().onInitializedSuccess += UpdateView;
    }


    protected virtual void UpdateView()
    {
    }

    protected virtual void RegisterView()
    {
    }

    protected virtual void SubscribeView()
    {
        CompositeDisposable.Clear();
    }

    public bool IsDisplayable()
    {
        return ShopItem.Price.IsInStock();
    }
}

public abstract class BaseExchangeView : GetShopItemView
{
    [SerializeField] protected WhenPopupClose whenCloseShop;

    [SerializeField]
    private NotAffordableHandlerEnum whenNotAffordable = NotAffordableHandlerEnum.ShowNotEnoughDialogThenOpenShop;

    [SerializeField] private bool showMessagePopupBeforeNotAffordableAction;
    [SerializeField] private string notAffordableMessagePopupContent;


    [ConditionalField(nameof(whenNotAffordable), new object[]
        {NotAffordableHandlerEnum.InvokeAction, NotAffordableHandlerEnum.OpenShopAndInvokeAction})]
    [SerializeField]
    private BaseBaseActionScript whenNotAffordableAction;

    [ConditionalField(nameof(whenNotAffordable), new object[] {NotAffordableHandlerEnum.QuantityHandlerScript})]
    [SerializeField]
    private QuantityHandlerScriptBase quantityHandler;

    [ConditionalField(nameof(whenNotAffordable), new object[] {NotAffordableHandlerEnum.InvokeButton})] [SerializeField]
    private BaseButton whenNotAffordableInvokeButton;

    [ConditionalField(nameof(whenNotAffordable), new object[] {NotAffordableHandlerEnum.OpenAdsItemPopup})]
    [SerializeField]
    [IndexAsEnum(BuiltInEnumType.AdsItemKey)]
    private int adsItemNotEnough;

    [ConditionalField(nameof(whenNotAffordable), new object[] {NotAffordableHandlerEnum.InvokeActionLink})]
    [SerializeField]
    [IndexAsEnum(BuiltInEnumType.ActionLink)]
    private int actionLink;

    public abstract bool IsAffordable();


    protected virtual void NotAffordableHandler(Quantity quantity)
    {
        if (showMessagePopupBeforeNotAffordableAction)
        {
            ScreenRoot.dialogController.ShowMessage(
                string.IsNullOrEmpty(notAffordableMessagePopupContent)
                    ? RootView.rootView.gameController.NotEnoughMessage(quantity)
                    : notAffordableMessagePopupContent, RealActionWhenDoNotAffordable,
                () => { });
        }
        else
        {
            RealActionWhenDoNotAffordable();
        }
    }

    private void RealActionWhenDoNotAffordable()
    {
        switch (whenNotAffordable)
        {
            case NotAffordableHandlerEnum.OpenShopPopup:
                ScreenRoot.popupManager.ShowPopup<PopupShop>(-1, whenCloseShop);
                if (whenCloseShop == WhenPopupClose.OpenLastDialog)
                    ScreenRoot.dialogController.CloseAll();
                break;
            case NotAffordableHandlerEnum.InvokeAction:
                whenNotAffordableAction.StartAction();
                break;
            case NotAffordableHandlerEnum.ShowNotEnoughDialogThenOpenShop:
                BaseGameController.HandlerShowNotEnough(ShopItem.Price.quantity);
                break;
            case NotAffordableHandlerEnum.InvokeButton:
                whenNotAffordableInvokeButton.TriggerOnClick();
                break;
            case NotAffordableHandlerEnum.OpenShopAndInvokeAction:
                if (whenNotAffordableAction != null)
                    whenNotAffordableAction.StartAction();
                ScreenRoot.popupManager.ShowPopup<PopupShop>(-1, whenCloseShop);
                if (whenCloseShop == WhenPopupClose.OpenLastDialog)
                    ScreenRoot.dialogController.CloseAll();
                break;
            case NotAffordableHandlerEnum.OpenShopNextAndInvokeAction:
                if (whenNotAffordableAction != null)
                    whenNotAffordableAction.StartAction();
                ScreenRoot.popupManager.ShowPopupNext<PopupShop>(whenCloseShop);
                if (whenCloseShop == WhenPopupClose.OpenLastDialog)
                    ScreenRoot.dialogController.CloseAll();
                break;
            case NotAffordableHandlerEnum.OpenAdsItemPopup:
                ScreenRoot.popupManager.ShowPopupNext<PopupAdsItem>().SetData(adsItemNotEnough);
                break;
            case NotAffordableHandlerEnum.QuantityHandlerScript:
                quantityHandler.Handler(ShopItem.Price.quantity);
                break;
            case NotAffordableHandlerEnum.None:
                break;
            case NotAffordableHandlerEnum.InvokeActionLink:
                ScreenRoot.InvokeActionLink(actionLink);
                break;
            case NotAffordableHandlerEnum.ShowNotEnoughDialogThenInvokeActionLink:
                BaseGameController.HandlerShowNotEnough(ShopItem.Price.quantity,
                    () => ScreenRoot.InvokeActionLink(actionLink));
                break;
            case NotAffordableHandlerEnum.ShowNotEnoughDialogOnly:
                BaseGameController.HandlerShowNotEnoughMessageOnly(ShopItem.Price.quantity);
                break;
            case NotAffordableHandlerEnum.ShowBuyBoosterPopup:
                ScreenRoot.popupManager.ShowPopupNext<PopupLetUsHelp>().SetData(ShopItem);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public enum ShopItemClickAction
{
    Buy = 0,
    Preview = 1,
    SetUse = 2,
    CustomHandler = 3,
    UseBeforeBuy = 4,
    BuyInventory = 5,
}


public abstract class BaseBaseShopItemView : BaseExchangeView
{
    // show popup preview
    [SerializeField] private ShopItemClickAction clickAction = ShopItemClickAction.Buy;

    [ConditionalField(nameof(clickAction), new object[] {ShopItemClickAction.CustomHandler})] [SerializeField]
    private BaseShopItemViewClickHandler handlerScript;

    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    protected int soundClick;

    [SerializeField] protected Button buyButton;
    [SerializeField] protected AdsItemButtonForShopItemView adsButton;
    [SerializeField] protected ToggleScript[] inStockBindings;
    [SerializeField] protected ProductBindingToIndex[] firstProductBindings;
    [SerializeField] protected RewardView[] rewardView;

    [IndexAsEnum(BuiltInEnumType.CustomGameCondition)] [SerializeField]
    private int customCondition;

    [SerializeField] private ToggleScript[] customConditionBinding;
    [SerializeField] private bool inStockActiveThis = true;
    [SerializeField] private bool closeCurrentPopupOnRewarded;
    [SerializeField] private bool showRewardAsNext;
    protected virtual bool FillOffer => false;

    protected override void RegisterView()
    {
        base.RegisterView();
        if (buyButton != null)
            buyButton.onClick.AddListener(ClickHandler);
        else
            Debug.LogError("buy button null", gameObject);

        if (adsButton != null)
            adsButton.SetOnReward(OnAdsRewarded, AdsProducts);
    }

    protected override void SubscribeView()
    {
        base.SubscribeView();
        SubjectController.InAppPurchasedEvent
            .TakeUntilDestroy(this)
            .Where(x => x == Key())
            .Subscribe(data => { UpdateStock(); })
            .DisposeWith(this)
            .AddTo(CompositeDisposable);

        if (customCondition != 0)
        {
            CheckCustomCondition();
            SubjectController.NotifyCustomGameCondition.Subscribe(data => CheckCustomCondition())
                .AddTo(CompositeDisposable);
        }


        if (ShopItem.Price.quantity == Quantity.CustomProduct)
            SubjectController.NotifyCustomProductPrice.Subscribe(data => UpdateView())
                .AddTo(CompositeDisposable);
    }

    protected virtual void UpdateStock()
    {
        var inStock = ShopItem.Price.IsInStock();
        foreach (var toggle in inStockBindings)
            toggle.OnChanged(inStock);
        if (ShopItem.OriginalPriceQuantity == Quantity.CustomProduct)
            UpdateView();
    }


    protected override void UpdateView()
    {
        CheckKernelLoaded();
        toggleFree.OnChanged(freeTake || freeTakeOnce);

        if (adsButton != null)
        {
            if (ShopItem == null)
                Debug.LogError("not found " + BuiltInEnumType.ShopItemKey.GetName(key));
        }

        foreach (var toggle in inStockBindings)
            toggle.OnChanged(ShopItem.Price.IsInStock());
        var products = Products.ToArray();
        foreach (var product in products)
        {
            foreach (var productBindingToIndex in firstProductBindings)
                productBindingToIndex.StartAction(product);
            break;
        }

        for (var i = 0; i < Mathf.Min(products.Length, rewardView.Length); i++)
            rewardView[i].SetData(products[i]);
    }


    // to inject action when successful
    protected Action WhenSuccessful { get; set; }

    [SerializeField] protected bool withExtra;

    protected virtual IEnumerable<Product> Products =>
        withExtra ? ShopItem.GetProductsWithExtra() : ShopItem.GetProducts();

    protected virtual IEnumerable<Product> AdsProducts => ShopItem.GetAdsProducts();

    [SerializeField] private bool freeTake;
    [SerializeField] private bool freeTakeOnce;

    [SerializeField] private bool isBoosterItem;

    [SerializeField] private ToggleScript[] toggleFree;

    public bool IsBoosterItem => isBoosterItem;

    public void SetFree(bool val)
    {
        //freeTake = val;
        //toggleFree.OnChanged(freeTake || freeTakeOnce);
    }

    public void SetFreeOnce(bool val)
    {
        //freeTakeOnce = val;
        //toggleFree.OnChanged(freeTake || freeTakeOnce);

        switch (key)
        {
            //Undo
            case GameDefine.BoosterUndoKey:
                if (!PlayerData.alreadyGetFreeBoosters[0])
                {
                    PlayerData.bUndo.Value += 1;
                    PlayerData.alreadyGetFreeBoosters[0] = true;
                }
                break;
            //More3Slot
            case GameDefine.BoosterMore3SlotKey:
                if (!PlayerData.alreadyGetFreeBoosters[1])
                {
                    PlayerData.bMore3Slot.Value += 1;
                    PlayerData.alreadyGetFreeBoosters[1] = true;
                }
                break;
            //GoStraight
            case GameDefine.BoosterGoStraightKey:
                if (!PlayerData.alreadyGetFreeBoosters[2])
                {
                    PlayerData.bGoStraight.Value += 1;
                    PlayerData.alreadyGetFreeBoosters[2] = true;
                }
                break;
        }

        PlayerData.Save();
    }

    public void ResetFreeStatus()
    {
        freeTake = freeTakeOnce = false;
        toggleFree.OnChanged(freeTake || freeTakeOnce);
    }

    public void TryToSpend()
    {
        if (!(freeTake || freeTakeOnce))
        {
            ShopItem.Spend();
            
        }
        freeTakeOnce = false;
        toggleFree.OnChanged(freeTake || freeTakeOnce);
    }

    public virtual void ClickHandler()
    {
        ShortHand.LightFeedBack();
        soundClick.PlaySound();
        var iconName = "ClickShop" + BuiltInEnumType.ShopItemKey.GetName(Key());
        BaseGameController.LogShortcut(iconName.ToSnakeCase());

        var valid = BaseGameController.GetCustomCondition(customCondition);

        if (!valid)
        {
            if (isBoosterItem && !PlayerData.CanUseItem(key))
            {
                //Show popup buy booster
                NotAffordableHandler(ShopItem.Price.quantity);
            }

            return;
        }

        if (clickAction == ShopItemClickAction.CustomHandler)
        {
            handlerScript.Handler(this);
            return;
        }

        if (clickAction == ShopItemClickAction.Preview)
        {
            ScreenRoot.popupManager.Resolve<PopupPreviewShopItem>().CheckShow(key);
            return;
        }

        if (isBoosterItem)
        {
            if (PlayerData.CanUseItem(key))
            {
                if (clickAction == ShopItemClickAction.Buy)
                {
                    var claimedReward = ShopItem.Product.ConvertRandom();
                    SubjectController.ProductGameActionEvent.OnNext(ProductAction.FromProduct(claimedReward));
                    PlayerData.UseItem(key);
                }

                if (clickAction == ShopItemClickAction.SetUse)
                {
                    BaseGameController.StartUseShopItem(this);
                }

                if (clickAction == ShopItemClickAction.UseBeforeBuy)
                {
                    BaseGameController.StartUseShopItemBeforeBuy(this);
                }
            }
            else
            {
                //Show popup buy booster
                NotAffordableHandler(ShopItem.Price.quantity);
            }

            return;
        }

        if (PlayerData.IsAffordable(ShopItem.Price) || freeTake || freeTakeOnce)
        {
            if (clickAction == ShopItemClickAction.Buy)
            {
                var trade = new Trade<Product, Product>((freeTake || freeTakeOnce) ? Product.Free : ShopItem.Price, Products);
                trade.SetBoosterName(ShopItem.boosterName);
                var input = new ShowRewardInput(trade, rewardTypeFlag);

                if ((rewardTypeFlag & (int) RewardTypeFlag.CustomClaimScript) != 0 && customClaimScript != null)
                    input.CustomClaimScript = customClaimScript;

                input.closeCurrentPopup = closeCurrentPopupOnRewarded;
                input.showRewardAsNext = showRewardAsNext;
                if (ShopItem.Price.quantity == Quantity.IapBuying)
                {
                    UIDebugLog.Log(Kernel.Resolve<BasePurchaser>().GetPriceText(ShopItem.Price.priceKey.key));
                    Debug.LogError("create buy iap log here");

                    GameDefine.OnClickBuyIAP = true;

                    Kernel.Resolve<BasePurchaser>().Buy(ShopItem.Price.priceKey.key, success =>
                    {
                        if (success)
                        {
                            RootView.rootView.screenRoot.ShowReward(input);
                            OnSuccess();
                        }
                    }, new SonatLogBuyShopItemIapInput("", "", 0)
                    {
                        //DTODO get buy log
                    });
                }
                else
                {
                    RootView.rootView.screenRoot.ShowReward(input);
                    OnSuccess();
                }

                freeTakeOnce = false;
                toggleFree.OnChanged(freeTake || freeTakeOnce);
            }

            if (clickAction == ShopItemClickAction.BuyInventory)
            {
                var trade = new Trade<Product, Product>((freeTake || freeTakeOnce) ? Product.Free : ShopItem.Price, Products);
                trade.SetBoosterName(ShopItem.boosterName);
                var input = new ShowRewardInput(trade, rewardTypeFlag);

                input.trade.Spend();
                PlayerData.AddBoosterToInventory(key, ShopItem.Product.amount);
                PlayerData.Save();
                SubjectController.GameActionEvent.OnNext(GameActionEvent.Resume);

                freeTakeOnce = false;
                toggleFree.OnChanged(freeTake || freeTakeOnce);
            }

            if (clickAction == ShopItemClickAction.SetUse)
            {
                BaseGameController.StartUseShopItem(this);
            }

            if (clickAction == ShopItemClickAction.UseBeforeBuy)
            {
                BaseGameController.StartUseShopItemBeforeBuy(this);
            }
        }
        else
            NotAffordableHandler(ShopItem.Price.quantity);
    }

    private void OnAdsRewarded()
    {
        if (clickAction == ShopItemClickAction.BuyInventory)
        {
            PlayerData.AddBoosterToInventory(key, ShopItem.GetAdsProducts().ToArray()[0].Amount);
            PlayerData.Save();
            SubjectController.GameActionEvent.OnNext(GameActionEvent.Resume);

            return;
        }

        var trade = new Trade<Product, Product>(new Product(Quantity.WatchAds, 1, 0), AdsProducts);
        trade.SetBoosterName(ShopItem.boosterName);
        var input = new ShowRewardInput(trade, rewardTypeFlag)
        {
            closeCurrentPopup = closeCurrentPopupOnRewarded,
            showRewardAsNext = showRewardAsNext
        };
        if ((rewardTypeFlag & (int) RewardTypeFlag.CustomClaimScript) != 0 && customClaimScript != null)
            input.CustomClaimScript = customClaimScript;
        RootView.rootView.screenRoot.ShowReward(input);
        OnSuccess();
    }


    protected virtual void OnSuccess()
    {
        OnSuccessful();
        PlayerData.FillShopPriceStock(ShopItem.Price);
        BaseGameController.OnShopStockFilled(ShopItem.Price);
        if (ShopItem.Price.quantity == Quantity.IapBuying)
            SubjectController.InAppPurchasedEvent.OnNext(ShopItem.Price.priceKey.key);
        DuongExtensions.ExecuteInNextFrame(() => { SubjectController.OnShopItemBought.OnNext(ShopItem.Key); });
        RootView.rootView.LogShopItem(ShopItem.Key, new[] {ShopItem.Price});
        UpdateStock();
        PlayerData.Save();
        WhenSuccessful?.Invoke();
    }

    protected virtual void OnSuccessful()
    {
        if (ShopItem.ignoreSave)
            PlayerData.IgnoreSaveUntilTurnOn();
        if (FillOffer)
        {
            DailyData.offerBought[Key()] = true;
            DailyData.Save();
            SubjectController.OfferActivated.OnNext(Key());
        }
    }

    protected virtual void CheckCustomCondition()
    {
        var valid = BaseGameController.GetCustomCondition(customCondition);
        customConditionBinding.OnChanged(valid);
    }

    public void CheckActiveIfInStock()
    {
        CheckRegister();
        if (ShopItem == null)
        {
            ShopItem = RootView.rootView.ShopDatabase.GetShopItem(key);
            if (ShopItem == null)
                Debug.LogError("shop item not found err", gameObject);
            return;
        }

        if (!Kernel.IsReady())
        {
            Debug.Log("Kernel is not ready");
            return;
        }

        var inStock = ShopItem.Price.IsInStock();
        if (inStockActiveThis)
            gameObject.SetActive(ShopItem.Price.IsInStock());
        foreach (var inStockBinding in inStockBindings)
            inStockBinding.OnChanged(inStock);
    }

    protected virtual void OnEnable()
    {
        if (KernelLoadedRegistered)
            if (ShopItem.IsRandomReward())
            {
                rewardView[0].SetData(ShopItem.GetProducts().ToArray()[0]);
                if (adsButton != null)
                    adsButton.SetOnReward(OnAdsRewarded, AdsProducts);
            }

        //  UpdateView();
    }
}

public abstract class BaseShopItemView : BaseBaseShopItemView
{
    [SerializeField] protected BindingToStringTextChange priceAmountText;
    [SerializeField] protected BindingToStringTextChange productAmountText;
    [SerializeField] protected Image currencyIcon;
    [SerializeField] protected BindingToStringTextChange progressText;
    [SerializeField] protected TMP_Text txtDescription;

    protected override void UpdateView()
    {
        base.UpdateView();
        if (ShopItem != null)
        {
//            UIDebugLog.Log("UpdateView "+ShopItem.Price.GetPriceText());
            if (txtDescription != null) txtDescription.text = ShopItem.description;
            if (priceAmountText != null) priceAmountText.OnChanged(ShopItem.Price.GetPriceText());
            if (ShopItem.Product != null && productAmountText != null)
                productAmountText.OnChanged(ShopItem.Product.amount.ToString());
            if (currencyIcon != null)
                currencyIcon.sprite = ShopItem.Price.GetImageSprite();
            if (progressText != null)
            {
                var progress = ShopItem.Price.GetProgress();
                progressText.OnChanged((progress.y - progress.x) + "/" + progress.y);
            }

            foreach (var toggle in inStockBindings)
                toggle.OnChanged(ShopItem.Price.IsInStock());
            RemapBinding();
        }
    }


    public bool IsInStock()
    {
        return ShopItem.Price.IsInStock();
    }

    protected override void UpdateStock()
    {
        base.UpdateStock();
        var progress2 = ShopItem.Price.GetProgress();
        if (progressText != null) progressText.OnChanged((progress2.y - progress2.x) + "/" + progress2.y);
    }


    public override bool IsAffordable()
    {
        return PlayerData.IsAffordable(ShopItem.Price);
    }

    public string GetPriceText()
    {
        if (ShopItem.Price.quantity != Quantity.IapBuying)
            return ShopItem.Price.amount.ToString();

        return GetPriceText(ShopItem.Price.priceKey.key);
    }

    protected static string GetPriceText(int k)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return "$" + RootView.rootView.ShopDatabase.storeProductDescriptors
                       .Find(x => x.key == k)
                       .price.ToString(CultureInfo.InvariantCulture);
        }
#endif

        return Kernel.Resolve<BasePurchaser>().GetPriceText(k);
    }
}