using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace RGSK
{
    public class MenuColorButton : MonoBehaviour
    {
        private MenuVehicleInstantiator menuVehicles;
       // private Image colorBox;
        public Color color;

        void Start()
        {
           // colorBox = GetComponent<Image>();
           // colorBox.color = color;

            menuVehicles = FindObjectOfType<MenuVehicleInstantiator>();
            if (menuVehicles != null)
            {
                GetComponent<Button>().onClick.AddListener(delegate { menuVehicles.SetSelectedVehicleColor(color); });
            }
        }
    }
}