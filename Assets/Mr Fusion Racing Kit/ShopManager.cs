using RGSK;
using UnityEngine;
using UnityEngine.Purchasing; 
using UnityEngine.UI;
using UnityEngine.Purchasing.Extension;
using Firebase.Analytics;

public class ShopManager : MonoBehaviour, IDetailedStoreListener
{
    private IStoreController storeController;
    private IExtensionProvider extensionProvider; // NEW: ��� RestorePurchases

    void Start()
    {
        if (storeController == null)
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct("cash_1", ProductType.Consumable);
            builder.AddProduct("cash_2", ProductType.Consumable);
            builder.AddProduct("cash_3", ProductType.Consumable);
            builder.AddProduct("cash_4", ProductType.Consumable);
            builder.AddProduct("cash_5", ProductType.Consumable);
            builder.AddProduct("cash_6", ProductType.Consumable);
            builder.AddProduct("cash_7", ProductType.Consumable);

            // NEW: ������� ���������� ������� (non-consumable)
            builder.AddProduct("remove_ads", ProductType.NonConsumable);

            UnityPurchasing.Initialize(this, builder);
        }
    }

    public void BuyProduct(string productId)
    {
        if (storeController != null && storeController.products.WithID(productId) != null)
        {
            storeController.InitiatePurchase(productId);
        }
        else
        {
            Debug.LogError("������ �������: ������� �� ������!");
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        extensionProvider = extensions;

        var product = storeController.products.WithID("remove_ads");
        if (product != null && product.hasReceipt)
        {
            // ���� � ��� ��� ���� receipt � ����� ��������� �������
            PlayerData.instance.RemoveAds();
        }
        else
        {
            // ����� ������� ��������� ������� (�������� ����� ��� �������� ���������)
            RestorePurchases();
        }
    }



    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"������ ������������� IAP: {error}");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"������ ������������� IAP: {error}, {message}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        string productId = args.purchasedProduct.definition.id;

        // 1) ��������� �������
        switch (productId)
        {
            case "cash_1":
                PlayerData.instance.AddCurrency(50000);
                break;
            case "cash_2":
                PlayerData.instance.AddCurrency(140000);
                break;
            case "cash_3":
                PlayerData.instance.AddCurrency(300000);
                break;
            case "cash_4":
                PlayerData.instance.AddCurrency(700000);
                break;
            case "cash_5":
                PlayerData.instance.AddCurrency(1000000);
                break;
            case "cash_6":
                PlayerData.instance.AddCurrency(1700000);
                break;
            case "cash_7":
                PlayerData.instance.AddCurrency(2500000);
                break;
            case "remove_ads":
                PlayerData.instance.RemoveAds();
                break;
            default:
                Debug.LogWarning($"ShopManager: ����������� ������� {productId}");
                break;
        }

        // 2) ���������� UI
        FindObjectOfType<PlayerSettings>()?.UpdateUIToMatchSettings();

        Debug.Log($"������� ���������: {productId}");

        // 3) �������� ���� ������� � Firebase Analytics
        FirebaseAnalytics.LogEvent(
            "iap_purchase",
            new Parameter("product_id", productId)
        );

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogError($"������� �� �������: {product.definition.id}, �������: {reason}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogError($"������� �� �������: {product.definition.id}, �������: {failureDescription.reason}, ������: {failureDescription.message}");
    }

    /// <summary>
    /// ��������������� non-consumable ������� � �������� ���������.
    /// </summary>
    public void RestorePurchases()
    {
        if (storeController == null || extensionProvider == null)
        {
            Debug.LogError("RestorePurchases: IAP �� ���������������");
            return;
        }

        var apple = extensionProvider.GetExtension<IAppleExtensions>();
        apple.RestoreTransactions((success, error) =>
        {
            Debug.Log($"RestoreTransactions ��������: success={success}, error={error}");

            // �������� ��������� �������������� � Firebase Analytics
            FirebaseAnalytics.LogEvent(
                "iap_restore",
                new Parameter("success", success ? 1 : 0),
                new Parameter("error", error ?? "none")
            );

            if (success)
            {
                var prod = storeController.products.WithID("remove_ads");
                if (prod != null && prod.hasReceipt)
                    PlayerData.instance.RemoveAds();
            }
        });

        // �� Google Play non-consumable ������������� �������������,
        // �� ��� ��������� �� ����� ���������� ������� ������ ��������.
        FirebaseAnalytics.LogEvent("iap_restore_started");
    }
}