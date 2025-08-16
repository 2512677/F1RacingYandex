using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RGSK
{
    // Класс RacingLineMesh отвечает за генерацию и управление мешем для гоночной линии
    public class RacingLineMesh : MonoBehaviour
    {
        // Ссылка на компонент RacingLine, содержащий данные о трассе
        public RacingLine racingLine;
        // Префаб, который будет использоваться для создания сегментов линии
        public GameObject meshPrefab;
        // Расстояние между сегментами линии
        public float spacing = 1;
        // Смещение по вертикали, чтобы линия немного приподнималась над землёй
        public float groundOffset = 0.02f;

        // Метод для генерации меша гоночной линии
        public void GenerateRaceLine()
        {
            // Если трасса или префаб не заданы, выход из метода
            if (racingLine == null || meshPrefab == null) return;

            // Создаем новый объект для меша линии и делаем его дочерним текущему объекту
            GameObject raceLineMesh = new GameObject("RaceLineMesh");
            raceLineMesh.transform.parent = transform;
            RaycastHit hit;

            // Проходим по всей длине трассы с шагом spacing
            for (float i = 0; i < (int)racingLine.length; i += spacing)
            {
                // Создаем копию префаба для сегмента линии
                GameObject line = Instantiate(meshPrefab);
                // Получаем позицию и направление сегмента по кривой трассы
                Vector3 position = racingLine.GetRoutePoint(i).position;
                Quaternion rotation = Quaternion.LookRotation(racingLine.GetRoutePoint(i).direction);

                // Устанавливаем позицию, поворот и делаем сегмент дочерним объектом raceLineMesh
                line.transform.position = position;
                line.transform.rotation = rotation;
                line.transform.parent = raceLineMesh.transform;

                // Выполняем лучевой каст вниз для корректного позиционирования относительно земли
                if (Physics.Raycast(new Ray(line.transform.position, -line.transform.up), out hit))
                {
                    // Если луч пересекается с землей, корректируем позицию с учетом groundOffset
                    line.transform.position = hit.point + new Vector3(0, groundOffset, 0);
                }
            }
        }

        // Метод для удаления сгенерированного меша трассы
        public void DeleteRaceLine()
        {
            // Если дочерний объект с именем "RaceLineMesh" существует, удаляем его
            if (transform.Find("RaceLineMesh"))
            {
                DestroyImmediate(transform.Find("RaceLineMesh").gameObject);
            }
        }

        // Метод для объединения отдельных мешей в один для оптимизации производительности
        public void CombineMeshes()
        {
            // Получаем все компоненты MeshFilter из объекта "RaceLineMesh"
            MeshFilter[] meshFilters = transform.Find("RaceLineMesh").GetComponentsInChildren<MeshFilter>();
            // Создаем массив для хранения данных об объединении мешей
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            while (i < meshFilters.Length)
            {
                if (meshFilters[i] != null)
                {
                    // Заполняем массив данными о меше и его трансформации
                    combine[i].mesh = meshFilters[i].sharedMesh;
                    combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                    // Деактивируем исходный объект меша, чтобы он не отображался отдельно
                    meshFilters[i].gameObject.SetActive(false);
                    i++;
                }
            }

            // Создаем новый объект для объединенного меша и добавляем ему компонент MeshFilter
            MeshFilter racingLineMeshCombined = new GameObject("Racing Line Mesh").AddComponent<MeshFilter>();
            racingLineMeshCombined.mesh = new Mesh();
            // Объединяем все собранные меши в один
            racingLineMeshCombined.mesh.CombineMeshes(combine);
        }
    }
}
