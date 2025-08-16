using UnityEngine;
using System.Collections;

namespace RGSK
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Изменяет слой всех коллайдеров в gameObject, игнорируя триггеры
        /// </summary>
        public static void SetColliderLayer(this GameObject _gameObject, string layerName)
        {
            foreach (Collider colliders in _gameObject.GetComponentsInChildren<Collider>())
            {
                if (!colliders.isTrigger)
                {
                    colliders.gameObject.layer = LayerMask.NameToLayer(layerName);
                }
            }
        }


        /// <summary>
        ///Заменяет шейдер всех рендереров в gameObject
        /// </summary>
        public static void ChangeRendererMaterials(this GameObject _gameObject, Shader shader)
        {
            foreach (Renderer renderes in _gameObject.GetComponentsInChildren<Renderer>())
            {
                //Если рендерер использует только один материал
                if (renderes.materials.Length == 1)
                {
                    Material instance = renderes.material;
                    instance.shader = shader;
                    Color col = instance.color;
                    col.a = 0.3f;
                    instance.color = col;
                    renderes.material = instance;
                }
                else
                {
                    //Если рендерер использует несколько материалов
                    Material[] instances = new Material[renderes.materials.Length];
                    Color[] col = new Color[renderes.materials.Length];

                    for (int i = 0; i < instances.Length; i++)
                    {
                        instances[i] = renderes.materials[i];
                        instances[i].shader = shader;
                        col[i] = instances[i].color;
                        col[i].a = 0.3f;
                        instances[i].color = col[i];
                        renderes.materials[i] = instances[i];
                    }
                }
            }
        }


        /// <summary>
        /// Заменяет материал всех рендереров в gameObject новым единым материалом
        /// </summary>
        public static void ChangeRendererMaterials(this GameObject _gameObject, Material material)
        {
            foreach (Renderer renderes in _gameObject.GetComponentsInChildren<Renderer>())
            {
                //Если рендерер использует только один материал
                if (renderes.materials.Length == 1)
                {
                    renderes.material = material;
                }
                else
                {
                    //Если рендерер использует несколько материалов
                    Material[] instances = new Material[renderes.materials.Length];
                    for (int i = 0; i < instances.Length; i++)
                    {
                        instances[i] = material;
                        renderes.materials = instances;
                    }
                }
            }
        }
    }
}
