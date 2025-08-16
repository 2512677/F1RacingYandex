using RGSK;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CarTuningPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public CarBodyKittPanel carBodyKittPanel;
    public CarPaintPanel carPaintPanle;
    public CarMufflerPanel carMufflerPanel;
    public CarWheelPanel carWheelPanel;
    public CarSpoilerPanel carSpoilerPanel;

    public Button carBodyKittButton;
    public Button carPaintButton;
    public Button carMufflerButton;
    public Button carWheelButton;
    public Button carSpoilerButton;

    // Update is called once per frame
    void Start()
    {
        // Add button listeners
        if (carPaintButton != null)
            carPaintButton.onClick.AddListener(delegate { ShowPanel("Car Paint"); });

        if (carBodyKittButton != null)
            carBodyKittButton.onClick.AddListener(delegate { ShowPanel("Car Body Kitt"); });

        if (carMufflerButton != null)
            carMufflerButton.onClick.AddListener(delegate { ShowPanel("Car Muffler"); });

        if (carWheelButton != null)
            carWheelButton.onClick.AddListener(delegate { ShowPanel("Car Wheel"); });

        if (carSpoilerButton != null)
            carSpoilerButton.onClick.AddListener(delegate { ShowPanel("Car Spoiler"); });

    }

    void ShowPanel(string panel)
    {
        gameObject.SetActive(false);

        switch (panel)
        {
            case "Car Body Kitt":
                carBodyKittPanel?.gameObject.SetActive(true);
                break;
            case "Car Paint":
                carPaintPanle?.gameObject.SetActive(true);
                break;
            case "Car Muffler":
                carMufflerPanel?.gameObject.SetActive(true);
                break;
            case "Car Wheel":
                carWheelPanel?.gameObject.SetActive(true);
                break;
            case "Car Spoiler":
                carSpoilerPanel?.gameObject.SetActive(true);
                break;

        }
    }
}