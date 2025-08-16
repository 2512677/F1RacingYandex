using UnityEngine;
using UnityEditor;
using RGSK;
using UnityEditor.SceneManagement;
using System.IO;

public class Editor_RGSK : Editor
{
    //---Race Components---
    [MenuItem("MR Fusuin Engine/Настройка сцены/Компоненты гонки")]
    public static void CreateTrackManagers()
    {
        GameObject template = (GameObject)Resources.Load("RGSK/Templates/RaceComponents");
        if (template != null)
        {
            GameObject clone = Instantiate(template);
            clone.name = "RaceComponents";
            Undo.RegisterCreatedObjectUndo(clone, "Созданные гоночные компоненты");
        }
        else
        {
            Debug.LogWarning("Шаблон компонентов гонки не найден! Рассмотрите возможность повторного импорта Ассета");
        }
    }


    //---Track Editor---
    [MenuItem("MR Fusuin Engine/Настройка сцены/Редактор трека")]
    public static void CreateTrackEditor()
    {
        GameObject template = (GameObject)Resources.Load("RGSK/Templates/TrackEditor");
        if (template != null)
        {
            GameObject clone = Instantiate(template);
            clone.name = "TrackEditor";
            Undo.RegisterCreatedObjectUndo(clone, "Создан Редактор трека");
        }
        else
        {
            Debug.LogWarning("Шаблон редактора треков не найден! Рассмотрите возможность повторного импорта Ассета");
        }
    }


    //---Race UI Template---
    [MenuItem("MR Fusuin Engine/Настройка сцены/Интерфейс/Интерфейса гоночной сцены")]
    public static void CreateRaceSceneUI()
    {
        GameObject raceUI = (GameObject)Resources.Load("RGSK/Templates/RaceCanvas");
        if (raceUI == null)
        {
            Debug.LogWarning("Не удалось найти шаблон интерфейса сцены гонки. Рассмотрите возможность повторного импорта Ассета");
            return;
        }

        GameObject clone = (GameObject)Instantiate(raceUI);
        clone.name = "RaceCanvas";
        Undo.RegisterCreatedObjectUndo(clone, "Создано RaceCanvas");
    }


    //---Race UI Template---
    [MenuItem("MR Fusuin Engine/Настройка сцены/Интерфейс/Интерфейса Дрифт сцены")]
    public static void CreateDriftSceneUI()
    {
        GameObject raceUI = (GameObject)Resources.Load("RGSK/Templates/DriftCanvas");
        if (raceUI == null)
        {
            Debug.LogWarning("Не удалось найти шаблон интерфейса сцены гонки. Рассмотрите возможность повторного импорта Ассета");
            return;
        }

        GameObject clone = (GameObject)Instantiate(raceUI);
        clone.name = "DriftCanvas";
        Undo.RegisterCreatedObjectUndo(clone, "Создано DriftCanvas");
    }


    //---Menu UI Template---
    [MenuItem("MR Fusuin Engine/Настройка сцены/Интерфейс/Интерфейса сцены меню")]
    public static void CreateMenuSceneUI()
    {
        GameObject menuUI = (GameObject)Resources.Load("RGSK/Templates/MenuCanvas");
        if (menuUI == null)
        {
            Debug.LogWarning("Не удалось найти шаблон интерфейса сцены меню. Рассмотрите возможность повторного импорта Ассета");
            return;
        }

        GameObject clone = (GameObject)Instantiate(menuUI);
        clone.name = "MenuCanvas";
        Undo.RegisterCreatedObjectUndo(clone, "Создано MenuCanvas");
    }


    //---Mobile UI Template---
    [MenuItem("MR Fusuin Engine/Настройка сцены/Интерфейс/Шаблон мобильного управления")]
    public static void CreateMobileControlUI()
    {
        RacePanel panel = FindObjectOfType<RacePanel>();
        if (panel == null)
        {
            Debug.LogWarning("Пожалуйста, создайте шаблон интерфейса гоночной сцены перед созданием мобильных элементов управления..");
            return;
        }

        GameObject mobileUI = (GameObject)Resources.Load("RGSK/Templates/MobileControlPanel");
        if (mobileUI == null)
        {
            Debug.LogWarning("Не удалось найти шаблон мобильного управления. Рассмотрите возможность повторного импорта Ассета");
            return;
        }

        GameObject clone = (GameObject)Instantiate(mobileUI, panel.transform, false);
        clone.name = "MobileControlPanel";
        Undo.RegisterCreatedObjectUndo(clone, "Создано MobileControlPanel");
    }


    //---Input Manager---
    [MenuItem("MR Fusuin Engine/Настройка сцены/Управление/менеджер ввода")]
    public static void CreateInputManager()
    {
        GameObject newGameObject = new GameObject("InputManager");
        newGameObject.AddComponent<StandardInputManager>();
        newGameObject.AddComponent<StandardTouchInputManager>();
        Undo.RegisterCreatedObjectUndo(newGameObject, "Создан InputManager");
    }


