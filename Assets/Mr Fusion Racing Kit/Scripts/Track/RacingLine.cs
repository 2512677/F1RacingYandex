using UnityEngine;
using System.Collections;

namespace RGSK
{
    // Класс RacingLine наследуется от TrackSpline и отвечает за расчёт и визуализацию гоночной линии
    public class RacingLine : TrackSpline
    {
        // Массив узлов гоночной линии
        private RacingLineNode[] racingLineNodes;

        // Параметры для проекции цели (расстояния, на которых будет рассчитываться цель)
        public float minTargetDistance = 10; // Минимальное расстояние до цели
        public float maxTargetDistance = 50; // Максимальное расстояние до цели

        // Значения скорости для узлов, рассчитываемые автоматически
        public float minSpeed = 50;   // Минимальная скорость, применяемая при крутых поворотах
        public float maxSpeed = 100;  // Максимальная скорость, применяемая на прямых участках
        public float cautionAngle = 50; // Угол, при котором начинается снижение скорости

        // Метод Start вызывается при запуске сцены
        void Start()
        {
            // Получаем все узлы гоночной линии и преобразуем их в массив
            racingLineNodes = GetRaceLineNodes().ToArray();
        }

        // Метод для получения целевой скорости узла по индексу
        public float GetSpeedAtNode(int index)
        {
            return racingLineNodes[index].targetSpeed;
        }

        // Метод для расчёта скоростей для каждого узла трассы на основе угла поворота
        public void CalculateNodeSpeeds()
        {
            // Корректировка поворотов узлов (наследуемый метод из TrackSpline)
            AdjustNodeRotation();

            // Обновляем массив узлов после корректировки
            racingLineNodes = GetRaceLineNodes().ToArray();

            // Проходим по всем узлам
            for (int i = 0; i < racingLineNodes.Length; i++)
            {
                // Пропускаем первый узел, так как для него нет предыдущего узла для сравнения
                if (i > 0)
                {
                    // Вычисляем вектор направления от предыдущего узла к текущему
                    Vector3 direction = racingLineNodes[i].transform.position
                                        - racingLineNodes[i - 1].transform.position;

                    // Вычисляем угол между направлением движения и "лицом" текущего узла
                    float angle = Vector3.Angle(direction, racingLineNodes[i].transform.forward);

                    // Нормализуем угол в диапазоне от 0 до cautionAngle (получаем значение от 0 до 1)
                    float ratio = Mathf.InverseLerp(0, cautionAngle, angle);

                    // Интерполируем скорость между maxSpeed и minSpeed в зависимости от угла
                    float nodeSpeed = Mathf.Lerp(maxSpeed, minSpeed, ratio);

                    // Устанавливаем рассчитанную скорость для предыдущего узла
                    racingLineNodes[i - 1].targetSpeed = nodeSpeed;
                }
            }

            // Последнему узлу присваиваем скорость, равную скорости предпоследнего узла, чтобы избежать ошибок
            racingLineNodes[racingLineNodes.Length - 1].targetSpeed = racingLineNodes[racingLineNodes.Length - 2].targetSpeed;
        }

        // Переопределённый метод для отрисовки Gizmos в сцене (для визуализации)
        public override void DrawGizmos()
        {
            // Вызываем базовую реализацию для отрисовки
            base.DrawGizmos();

            // Если узлов всего один, визуализируем его в виде проводной сферы
            if (nodes.Count == 1)
            {
                Gizmos.color = color; // Устанавливаем цвет Gizmos в соответствии с параметром color
                Gizmos.DrawWireSphere(transform.GetChild(0).position, 0.5f); // Рисуем сферу радиусом 0.5
            }
        }
    }
}
