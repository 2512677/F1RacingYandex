using UnityEngine;

public class Wing_Anim : MonoBehaviour
{
    public GameObject Car;
    public float OpenSpeed = 100f;

    private Animator anim;
    private RCC_CarControllerV4 carController;
    private int OpenHash;

    private void Start()
    {
        // Инициализируем компоненты
        if (Car != null)
        {
            carController = Car.GetComponent<RCC_CarControllerV4>();
            if (carController == null)
            {
                Debug.LogError("RCC_CarControllerV3 component not found on the Car object.");
            }

            anim = GetComponent<Animator>();
            if (anim == null)
            {
                Debug.LogError("Animator component is not found on the wing object.");
            }
        }
        else
        {
            Debug.LogError("Car is not assigned to the Wing_Anim script.");
        }

        // Получаем хеш для параметра анимации "Open"
        OpenHash = Animator.StringToHash("Open");
    }

    private void FixedUpdate()
    {
        if (carController != null && anim != null)
        {
            WingControl();
        }
    }

    private void WingControl()
    {
        float currentSpeed = carController.speed;
        // Выводим текущую скорость в консоль для отладки
      //  Debug.Log("Current Speed: " + currentSpeed);

        if (currentSpeed >= OpenSpeed)
        {
            if (!anim.GetBool(OpenHash))
            {
                anim.SetBool(OpenHash, true); // Открываем крыло
              //  Debug.Log("Opening wing.");
            }
        }
        else
        {
            if (anim.GetBool(OpenHash))
            {
                anim.SetBool(OpenHash, false); // Закрываем крыло
               // Debug.Log("Closing wing.");
            }
        }
    }
}
