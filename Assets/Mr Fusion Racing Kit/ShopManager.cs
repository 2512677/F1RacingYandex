using RGSK;
using UnityEngine;
using UnityEngine.Purchasing; 
using UnityEngine.UI;
using UnityEngine.Purchasing.Extension;
using Firebase.Analytics;

public class ShopManager : MonoBehaviour, IDetailedStoreListener
{
    private IStoreController storeController;
    private IExtensionProvider extensionProvider; // NEW: для RestorePurchases

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

            // NEW: продукт «отключить рекламу» (non-consumable)
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
            Debug.LogError("Ошибка покупки: продукт не найден!");
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        extensionProvider = extensions;

        var product = storeController.products.WithID("remove_ads");
        if (product != null && product.hasReceipt)
        {
            // Если у нас уже есть receipt — сразу отключаем рекламу
            PlayerData.instance.RemoveAds();
        }
        else
        {
            // Иначе пробуем подтянуть покупки (особенно важно для тестовых аккаунтов)
            RestorePurchases();
        }
    }



    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"Ошибка инициализации IAP: {error}");
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError($"Ошибка инициализации IAP: {error}, {message}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        string productId = args.purchasedProduct.definition.id;

        // 1) Обработка покупки
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
                Debug.LogWarning($"ShopManager: Неизвестный продукт {productId}");
                break;
        }

        // 2) Обновление UI
        FindObjectOfType<PlayerSettings>()?.UpdateUIToMatchSettings();

        Debug.Log($"Покупка завершена: {productId}");

        // 3) Логируем факт покупки в Firebase Analytics
        FirebaseAnalytics.LogEvent(
            "iap_purchase",
            new Parameter("product_id", productId)
        );

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogError($"Покупка не удалась: {product.definition.id}, Причина: {reason}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        Debug.LogError($"Покупка не удалась: {product.definition.id}, Причина: {failureDescription.reason}, Детали: {failureDescription.message}");
    }

    /// <summary>
    /// Восстанавливаем non-consumable покупки и логируем результат.
    /// </summary>
    public void RestorePurchases()
    {
        if (storeController == null || extensionProvider == null)
        {
            Debug.LogError("RestorePurchases: IAP не инициализирован");
            return;
        }

        var apple = extensionProvider.GetExtension<IAppleExtensions>();
        apple.RestoreTransactions((success, error) =>
        {
            Debug.Log($"RestoreTransactions завершён: success={success}, error={error}");

            // Логируем результат восстановления в Firebase Analytics
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

        // На Google Play non-consumable подтягиваются автоматически,
        // но для аналитики всё равно отправляем событие старта процесса.
        FirebaseAnalytics.LogEvent("iap_restore_started");
    }
}