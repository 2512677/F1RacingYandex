using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class start_load : MonoBehaviour
{
    public Canvas loading_info;
    public Text tip_message;
    public Text loading_percentage;
    public Text versionText; // UI Text для отображения версии билда
    public string[] tips;

    private IEnumerator Start()
    {
        // Устанавливаем целевой FPS
        Application.targetFrameRate = 60;

        // Устанавливаем текст с версией билда, если ссылка на UI элемент задана
        if (versionText != null)
        {
            versionText.text = "Версия билда: " + Application.version;
            // Если хотите использовать версию из RCC_Settings:
            // versionText.text = "Версия билда: " + RCC_Settings.Instance.RCCVersion;
        }

        // Показываем случайный совет из списка tips
        if (tips.Length > 0)
        {
            tip_message.text = tips[Random.Range(0, tips.Length)];
        }

        // Короткая задержка перед началом загрузки
        yield return new WaitForSeconds(3f);

        // Активируем Canvas с информацией о загрузке
        loading_info.gameObject.SetActive(true);

        // Начинаем асинхронную загрузку сцены
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Main Menu");
        asyncOperation.allowSceneActivation = false;

        // Обновляем отображение процента загрузки
        while (!asyncOperation.isDone)
        {
            // Вычисляем процент загрузки (сцена достигает 90%, затем до 100%)
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f) * 100;
            loading_percentage.text = $"{Mathf.RoundToInt(progress)}";

            // Когда загрузка достигает 90%, завершаем загрузку
            if (asyncOperation.progress >= 0.9f)
            {
                loading_percentage.text = "100"; // Отображаем 100%, когда загрузка завершена
                yield return new WaitForSeconds(1f); // Короткая задержка перед активацией сцены
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