    //---Audio Manager---
    [MenuItem("MR Fusuin Engine/Настройка сцены/Аудио/Аудиоменеджер")]
    public static void CreateAudioManager()
    {
        GameObject newGameObject = new GameObject("AudioManager");
        newGameObject.AddComponent<AudioManager>();
        Undo.RegisterCreatedObjectUndo(newGameObject, "Создан AudioManager");
    }


    //---Scene Controller---
    [MenuItem("MR Fusuin Engine/Настройка сцены/Загрузчик сцены")]
    public static void CreateSceneController()
    {
        GameObject sceneController = (GameObject)Resources.Load("RGSK/Templates/SceneController");
        if (sceneController != null)
        {
            GameObject clone = Instantiate(sceneController);
            clone.name = "SceneController";
            Undo.RegisterCreatedObjectUndo(clone, "Создан SceneController");
        }
        else
        {
            Debug.LogWarning("Не удалось найти шаблон контроллера сцены. Рассмотрите возможность повторного импорта Ассета");
        }
    }


    //---Player Data---
    [MenuItem("MR Fusuin Engine/Настройка сцены/Данные/Данные игрока")]
    public static void CreatePlayerData()
    {
        GameObject newGameObject = new GameObject("PlayerData");
        PlayerData data = newGameObject.AddComponent<PlayerData>();

        //Create good default values
        data.defaultValues.playerName = "Player";
        data.defaultValues.playerCurrency = 10000;
        data.defaultValues.nextLevelXP = 1000;
        data.defaultValues.nextLevelXpMultiplier = 1.25f;

        Undo.RegisterCreatedObjectUndo(newGameObject, "Создано PlayerData");
    }


    //---Music Player---
    [MenuItem("MR Fusuin Engine/Настройка сцены/Аудио/Музыкальный проигрыватель")]
    public static void CreateMusicPlayer()
    {
        GameObject newGameObject = new GameObject("MusicPlayer");
        newGameObject.AddComponent<MusicPlayer>();
        Undo.RegisterCreatedObjectUndo(newGameObject, "Создать музыкальный плеер");
    }




   // ---Racing Line Mesh---
    [MenuItem("MR Fusuin Engine/Настройка сцены/Нарисовать траекторию дороги")]
    public static void CreateRacingLineMesh()
    {
      GameObject newGameObject = new GameObject("RacingLineMesh");
        newGameObject.AddComponent<RacingLineMesh>();
        Undo.RegisterCreatedObjectUndo(newGameObject, "Создана траектория дороги");
    }


    //---Мастер настройки автомобиля---
    
   // [MenuItem("Окно/Racing Game Starter Kit/Настройка автомобиля/Мастер настройки")]
   // public static void SetupVehicle()
   // {
    //    GUIContent title = new GUIContent("Мастер настройки автомобиля");
    //    Window_VehicleSetup window = EditorWindow.GetWindow(typeof(Window_VehicleSetup)) as Window_VehicleSetup;
   //     window.titleContent = title;
    //}
    


