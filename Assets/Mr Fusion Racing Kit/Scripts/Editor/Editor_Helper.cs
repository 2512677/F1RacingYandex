using UnityEngine;
using UnityEditor;
using System.IO;
using RGSK;

// Вспомогательный класс редактора для обновления настроек проекта и получения логотипа ассета
public class Editor_Helper : MonoBehaviour
{
    // Версия ассета
    public static string version = "2.0.0 beta";
    // URL форума ассета
    public static string forumURL = "https://forum.unity3d.com/threads/racing-game-starter-kit-easily-create-racing-games.337366/";
    // URL YouTube-плейлиста ассета
    public static string youtubeURL = "https://www.youtube.com/playlist?list=PLdNzy1P_hi4SQ9qg9Lv1CNC9wa6zs3Va3";
    // URL онлайн-документации ассета
    public static string onlineDocumentationURL = "https://www.dropbox.com/s/oe98mz89m0msf8y/ReadMe.pdf?dl=0";

    // --- Логотип ассета ---
    // Метод для получения логотипа ассета из ресурсов
    public static Texture2D Logo()
    {
        return (Texture2D)Resources.Load("RGSK/Logo/RGSKLogo");
    }

    // --- Обновление настроек проекта ---
    // Метод для обновления файлов настроек проекта из ассета Racing Game Starter Kit
    public static void UpdateProjectSettings()
    {
        // Путь к папке настроек проекта
        string projectSettingsFolder = Application.dataPath.Replace("Assets", "ProjectSettings");
        // Путь к папке с настройками, поставляемыми с Racing Game Starter Kit
        string assetsFolder = Application.dataPath + "/Racing Game Starter Kit 2.0/Other/ProjectSettings";

        if (Directory.Exists(projectSettingsFolder))
        {
            // FileUtil.ReplaceFile не работает корректно, поэтому сначала удаляем, затем копируем .asset файл

            // Обновление InputManager
            if (File.Exists(assetsFolder + "/InputManager.asset"))
            {
                FileUtil.DeleteFileOrDirectory(projectSettingsFolder + "/InputManager.asset");
                FileUtil.CopyFileOrDirectory(assetsFolder + "/InputManager.asset", projectSettingsFolder + "/InputManager.asset");
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogWarning("Не удалось обновить настройки ввода! Убедитесь, что каталог существует: " + assetsFolder + "/InputManager.asset");
                return;
            }

            // Обновление Tags & Layers (тегов и слоёв)
            if (File.Exists(assetsFolder + "/TagManager.asset"))
            {
                FileUtil.DeleteFileOrDirectory(projectSettingsFolder + "/TagManager.asset");
                FileUtil.CopyFileOrDirectory(assetsFolder + "/TagManager.asset", projectSettingsFolder + "/TagManager.asset");
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogWarning("Не удалось обновить теги и слои! Убедитесь, что каталог существует: " + assetsFolder + "/TagManager.asset");
                return;
            }

            // Обновление настроек физики (Dynamic Manager)
            if (File.Exists(assetsFolder + "/DynamicsManager.asset"))
            {
                FileUtil.DeleteFileOrDirectory(projectSettingsFolder + "/DynamicsManager.asset");
                FileUtil.CopyFileOrDirectory(assetsFolder + "/DynamicsManager.asset", projectSettingsFolder + "/DynamicsManager.asset");
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogWarning("Не удалось обновить настройки физики! Убедитесь, что каталог существует: " + assetsFolder + "/DynamicsManager.asset");
                return;
            }

            Debug.Log("Настройки проекта успешно обновлены!");
        }
    }
}
