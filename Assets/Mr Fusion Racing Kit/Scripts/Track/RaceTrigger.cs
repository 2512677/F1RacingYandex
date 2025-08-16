using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class RaceTrigger : MonoBehaviour
    {
        public RaceTriggerType triggerType;
        public TrackNode nearestTrackNode; //Ближайший узел к этому триггеру - должен быть за триггером
        public int index; //Индекс этого триггера должен быть уникальным среди одинаковых типов триггеров
        public float addedTime = 10; //Время добавляется, когда транспортное средство проходит эту контрольную точку (применимо только к типу гонки «Контрольная точка»)

        void Awake()
        {
            //Установить слой этого игрового объекта на игнорирование raycast
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }


        public float GetValidDistance()
        {
            if (nearestTrackNode != null)
                return nearestTrackNode.distanceAtNode;

            return 0;
        }
    }
}
