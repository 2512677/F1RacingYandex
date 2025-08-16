using UnityEngine;

public class lights_manager : MonoBehaviour
{
    public enum LightType
    {
        headlights,
        taillights,
        brakelights,
        tailNbrake,
        reverselights,
        redIndicator,
    }

    private RCC_CarControllerV4 carController;

    public Renderer LightsRenderer;

    public int materialIndex;

    public LightType lighttype;

    [SerializeField]
    public float rpmThreshold = 6000f; // Порог для redIndicator

    public float GetRpmThreshold() => rpmThreshold; // Для доступа извне

    public void SetRpmThreshold(float value) => rpmThreshold = value; // Для изменения

    private void Start()
    {
        carController = GetComponentInParent<RCC_CarControllerV4>();

        if (carController == null)
        {
            Debug.LogError("RCC_CarControllerV4 не найден в родительском объекте!");
        }
    }

    private void Update()
    {
        switch (lighttype)
        {
            case LightType.headlights:
                HandleHeadlights();
                break;

            case LightType.taillights:
                HandleTaillights();
                break;

            case LightType.brakelights:
                HandleBrakelights();
                break;

            case LightType.tailNbrake:
                HandleTailNbrake();
                break;

            case LightType.reverselights:
                HandleReverselights();
                break;

            case LightType.redIndicator:
                HandleRedIndicator();
                break;
        }
    }

    private void HandleHeadlights()
    {
        if (carController.lowBeamHeadLightsOn && carController.engineRunning)
        {
            LightsRenderer.materials[materialIndex].SetColor("_EmissionColor", Color.white * 1f);
        }
        else
        {
            LightsRenderer.materials[materialIndex].SetColor("_EmissionColor", Color.white * 0f);
        }
    }

    private void HandleTaillights()
    {
        if (carController.lowBeamHeadLightsOn && !(carController.brakeInput >= 0.1f) && carController.engineRunning)
        {
            LightsRenderer.materials[materialIndex].SetColor("_EmissionColor", Color.white * 0.5f);
        }
        else
        {
            LightsRenderer.materials[materialIndex].SetColor("_EmissionColor", Color.white * 0f);
        }
    }

    private void HandleBrakelights()
    {
        if (carController.brakeInput >= 0.1f && carController.engineRunning)
        {
            LightsRenderer.materials[materialIndex].SetColor("_EmissionColor", Color.white * 1f);
        }
        else
        {
            LightsRenderer.materials[materialIndex].SetColor("_EmissionColor", Color.white * 0f);
        }
    }

    private void HandleTailNbrake()
    {
        if (carController.lowBeamHeadLightsOn && !(carController.brakeInput >= 0.1f) && carController.engineRunning)
        {
            LightsRenderer.materials[materialIndex].SetColor("_EmissionColor", Color.white * 0.5f);
        }
        else if (carController.brakeInput >= 0.1f && carController.engineRunning)
        {
            LightsRenderer.materials[materialIndex].SetColor("_EmissionColor", Color.white * 1f);
        }
        else
        {
            LightsRenderer.materials[materialIndex].SetColor("_EmissionColor", Color.white * 0f);
        }
    }

    private void HandleReverselights()
    {
        if (carController.direction == -1 && carController.engineRunning)
        {
            LightsRenderer.materials[materialIndex].SetColor("_EmissionColor", Color.white * 1f);
        }
        else
        {
            LightsRenderer.materials[materialIndex].SetColor("_EmissionColor", Color.white * 0f);
        }
    }

    private void HandleRedIndicator()
    {
        if (carController != null && carController.engineRPM >= rpmThreshold && carController.engineRunning)
        {
            LightsRenderer.materials[materialIndex].SetColor("_EmissionColor", Color.red * 5f);
        }
        else
        {
            LightsRenderer.materials[materialIndex].SetColor("_EmissionColor", Color.red * 0f);
        }
    }
}