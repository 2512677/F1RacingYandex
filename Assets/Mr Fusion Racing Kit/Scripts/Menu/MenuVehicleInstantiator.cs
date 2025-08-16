using System;
using System.Collections.Generic;
using UnityEngine;
using static CarClass;          // чтобы писать VehicleClass без полного имени

namespace RGSK
{
    public class MenuVehicleInstantiator : MonoBehaviour
    {
        // -----------------
        // Singleton Setup
        // -----------------
        public static MenuVehicleInstantiator Instance { get; private set; }
        private VehicleClass currentFilter = VehicleClass.None;   // активный фильтр
        private List<int> filteredIndices = new List<int>();      // индексы машин, попавших под фильтр

        [Serializable]
        public class MenuVehicle
        {
            public GameObject vehicle;
            public VehicleDatabase.VehicleData vehicleData;

            public MenuVehicle(GameObject _vehicle, VehicleDatabase.VehicleData _vehicleData)
            {
                vehicle = _vehicle;
                vehicleData = _vehicleData;
            }
        }

        [HideInInspector]
        public List<MenuVehicle> menuVehicles = new List<MenuVehicle>();

        private int vehicleIndex;

        // Ссылка на базу автомобилей (через GlobalSettings)
        public VehicleDatabase vehicleDatabase => GlobalSettings.Instance.vehicleDatabase;

        private void Awake()
        {
            // Инициализация синглтона
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }


        // MenuVehicleInstantiator.cs
        public void FilterByClass(CarClass.VehicleClass cls)
        {
            // показаем / скрываем объекты
            for (int i = 0; i < menuVehicles.Count; i++)
            {
                bool show = (cls == CarClass.VehicleClass.None)        // “All”
                           || menuVehicles[i].vehicleData.CarClass == cls;

                menuVehicles[i].vehicle.SetActive(show);
            }

            // сдвигаем указатель на первый видимый автомобиль
            vehicleIndex = menuVehicles.FindIndex(v => v.vehicle.activeSelf);
            if (vehicleIndex < 0) vehicleIndex = 0;
        }

        /// <summary>
        /// Включает фильтр по классу. Передаём VehicleClass.None, чтобы показать “всё”.
        /// </summary>
        public void ApplyClassFilter(VehicleClass filter)
        {
            currentFilter = filter;
            filteredIndices.Clear();

            // 1. Выключаем ВСЕ машины
            foreach (var mv in menuVehicles)
                mv.vehicle.SetActive(false);

            // 2. Заполняем список подходящих индексов
            for (int i = 0; i < menuVehicles.Count; i++)
            {
                if (filter == VehicleClass.None ||
                    menuVehicles[i].vehicleData.CarClass == filter)
                {
                    filteredIndices.Add(i);
                }
            }

            // 3. Если ничего не подошло — выходим
            if (filteredIndices.Count == 0)
            {
                Debug.LogWarning($"[MenuVehicleInstantiator] Нет машин класса {filter}");
                return;
            }

            // 4. Включаем первую подходящую
            vehicleIndex = filteredIndices[0];
            menuVehicles[vehicleIndex].vehicle.SetActive(true);
            ApplySavedColor(menuVehicles[vehicleIndex].vehicleData);

            // -------- Добавлено: обновляем панель выбора, чтобы кнопки сразу показались корректно
            VehicleSelectionPanel panel = FindObjectOfType<VehicleSelectionPanel>();
            if (panel != null) panel.UpdateVehicleInformation();
        }

        /// <summary>
        /// Листаем машины. Теперь учитываем активный фильтр.
        /// </summary>



        private void Start()
        {
            // Создаём все автомобили и скрываем их
            InstantiateVehicles();

            // Активируем и красим машину, которая записана в playerData.vehicleID
            LoadPlayerVehicle();
        }

        /// <summary>
        /// Создать объекты всех автомобилей и добавить в список.
        /// </summary>
        private void InstantiateVehicles()
        {
            if (vehicleDatabase == null)
            {
                Debug.LogError("Vehicle database is not assigned.");
                return;
            }

            // Проходимся по всем машинам в БД
            foreach (var vehicleData in vehicleDatabase.vehicles)
            {
                // Создаём prefab для меню
                GameObject vehicleObject = Instantiate(vehicleData.menuVehicle, transform.position, transform.rotation, transform);

                // Заворачиваем в нашу структуру MenuVehicle
                menuVehicles.Add(new MenuVehicle(vehicleObject, vehicleData));

                // По умолчанию скрываем
                vehicleObject.SetActive(false);

                // Если у нас есть PlayerData, проставляем флаг isLocked
                if (PlayerData.instance != null)
                {
                    vehicleData.isLocked = !PlayerData.instance.IsItemUnlocked(vehicleData.uniqueID);
                }
            }
        }

