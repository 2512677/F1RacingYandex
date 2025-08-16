using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using RGSK;                         // DataContainer / PlayerData


/// <summary>
/// Сохранение и загрузка данных через Unity Cloud Save.
/// Сейчас используется только анонимный вход; привязок к сторонним провайдерам нет.
/// </summary>
public class CloudSaveManager : MonoBehaviour
{
    [Header("UI для сообщений")]
    [SerializeField] private Text statusText;
    [SerializeField] private float messageDuration = 2f;

    private const string PlayerDataKey = "playerData";
    private bool isInitialized;

    // ──────────────────────────────────────────────────────────────────────
    // ЖИЗНЕННЫЙ ЦИКЛ
    // ──────────────────────────────────────────────────────────────────────
    async void Start()
    {
        await InitializeUnityServices();              // инициализируем Unity Services
    }

    // ──────────────────────────────────────────────────────────────────────
    // ПУБЛИЧНЫЕ API (Save / Load)
    // ──────────────────────────────────────────────────────────────────────
    /// <summary>Сохраняет объект «data» под ключом «key».</summary>
    public async Task<bool> SaveGameAsync(string key, object data)
    {
        if (!await EnsureReady()) return false;
        ShowMessage("Saving data…");

        try
        {
            string json = JsonUtility.ToJson(data);
            var record = new Dictionary<string, object> { { key, json } };
            await CloudSaveService.Instance.Data.ForceSaveAsync(record);
            ShowMessage("Save completed");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Cloud save error: " + ex.Message);
            ShowMessage("Save failed");
            return false;
        }
    }


    /// <summary>Загружает данные под ключом «key». Возвращает null, если нет сохранений.</summary>
    public async Task<T> LoadGameAsync<T>(string key) where T : class
    {
        if (!await EnsureReady()) return null;
        ShowMessage("Loading data…");

        try
        {
            var keys = new HashSet<string> { key };
            var data = await CloudSaveService.Instance.Data.LoadAsync(keys);

            if (data != null && data.ContainsKey(key))
            {
                var result = JsonUtility.FromJson<T>(data[key].ToString());
                ShowMessage("Load completed");
                return result;
            }

            ShowMessage("No cloud data");
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError("Cloud load error: " + ex.Message);
            ShowMessage("Load failed");
            return null;
        }
    }


    // ──────────────────────────────────────────────────────────────────────
    // ОБЁРТКИ ДЛЯ КНОПОК UI (оставлены как были)
    // ──────────────────────────────────────────────────────────────────────
    public void SaveGameButton() => _ = SaveGameButtonWrapper();
    public void LoadGameButton() => _ = LoadGameButtonWrapper();

    private async Task SaveGameButtonWrapper()
    {
        if (PlayerData.instance != null)
            await SaveGameAsync(PlayerDataKey, PlayerData.instance.playerData);
        else
            ShowMessage("PlayerData not found");
    }

    // CloudSaveManager.cs
    private async Task LoadGameButtonWrapper()
    {
        if (PlayerData.instance == null)
        {
            ShowMessage("PlayerData not found");
            return;
        }

        var loaded = await LoadGameAsync<DataContainer>(PlayerDataKey);
        if (loaded != null)
        {
            PlayerData.instance.playerData = loaded;
            PlayerData.instance.SaveData();          // локальный файл
            PlayerData.instance.RaiseDataLoadedEvent();

            // восстановить выбранную в момент сохранения машину
            var inst = UnityEngine.Object.FindObjectOfType<MenuVehicleInstantiator>();
            if (inst) inst.LoadPlayerVehicle();     // активируем авто из playerData.vehicleID

            UnityEngine.Object.FindObjectOfType<VehicleSelectionPanel>()?.UpdateVehicleInformation();
            UnityEngine.Object.FindObjectOfType<PlayerSettings>()?.UpdateUIToMatchSettings();

            // обновить UI карьеры
            var cm = CareerManager.instance;
            if (cm) cm.UpdateEventUI();

            // дополнительно гарантируем синхронизацию UI
            FindObjectOfType<PlayerSettings>()?.UpdateUIToMatchSettings();
            FindObjectOfType<VehicleSelectionPanel>()?.UpdateVehicleInformation();
        }
    }


    // ──────────────────────────────────────────────────────────────────────
    // ВНУТРЕННЯЯ ЛОГИКА
    // ──────────────────────────────────────────────────────────────────────
    /// <summary>Инициализирует Unity Services один раз.</summary>
    private async Task InitializeUnityServices()
    {
        if (isInitialized) return;

        try
        {
            await UnityServices.InitializeAsync();
            isInitialized = true;
            Debug.Log("Unity Services initialized");
        }
        catch (Exception ex)
        {
            Debug.LogError("Services init error: " + ex.Message);
            ShowMessage("Init error");
        }
    }

    /// <summary>Проверяет, что сервисы готовы и выполнен анонимный вход.</summary>
    private async Task<bool> EnsureReady()
    {
        if (!isInitialized)
            await InitializeUnityServices();

        if (!isInitialized)          // инициализировать не удалось
            return false;

        if (AuthenticationService.Instance.IsSignedIn)
            return true;             // уже вошли

        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Signed in anonymously – PlayerID: {AuthenticationService.Instance.PlayerId}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Anon auth error: " + ex.Message);
            ShowMessage("Auth error");
            return false;
        }
    }

    // ──────────────────────────────────────────────────────────────────────
    // UI helper
    // ──────────────────────────────────────────────────────────────────────
    private void ShowMessage(string msg)
    {
        if (statusText == null) return;
        StopAllCoroutines();
        StartCoroutine(ShowMessageRoutine(msg));
    }

    private IEnumerator ShowMessageRoutine(string msg)
    {
        statusText.text = msg;
        yield return new WaitForSeconds(messageDuration);
        statusText.text = string.Empty;
    }
}
