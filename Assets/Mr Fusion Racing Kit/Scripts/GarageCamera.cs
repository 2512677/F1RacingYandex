using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageCamera : MonoBehaviour 
{
    public Transform target;
    Transform currentTarget;
    
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;
    
    public Transform[] points;
    
    // Ссылка на компонент Animator, который отвечает за анимацию камеры (Camera_animator)
    public Animator cameraAnimator;
    // Флаг, определяющий, находится ли камера в меню гаража
    public bool isGarageMenu = false;
    
    void Start () 
    {
        currentTarget = points[0];
    }
    
    void Update() 
    {
        // Если мы не в меню гаража, анимация включается, иначе – отключается
        if(cameraAnimator != null)
        {
            cameraAnimator.enabled = !isGarageMenu;
        }
    
        // Перемещение камеры к текущей цели с интерполяцией
        transform.position = Vector3.Lerp(transform.position, currentTarget.position, smoothTime * Time.deltaTime);
        transform.LookAt(target.position);
    }
    
    // Метод для переключения между целевыми точками
    public void SwitchTarget(int id)
    {
        currentTarget = points[id];
    }
    
    // Дополнительный метод, чтобы можно было из других скриптов задать состояние меню гаража
    public void SetGarageMenuState(bool inGarage)
    {
        isGarageMenu = inGarage;
    }
}
