using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class DriftPointsManager : MonoBehaviour
    {
        private Rigidbody rigid;
        private RacerStatistics racerStatistics;
        public DriftPanel driftPanel;
        public DriftRaceSettings driftSettings;

        public bool drifting { get; private set; }
        private float driftAngle;

        [HideInInspector]
        public int driftMultiplier = 1;
        private float lastMultiply;

        [Header("Статистика Дрифта")]
        public float totalDriftPoints; //Общие очки дрифта
        public float currentDriftPoints; //Очки, заработанные за текущий дрифт
        public float currentDriftTime; //Время, в течение которого длился текущий дрифт
        public float bestDrift; //Лучший результат за один дрифт
        public float longestDrift; //Самый длинный дрифт
        private float driftSuccessCounter;
        private float lastCollision;
        private bool countedLastDrift;
        private bool isOfftrack;
        private float lastDistance; //Отслеживание пройденного расстояния, чтобы избежать нечестного дрифта
        private bool isValidDistance; //Является ли текущее расстояние допустимым для дрифта

        void Start()
        {
            rigid = GetComponent<Rigidbody>();
            racerStatistics = GetComponent<RacerStatistics>();
            countedLastDrift = true;

            driftMultiplier = 1;

            //Получение значений из менеджера гонки
            if (RaceManager.instance != null)
            {
                driftSettings = RaceManager.instance.driftRaceSettings;
                lastMultiply = driftSettings.multiplyRate;
            }
        }


        void FixedUpdate()
        {
            //Если мы не находимся в состоянии гонки, выйти
            if (RaceManager.instance != null && RaceManager.instance.raceState != RaceState.Race)
                return;

            //Вычислить скорость Rigidbody для последующей проверки, достаточно ли быстро мы движемся для регистрации дрифта
            float speed = rigid.linearVelocity.magnitude * 3.6f;

            //Проверка выхода за пределы трассы
            isOfftrack = racerStatistics != null && racerStatistics.offTrack ? true : false;

            //Отслеживание пройденного расстояния
            if (racerStatistics != null)
            {
                if (racerStatistics.totalDistance > lastDistance)
                {
                    lastDistance = racerStatistics.totalDistance;
                }

                //Проверка, является ли текущее расстояние допустимым для начисления очков дрифта
                isValidDistance = racerStatistics.totalDistance >= (lastDistance - 10);
            }

            //Вычислить угол дрифта
            Vector3 localVelocity = transform.InverseTransformDirection(rigid.linearVelocity);
            driftAngle = Mathf.Asin(localVelocity.normalized.x) * Mathf.Rad2Deg;
            if (speed < 1) driftAngle = 0;

            //Проверить, находится ли транспортное средство в состоянии дрифта, если выполнены условия
            drifting = Direction() > 1 && Mathf.Abs(driftAngle) >= driftSettings.minAngle
                        && speed >= driftSettings.minDriftSpeed
                        && Time.time > lastCollision && !isOfftrack && isValidDistance;

            if (drifting)
            {
                //В данный момент мы в дрифте, добавляем очки дрифта
                countedLastDrift = false;
                currentDriftPoints += driftSettings.accumulationRate * Time.deltaTime;
                currentDriftTime += Time.deltaTime;
                driftSuccessCounter = 0;

                //Во время дрифта вызываем CountDriftMultiplier(), чтобы проверить, достаточно ли долго мы дрифтовали для получения множителя очков
                CountDriftMultiplier();
            }
            else
            {
                //Мы больше не в дрифте, начинаем отсчёт времени до 'driftSuccessTime'
                if (driftSuccessCounter < driftSettings.driftSuccessTime)
                {
                    driftSuccessCounter += Time.deltaTime;
                }
                else
                {
                    //Время 'driftSuccessTime' превышено, начисляем очки дрифта
                    if (!countedLastDrift)
                    {
                        countedLastDrift = true;
                        CompleteDrift();
                    }
                }
            }

            //Если мы останавливаемся, неудача дрифта
          //  if (speed < 2 && currentDriftPoints > 0)
          //  {
           //     FailDrift();
          //  }

            //Если мы выходим за пределы трассы, неудача дрифта
            if (isOfftrack && currentDriftPoints > 0)
            {
                FailDrift();
            }
        }


        void CompleteDrift()
        {
            if (currentDriftPoints <= 0)
                return;

            //Сохранить самый длинный дрифт
            if (currentDriftTime > longestDrift)
            {
                longestDrift = currentDriftTime;
            }

            //Сохранить лучший дрифт
            if (bestDrift < currentDriftPoints)
            {
                bestDrift = currentDriftPoints * driftMultiplier;
            }

            //Добавить к общим очкам дрифта
            totalDriftPoints += (int)currentDriftPoints * driftMultiplier;

            //Установить общий счёт гонщикам
            if (racerStatistics != null)
            {
                racerStatistics.scorePoints = totalDriftPoints;
            }

            //Обновить панель UI
            if (driftPanel != null)
            {
                driftPanel.UpdateTotalDriftPoints();
            }

            //Сбросить другие значения
            currentDriftPoints = 0;
            currentDriftTime = 0;
            driftMultiplier = 1;
            lastMultiply = driftSettings.multiplyRate;
        }


        void FailDrift()
        {
            //Обновить панель UI
            if (driftPanel != null)
            {
                driftPanel.UpdateDriftFailInfo();
            }

            //Сбросить значения
            currentDriftPoints = 0;
            currentDriftTime = 0;
            driftMultiplier = 1;
            lastMultiply = driftSettings.multiplyRate;
        }


        void CountDriftMultiplier()
        {
            if (driftMultiplier >= driftSettings.maxMultiply)
                return;

            //Увеличить множитель каждые "multiplyRate" секунд
            if (currentDriftTime > lastMultiply)
            {
                lastMultiply += driftSettings.multiplyRate;
                driftMultiplier++;

                //Обновить панель UI
                if (driftPanel != null)
                {
                    driftPanel.UpdateDriftMultipier();
                }
            }
        }


        void OnCollisionEnter(Collision col)
        {
            //Неудача дрифта, если транспортное средство сталкивается с силой >= minCollisionForce
            if (Time.time > lastCollision)
            {
                lastCollision = Time.time + 1.0f;

                if (currentDriftTime > 0 && col.relativeVelocity.magnitude >= driftSettings.minCollisionForce)
                {
                    FailDrift();
                }
            }
        }


        void FindDriftPanel()
        {
            driftPanel = FindObjectOfType<DriftPanel>();
        }


        float Direction()
        {
            return transform.InverseTransformDirection(rigid.linearVelocity).z;
        }


        void OnEnable()
        {
            RaceManager.OnRaceStart += FindDriftPanel;
            RaceManager.OnPlayerFinish += CompleteDrift;
        }


        void OnDisable()
        {
            RaceManager.OnRaceStart -= FindDriftPanel;
            RaceManager.OnPlayerFinish -= CompleteDrift;
        }
    }


    [System.Serializable]
    public class DriftRaceSettings
    {
        public float minAngle = 20f; //Минимальный угол автомобиля для начала дрифта
        public float minDriftSpeed = 20.0f; //Минимальная скорость (в км/ч), с которой автомобиль должен двигаться для начала дрифта
        public float driftSuccessTime = 3.0f; //Время после окончания дрифта для начисления очков
        public float minCollisionForce = 5.0f; //Минимальная сила столкновения, которая приводит к неудаче дрифта
        public float accumulationRate = 500; //Количество очков, добавляемых в секунду во время дрифта
        public float multiplyRate = 5.0f; //Время для увеличения множителя дрифта
        public int maxMultiply = 10; //Максимальный множитель дрифта
    }
}
