using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Добавлено: для Dictionary

namespace RGSK
{
    public class RespawnTrigger : MonoBehaviour
    {
        // Добавлено: словарь для отслеживания активных корутин респауна по объекту RacerStatistics
        private Dictionary<RacerStatistics, Coroutine> respawnCoroutines = new Dictionary<RacerStatistics, Coroutine>();

        void OnTriggerEnter(Collider other)
        {
            RacerStatistics stats = other.GetComponentInParent<RacerStatistics>();

            if (stats != null)
            {
                // Запускаем корутину для задержанного респауна
                if (!respawnCoroutines.ContainsKey(stats))
                {
                    Coroutine coroutine = StartCoroutine(RespawnWithDelay(stats.transform));
                    respawnCoroutines.Add(stats, coroutine);
                    // Добавлено: сохраняем корутину, чтобы можно было её остановить при выходе из триггера
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            RacerStatistics stats = other.GetComponentInParent<RacerStatistics>();

            if (stats != null)
            {
                // Добавлено: при выходе из триггера отменяем запланированный респаун
                if (respawnCoroutines.TryGetValue(stats, out Coroutine coroutine))
                {
                    StopCoroutine(coroutine);
                    respawnCoroutines.Remove(stats);
                    // Добавлено: отменили корутину респауна для данного объекта
                }
            }
        }

        // Корутина для ожидания 3 секунд до респауна
        IEnumerator RespawnWithDelay(Transform vehicleTransform)
        {
            // Ждем 3 секунды
            yield return new WaitForSeconds(3f);

            // Добавлено: проверяем, что корутина всё ещё активна и объект остался в триггере
            RacerStatistics stats = vehicleTransform.GetComponentInParent<RacerStatistics>();
            if (stats != null && respawnCoroutines.ContainsKey(stats))
            {
                // Выполняем респаун транспортного средства
                RaceManager.instance.RespawnVehicle(vehicleTransform);
                // Добавлено: после респауна убираем запись из словаря
                respawnCoroutines.Remove(stats);
            }
        }
    }
}
