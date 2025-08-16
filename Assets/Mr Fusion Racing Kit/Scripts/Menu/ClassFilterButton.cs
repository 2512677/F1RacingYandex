using RGSK;
using UnityEngine;
using UnityEngine.UI;
using static CarClass;

public class ClassFilterButton : MonoBehaviour
{
    public VehicleClass classToShow = VehicleClass.None;   // выбираешь в инспекторе

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(
            () => MenuVehicleInstantiator.Instance.ApplyClassFilter(classToShow));
    }
}
