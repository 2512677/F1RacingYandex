using UnityEngine;
using System.Collections;

public class RacingDisplayControl : MonoBehaviour
{

    // Ссылка на объект RCC_CarControllerV3
    public RCC_CarControllerV4 carController;

    // Тексты для отображения передачи, скорости, оборотов двигателя и топлива
    public TextMesh gearDisp;
    public TextMesh speedDisp;
    public TextMesh rpmDisp;
    public TextMesh fuelLevelDisp;

    // Материалы и индикаторы для RPM
    public Material rpmBarMat;
    public GameObject rpmBar;

    public Material ledred1;
    public Material ledred2;
    public Material ledyellow1;
    public Material ledyellow2;
    public Material ledgreen1;
    public Material ledgreen2;
    public Material ledgreen3;
    public Material ledgreen4;

    public Texture ledoff;
    public Texture ledon;

    // Вспомогательные переменные
    private float rpmOnValue = 0;

   void Start() {
    if (rpmBarMat == null) {
        Debug.LogError("RPM Bar Material is not assigned!");
    }

    if (ledred1 == null || ledred2 == null || ledyellow1 == null || ledyellow2 == null || ledgreen1 == null || ledgreen2 == null || ledgreen3 == null || ledgreen4 == null) {
        Debug.LogError("One or more LED materials are not assigned!");
    }

    ResetLEDColors();
}


    void Update()
    {
        if (carController == null)
        {
            Debug.LogWarning("RCC_CarControllerV3 не назначен!");
            return;
        }

        // Обновление дисплея передач
        if (carController.currentGear < 0)
        {
            gearDisp.text = "R";
        }
        else if (carController.currentGear == 0)
        {
            gearDisp.text = "N";
        }
        else
        {
            gearDisp.text = carController.currentGear.ToString();
        }

        // Обновление дисплея скорости
        speedDisp.text = carController.speed.ToString("000") + " km/h";

        // Обновление дисплея оборотов двигателя
        rpmDisp.text = carController.engineRPM.ToString("0000");

        // Обновление уровня топлива
        if (carController.useFuelConsumption)
        {
            fuelLevelDisp.text = carController.fuelTank.ToString("0") + " L";
        }
        else
        {
            fuelLevelDisp.text = "--";
        }

        // Управление индикаторами оборотов
        UpdateRPMIndicators();

        // Обновление материала RPM бара
        UpdateRPMBar();
    }

    private void ResetLEDColors()
    {
        ledgreen1.color = Color.white;
        ledgreen2.color = Color.white;
        ledgreen3.color = Color.white;
        ledgreen4.color = Color.white;
        ledyellow1.color = Color.white;
        ledyellow2.color = Color.white;
        ledred1.color = Color.white;
        ledred2.color = Color.white;
    }

    private void UpdateRPMIndicators()
    {
        if (carController.engineRPM > 1000)
        {
            ledgreen1.color = Color.green;
            ledgreen1.mainTexture = ledon;
        }
        else
        {
            ledgreen1.color = Color.white;
            ledgreen1.mainTexture = ledoff;
        }

        if (carController.engineRPM > 1500)
        {
            ledgreen2.color = Color.green;
            ledgreen2.mainTexture = ledon;
        }
        else
        {
            ledgreen2.color = Color.white;
            ledgreen2.mainTexture = ledoff;
        }

        if (carController.engineRPM > 2000)
        {
            ledgreen3.color = Color.green;
            ledgreen3.mainTexture = ledon;
        }
        else
        {
            ledgreen3.color = Color.white;
            ledgreen3.mainTexture = ledoff;
        }

        if (carController.engineRPM > 2500)
        {
            ledgreen4.color = Color.green;
            ledgreen4.mainTexture = ledon;
        }
        else
        {
            ledgreen4.color = Color.white;
            ledgreen4.mainTexture = ledoff;
        }

        if (carController.engineRPM > 3000)
        {
            ledyellow1.color = Color.yellow;
            ledyellow1.mainTexture = ledon;
        }
        else
        {
            ledyellow1.color = Color.white;
            ledyellow1.mainTexture = ledoff;
        }

        if (carController.engineRPM > 4000)
        {
            ledyellow2.color = Color.yellow;
            ledyellow2.mainTexture = ledon;
        }
        else
        {
            ledyellow2.color = Color.white;
            ledyellow2.mainTexture = ledoff;
        }

        if (carController.engineRPM > 5000)
        {
            ledred1.color = Color.red;
            ledred1.mainTexture = ledon;
        }
        else
        {
            ledred1.color = Color.white;
            ledred1.mainTexture = ledoff;
        }

        if (carController.engineRPM > 6000)
        {
            ledred2.color = Color.red;
            ledred2.mainTexture = ledon;
        }
        else
        {
            ledred2.color = Color.white;
            ledred2.mainTexture = ledoff;
        }
    }

    private void UpdateRPMBar()
    {
        rpmOnValue = Mathf.Clamp(carController.engineRPM / carController.maxEngineRPM, 0, 1);
        if (rpmBarMat != null)
        {
            rpmBarMat.SetFloat("_Progress", rpmOnValue);
        }
    }
}