    //---Add Player Components---
    [MenuItem("MR Fusuin Engine/Настройка автомобиля/Преобразовать в игрока")]
    public static void AddPlayerComponentsToVehicle()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            return;
        }

        if (!EditorUtility.DisplayDialog("RGSK",
                    "Это удалит все компоненты ИИ и добавит компоненты игрока. '" + selected.name + "'.", "Продолжать", "Отмена"))
        {
            return;
        }

        selected.tag = "Player";

        AiLogic AILogic = selected.GetComponent<AiLogic>();
        if (AILogic != null)
        {
            DestroyImmediate(AILogic);
        }

        RCCAIInput aiInput = selected.GetComponent<RCCAIInput>();
        if (aiInput != null)
        {
            DestroyImmediate(aiInput);
        }

        if (!selected.GetComponent<RCCPlayerInput>())
        {
            selected.AddComponent<RCCPlayerInput>();
        }


        if (!selected.GetComponent<CarClass>())
        {
            selected.AddComponent<CarClass>();
        }

        if (!selected.GetComponent<RacerStatistics>())
        {
            selected.AddComponent<RacerStatistics>();
        }
        if (!selected.GetComponent<Slipstream>())
        {
            selected.AddComponent<Slipstream>();
        }

        


        Debug.Log("Добавлены компоненты игрока " + selected.name);
    }


    [MenuItem("MR Fusuin Engine/Настройка автомобиля/Преобразовать в Копа")]
    public static void AddPlayerCopCarComponentsToVehicle()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            return;
        }

        if (!EditorUtility.DisplayDialog("RGSK",
                    "Это удалит все компоненты ИИ и добавит компоненты игрока. '" + selected.name + "'.", "Продолжать", "Отмена"))
        {
            return;
        }

        selected.tag = "Player";

        AiLogic AILogic = selected.GetComponent<AiLogic>();
        if (AILogic != null)
        {
            DestroyImmediate(AILogic);
        }

        RCCAIInput aiInput = selected.GetComponent<RCCAIInput>();
        if (aiInput != null)
        {
            DestroyImmediate(aiInput);
        }

        if (!selected.GetComponent<RCCPlayerInput>())
        {
            selected.AddComponent<RCCPlayerInput>();
        }


        if (!selected.GetComponent<CarClass>())
        {
            selected.AddComponent<CarClass>();
        }

        if (!selected.GetComponent<RacerStatistics>())
        {
            selected.AddComponent<RacerStatistics>();
        }
        if (!selected.GetComponent<Slipstream>())
        {
            selected.AddComponent<Slipstream>();
        }

        if (!selected.GetComponent<PursuitTargetManager>())
        {
            selected.AddComponent<PursuitTargetManager>();
        }

        if (!selected.GetComponent<ChaseCar>())
        {
            selected.AddComponent<ChaseCar>();
        }


        Debug.Log("Добавлены компоненты игрока " + selected.name);
    }

    //---Add AI Components---
    [MenuItem("MR Fusuin Engine/Настройка автомобиля/Преобразовать в ИИ")]
    public static void AddAIComponentsToVehicle()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            return;
        }

        if (!EditorUtility.DisplayDialog("RGSK",
                    "Это удалит все компоненты Player и добавит компоненты ИИ. '" + selected.name + "'.", "Продолжать", "Отмена"))
        {
            return;
        }

        selected.tag = "Untagged";

        RCCPlayerInput player = selected.GetComponent<RCCPlayerInput>();
       

        if (player != null)
        {
            DestroyImmediate(player);
        }



        if (!selected.GetComponent<AiLogic>())
        {
            selected.AddComponent<AiLogic>();
        }
        if (!selected.GetComponent<CarClass>())
        {
            selected.AddComponent<CarClass>();
        }

        if (!selected.GetComponent<RCCAIInput>())
        {
            selected.AddComponent<RCCAIInput>();
        }
        if (!selected.GetComponent<RacerStatistics>())
        {
            selected.AddComponent<RacerStatistics>();
        }
        if (!selected.GetComponent<BodyMaterialSelector>())
        {
            selected.AddComponent<BodyMaterialSelector>();
        }
        if (!selected.GetComponent<Slipstream>())
        {
            selected.AddComponent<Slipstream>();
        }

        Debug.Log("Added AI Components to " + selected.name);
    }

    //---Add AI Components---
    [MenuItem("MR Fusuin Engine/Настройка автомобиля/Преобразовать в МЕНЮ")]
    public static void RemovePlayerComponentsFromVehicle()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("Нет выбранных игровых объектов.");
            return;
        }

        if (!EditorUtility.DisplayDialog("RGSK",
                    $"Это удалит все компоненты игрока и ИИ для {selectedObjects.Length} объекта(ов). Продолжить?",
                    "Продолжать",
                    "Отмена"))
        {
            return;
        }

        // Список типов компонентов для удаления
        System.Type[] componentsToRemove = new System.Type[]
        {
        typeof(RCCPlayerInput),
        typeof(RacerStatistics),
        typeof(Slipstream),
        typeof(AiLogic),
        typeof(RCCAIInput),
        typeof(BodyMaterialSelector),
        typeof(RCC_CarControllerV4),
        typeof(PursuitTargetManager),

        };

        foreach (GameObject selected in selectedObjects)
        {
            // Изменение тега на "Untagged" или другой подходящий тег
            selected.tag = "Untagged";

            foreach (var componentType in componentsToRemove)
            {
                Component component = selected.GetComponent(componentType);
                if (component != null)
                {
                    DestroyImmediate(component);
                    Debug.Log($"Удалён компонент {componentType.Name} из {selected.name}");
                }
            }
        }

        Debug.Log($"Все указанные компоненты удалены из {selectedObjects.Length} объекта(ов)");
    }






    //---Reset Player Data---
    [MenuItem("MR Fusuin Engine/Другое/Удалить PlayerPrefs")]
    public static void ResetData()
    {
        if (!EditorUtility.DisplayDialog("RGSK",
                    "Вы уверены, что хотите удалить все PlayerPrefs?", "Да", "Нет"))
        {
            return;
        }

        PlayerPrefs.DeleteAll();
    }


    //---About---
    [MenuItem("MR Fusuin Engine/О MR Fusuin Engine")]
    public static void OpenAboutWindow()
    {
        GUIContent title = new GUIContent("О MR Fusuin Engine");
        Window_About window = EditorWindow.GetWindow(typeof(Window_About)) as Window_About;
        window.titleContent = title;
        window.minSize = new Vector2(480, 600);
        window.maxSize = new Vector2(480, 600);
        window.Show();
    }


    public static void MarkSceneAsDirty()
    {
        if (!Application.isPlaying)
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}