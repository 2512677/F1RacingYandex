using UnityEngine;

public class WheelBlur : MonoBehaviour
{
    public enum BlurType
    {
        Material = 0,
        Mesh = 1
    }

    public BlurType _BlurType;
    public int BlurRPM = 1000;
    public WheelCollider wheelCollider;
    public GameObject Wheel;
    public GameObject BlurWheel;
    public Material Normal;
    public Material Blur;

    private void Awake()
    {
        // Инициализация, если требуется
        if (wheelCollider == null)
        {
            Debug.LogError("WheelCollider is not assigned to the WheelBlur script.");
        }
    }

    private void Start()
    {
        // Если BlurWheel не активен по умолчанию, отключаем его
        if (BlurWheel != null)
        {
            BlurWheel.SetActive(false);
        }
    }

    private void Update()
    {
        float rpm = Mathf.Abs(wheelCollider.rpm);

        if (rpm >= BlurRPM)
        {
            switch (_BlurType)
            {
                case BlurType.Material:
                    BlurMat();
                    break;
                case BlurType.Mesh:
                    BlurMesh();
                    break;
            }
        }
        else
        {
            ResetBlur();
        }
    }

    private void BlurMesh()
    {
        if (Wheel != null && BlurWheel != null)
        {
            Wheel.SetActive(false);
            BlurWheel.SetActive(true);
        }
    }

    private void BlurMat()
    {
        if (Wheel != null)
        {
            Renderer wheelRenderer = Wheel.GetComponent<Renderer>();
            if (wheelRenderer != null && Blur != null)
            {
                wheelRenderer.material = Blur;
            }
        }
    }

    private void ResetBlur()
    {
        if (Wheel != null && BlurWheel != null)
        {
            Wheel.SetActive(true);
            BlurWheel.SetActive(false);
        }

        if (Wheel != null && Normal != null)
        {
            Renderer wheelRenderer = Wheel.GetComponent<Renderer>();
            if (wheelRenderer != null)
            {
                wheelRenderer.material = Normal;
            }
        }
    }
}
