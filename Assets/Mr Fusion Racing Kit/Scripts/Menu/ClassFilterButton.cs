using RGSK;
using UnityEngine;
using UnityEngine.UI;
using static CarClass;

public class ClassFilterButton : MonoBehaviour
{
    public VehicleClass classToShow = VehicleClass.None;   // ��������� � ����������

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(
            () => MenuVehicleInstantiator.Instance.ApplyClassFilter(classToShow));
    }
}