        /// <summary>
        /// Активирует машину из playerData.vehicleID и красит её в сохранённый цвет (или стоковый).
        /// </summary>
        public void LoadPlayerVehicle()
        {
            // Если почему-то PlayerData нет, просто включим первую машину (если она есть)
            if (PlayerData.instance == null)
            {
                if (menuVehicles.Count > 0)
                {
                    menuVehicles[0].vehicle.SetActive(true);
                    vehicleIndex = 0;
                }
                return;
            }

            bool vehicleFound = false;

            // Перебираем все машины, чтобы найти ту, что совпадает с playerData.vehicleID
            for (int i = 0; i < menuVehicles.Count; i++)
            {
                string vehicleID = menuVehicles[i].vehicleData.uniqueID;

                // Если машина разблокирована, снимаем флаг isLocked
                if (PlayerData.instance.IsItemUnlocked(vehicleID))
                {
                    menuVehicles[i].vehicleData.isLocked = false;
                }

                // Проверяем, совпадает ли эта машина с выбранной у игрока
                if (vehicleID == PlayerData.instance.playerData.vehicleID)
                {
                    vehicleIndex = i;
                    menuVehicles[i].vehicle.SetActive(true);
                    vehicleFound = true;

                    // Загружаем цвет (через PlayerData)
                    ApplySavedColor(menuVehicles[i].vehicleData);
                }
                else
                {
                    // Остальные машины выключаем
                    menuVehicles[i].vehicle.SetActive(false);
                }
            }

            // Если playerData.vehicleID не совпал ни с одной машиной, включаем первую
            if (!vehicleFound && menuVehicles.Count > 0)
            {
                menuVehicles[0].vehicle.SetActive(true);
                vehicleIndex = 0;

                // Красим её
                ApplySavedColor(menuVehicles[0].vehicleData);
            }
        }

        /// <summary>
        /// Переключение машин (вперёд или назад).
        /// Здесь мы красим «новую» машину, чтобы она брала свой сохранённый цвет.
        /// </summary>
        public void CycleVehicles(int dir)
        {
            // Каким списком пользуемся: полным или отфильтрованным
            List<int> list = currentFilter == VehicleClass.None ? null : filteredIndices;

            // гасим текущую
            menuVehicles[vehicleIndex].vehicle.SetActive(false);

            if (list == null)
            {
                // обычный режим “все машины”
                vehicleIndex = Mathf.Clamp(vehicleIndex + dir, 0, menuVehicles.Count - 1);
            }
            else
            {
                // листаем ТОЛЬКО в пределах filteredIndices
                int localPos = list.IndexOf(vehicleIndex);
                localPos = Mathf.Clamp(localPos + dir, 0, list.Count - 1);
                vehicleIndex = list[localPos];
            }

            // показываем новую
            menuVehicles[vehicleIndex].vehicle.SetActive(true);
            ApplySavedColor(menuVehicles[vehicleIndex].vehicleData);
        }

        /// <summary>
        /// Сохранить выбранную машину (записать её uniqueID в playerData.vehicleID).
        /// </summary>
        public void SetSelectedVehicle()
        {
            if (PlayerData.instance != null)
            {
                string id = GetVehicleData().uniqueID;
                PlayerData.instance.SaveSelectedVehicle(id);
            }
        }


        public GameObject GetCurrentVehicle()
        {
            if (menuVehicles != null && menuVehicles.Count > 0)
                return menuVehicles[vehicleIndex].vehicle;
            return null;
        }


        /// <summary>
        /// Установить цвет для текущей (активной) машины и сохранить в PlayerData.
        /// </summary>
        public void SetSelectedVehicleColor(Color color)
        {
            var vehicleData = GetVehicleData();
            if (vehicleData.bodyMaterials != null && vehicleData.bodyMaterials.Length > 0)
            {
                // Крась все материалы
                foreach (var material in vehicleData.bodyMaterials)
                {
                    material.color = color;
                }

                // Сохраняем цвет в PlayerData (для future load)
                PlayerData.instance.SetVehicleColor(vehicleData.uniqueID, color);
            }
        }

        /// <summary>
        /// Применить сохранённый цвет к машине (или взять стоковый, если нет сохранённого).
        /// </summary>
        private void ApplySavedColor(VehicleDatabase.VehicleData vehicleData)
        {
            if (vehicleData == null)
                return;

            if (vehicleData.bodyMaterials != null && vehicleData.bodyMaterials.Length > 0)
            {
                // Берём сохранённый цвет (или stockColor)
                Color loadedColor = Color.white;

                if (PlayerData.instance != null)
                {
                    loadedColor = PlayerData.instance.GetVehicleColor(vehicleData.uniqueID, vehicleData.stockColor);
                }
                else
                {
                    // На случай отсутствия PlayerData берём стоковый
                    loadedColor = vehicleData.stockColor;
                }

                // Назначаем всем материалам
                foreach (var material in vehicleData.bodyMaterials)
                {
                    material.color = loadedColor;
                }
            }
        }

        /// <summary>
        /// Вернуть машину игрока (если надо перезагрузить состояние).
        /// </summary>
        public void RevertPlayerVehicle()
        {
            if (PlayerData.instance == null)
                return;

            for (int i = 0; i < menuVehicles.Count; i++)
            {
                if (menuVehicles[i].vehicleData.uniqueID == PlayerData.instance.playerData.vehicleID)
                {
                    vehicleIndex = i;
                    menuVehicles[i].vehicle.SetActive(true);
                    ApplySavedColor(menuVehicles[i].vehicleData);
                }
                else
                {
                    menuVehicles[i].vehicle.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Получить данные о машине, которая сейчас выбрана (vehicleIndex).
        /// </summary>
        public VehicleDatabase.VehicleData GetVehicleData()
        {
            if (menuVehicles.Count == 0)
            {
                return null;
            }
            return menuVehicles[vehicleIndex].vehicleData;
        }

        public bool IsLastVehicleInList()
        {
            return vehicleIndex == menuVehicles.Count - 1;
        }

        public bool IsFirstVehicleInList()
        {
            return vehicleIndex == 0;
        }

        public bool HasVehicleDatabase()
        {
            return vehicleDatabase != null;
        }
    }
}
