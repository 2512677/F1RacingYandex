using System;
using UnityEngine;

public class CustomizeController : MonoBehaviour
{
    [Serializable]
    public class headLightParts
    {
        public GameObject headLights;
        public int price;
    }

    [Serializable]
    public class frontBumperParts
    {
        public GameObject frontBumper;
        public int price;
    }

    [Serializable]
    public class hoodParts
    {
        public GameObject hood;
        public int price;
    }

    [Serializable]
    public class fendersParts
    {
        public GameObject fenders;
        public int price;
    }

    [Serializable]
    public class roofParts
    {
        public GameObject roof;
        public int price;
    }

    [Serializable]
    public class mirrorsParts
    {
        public GameObject mirrors;
        public int price;
    }

    [Serializable]
    public class doorsParts
    {
        public GameObject doors;
        public int price;
    }

    [Serializable]
    public class skirtsParts
    {
        public GameObject skirts;
        public int price;
    }

    [Serializable]
    public class trunkParts
    {
        public GameObject trunk;
        public int price;
    }

    [Serializable]
    public class rearBumperParts
    {
        public GameObject rearBumper;
        public int price;
    }

    [Serializable]
    public class spoilerParts
    {
        public GameObject spoiler;
        public int price;
    }

    [Serializable]
    public class tailLightParts
    {
        public GameObject tailLights;
        public int price;
    }

    [Serializable]
    public class exhaustParts
    {
        public GameObject exhaust;
        public int price;
    }

    // Добавляем классы wheel_list и WheelData
    [Serializable]
    public class WheelData
    {
        public string wheelName;
        public GameObject Wheel_Prefab;
        public int price;
    }

    [Serializable]
    public class wheel_list
    {
        public WheelData[] wheels;
    }

    public string carName;

    public headLightParts[] HeadLights;
    public frontBumperParts[] FrontBumpers;
    public hoodParts[] Hoods;
    public fendersParts[] Fenders;
    public roofParts[] Roofs;
    public mirrorsParts[] Mirrors;
    public doorsParts[] Doors;
    public skirtsParts[] Skirts;
    public trunkParts[] Trunks;
    public rearBumperParts[] RearBumpers;
    public spoilerParts[] Spoilers;
    public tailLightParts[] TailLights;
    public exhaustParts[] Exhausts;

    public int HLightSelected;
    public int FBumperSelected;
    public int HoodSelected;
    public int FenderSelected;
    public int RoofSelected;
    public int MirrorSelected;
    public int DoorSelected;
    public int SkirtSelected;
    public int TrunkSelected;
    public int RBumperSelected;
    public int SpoilerSelected;
    public int TLightSelected;
    public int ExhaustSelected;
    public int WhlSelected;

    public wheel_list Wheel_list; // Список колес
    public GameObject FrontWheel;
    public GameObject RearWheel;
    public GameObject ExtraWheel;
    public Transform FrontLeftWheelTransform;
    public Transform FrontRightWheelTransform;
    public Transform RearLeftWheelTransform;
    public Transform RearRightWheelTransform;
    public Transform[] ExtraWheelsTransform;
    public GameObject FLWheelObject;
    public GameObject FRWheelObject;
    public GameObject RLWheelObject;
    public GameObject RRWheelObject;
    public GameObject[] ExtraWheelsObject;

    private void Awake()
    {
        // Инициализация при запуске (если необходимо)
    }

    private void Start()
    {
        // Загрузка сохраненных деталей при старте
        LoadParts();
    }

    private void Update()
    {
        // Обновление каждый кадр (если необходимо)
    }

    public void LoadParts()
    {
        // Загрузка выбранных деталей из PlayerPrefs
        HLightSelected = PlayerPrefs.GetInt(carName + "_HLightSelected", 0);
        FBumperSelected = PlayerPrefs.GetInt(carName + "_FBumperSelected", 0);
        HoodSelected = PlayerPrefs.GetInt(carName + "_HoodSelected", 0);
        FenderSelected = PlayerPrefs.GetInt(carName + "_FenderSelected", 0);
        RoofSelected = PlayerPrefs.GetInt(carName + "_RoofSelected", 0);
        MirrorSelected = PlayerPrefs.GetInt(carName + "_MirrorSelected", 0);
        DoorSelected = PlayerPrefs.GetInt(carName + "_DoorSelected", 0);
        SkirtSelected = PlayerPrefs.GetInt(carName + "_SkirtSelected", 0);
        TrunkSelected = PlayerPrefs.GetInt(carName + "_TrunkSelected", 0);
        RBumperSelected = PlayerPrefs.GetInt(carName + "_RBumperSelected", 0);
        SpoilerSelected = PlayerPrefs.GetInt(carName + "_SpoilerSelected", 0);
        TLightSelected = PlayerPrefs.GetInt(carName + "_TLightSelected", 0);
        ExhaustSelected = PlayerPrefs.GetInt(carName + "_ExhaustSelected", 0);
        WhlSelected = PlayerPrefs.GetInt(carName + "_WhlSelected", 0);

        // Применение выбранных деталей
        SelectHLight(HLightSelected);
        SelectFBumper(FBumperSelected);
        SelectHood(HoodSelected);
        SelectFender(FenderSelected);
        SelectRoof(RoofSelected);
        SelectMirrors(MirrorSelected);
        SelectDoor(DoorSelected);
        SelectSkirt(SkirtSelected);
        SelectTrunk(TrunkSelected);
        SelectRBumper(RBumperSelected);
        SelectSpoiler(SpoilerSelected);
        SelectTLight(TLightSelected);
        SelectExhaust(ExhaustSelected);
        ChangeAllWheels(WhlSelected);
    }

    public void SelectHLight(int ID)
    {
        // Отключаем все фары
        foreach (var hl in HeadLights)
        {
            if (hl.headLights != null)
                hl.headLights.SetActive(false);
        }

        // Включаем выбранные фары
        if (ID >= 0 && ID < HeadLights.Length && HeadLights[ID].headLights != null)
        {
            HeadLights[ID].headLights.SetActive(true);
            HLightSelected = ID;
            PlayerPrefs.SetInt(carName + "_HLightSelected", HLightSelected);
        }
        else
        {
            Debug.LogWarning("Неверный ID фар");
        }
    }

    public void SelectFBumper(int ID)
    {
        // Отключаем все передние бамперы
        foreach (var fb in FrontBumpers)
        {
            if (fb.frontBumper != null)
                fb.frontBumper.SetActive(false);
        }

        // Включаем выбранный бампер
        if (ID >= 0 && ID < FrontBumpers.Length && FrontBumpers[ID].frontBumper != null)
        {
            FrontBumpers[ID].frontBumper.SetActive(true);
            FBumperSelected = ID;
            PlayerPrefs.SetInt(carName + "_FBumperSelected", FBumperSelected);
        }
        else
        {
            Debug.LogWarning("Неверный ID переднего бампера");
        }
    }

    public void SelectHood(int ID)
    {
        // Отключаем все капоты
        foreach (var h in Hoods)
        {
            if (h.hood != null)
                h.hood.SetActive(false);
        }

        // Включаем выбранный капот
        if (ID >= 0 && ID < Hoods.Length && Hoods[ID].hood != null)
        {
            Hoods[ID].hood.SetActive(true);
            HoodSelected = ID;
            PlayerPrefs.SetInt(carName + "_HoodSelected", HoodSelected);
        }
        else
        {
            Debug.LogWarning("Неверный ID капота");
        }
    }

    public void SelectFender(int ID)
    {
        // Отключаем все крылья
        foreach (var f in Fenders)
        {
            if (f.fenders != null)
                f.fenders.SetActive(false);
        }

        // Включаем выбранные крылья
        if (ID >= 0 && ID < Fenders.Length && Fenders[ID].fenders != null)
        {
            Fenders[ID].fenders.SetActive(true);
            FenderSelected = ID;
            PlayerPrefs.SetInt(carName + "_FenderSelected", FenderSelected);
        }
        else
        {
            Debug.LogWarning("Неверный ID крыльев");
        }
    }

    public void SelectRoof(int ID)
    {
        // Отключаем все крыши
        foreach (var r in Roofs)
        {
            if (r.roof != null)
                r.roof.SetActive(false);
        }

        // Включаем выбранную крышу
        if (ID >= 0 && ID < Roofs.Length && Roofs[ID].roof != null)
        {
            Roofs[ID].roof.SetActive(true);
            RoofSelected = ID;
            PlayerPrefs.SetInt(carName + "_RoofSelected", RoofSelected);
        }
        else
        {
            Debug.LogWarning("Неверный ID крыши");
        }
    }

    public void SelectMirrors(int ID)
    {
        // Отключаем все зеркала
        foreach (var m in Mirrors)
        {
            if (m.mirrors != null)
                m.mirrors.SetActive(false);
        }

        // Включаем выбранные зеркала
        if (ID >= 0 && ID < Mirrors.Length && Mirrors[ID].mirrors != null)
        {
            Mirrors[ID].mirrors.SetActive(true);
            MirrorSelected = ID;
            PlayerPrefs.SetInt(carName + "_MirrorSelected", MirrorSelected);
        }
        else
        {
            Debug.LogWarning("Неверный ID зеркал");
        }
    }

    public void SelectDoor(int ID)
    {
        // Отключаем все двери
        foreach (var d in Doors)
        {
            if (d.doors != null)
                d.doors.SetActive(false);
        }

        // Включаем выбранные двери
        if (ID >= 0 && ID < Doors.Length && Doors[ID].doors != null)
        {
            Doors[ID].doors.SetActive(true);
            DoorSelected = ID;
            PlayerPrefs.SetInt(carName + "_DoorSelected", DoorSelected);
        }
        else
        {
            Debug.LogWarning("Неверный ID дверей");
        }
    }

    public void SelectSkirt(int ID)
    {
        // Отключаем все пороги
        foreach (var s in Skirts)
        {
            if (s.skirts != null)
                s.skirts.SetActive(false);
        }

        // Включаем выбранные пороги
        if (ID >= 0 && ID < Skirts.Length && Skirts[ID].skirts != null)
        {
            Skirts[ID].skirts.SetActive(true);
            SkirtSelected = ID;
            PlayerPrefs.SetInt(carName + "_SkirtSelected", SkirtSelected);
        }
        else
        {
            Debug.LogWarning("Неверный ID порогов");
        }
    }

    public void SelectTrunk(int ID)
    {
        // Отключаем все багажники
        foreach (var t in Trunks)
        {
            if (t.trunk != null)
                t.trunk.SetActive(false);
        }

        // Включаем выбранный багажник
        if (ID >= 0 && ID < Trunks.Length && Trunks[ID].trunk != null)
        {
            Trunks[ID].trunk.SetActive(true);
            TrunkSelected = ID;
            PlayerPrefs.SetInt(carName + "_TrunkSelected", TrunkSelected);
        }
        else
        {
            Debug.LogWarning("Неверный ID багажника");
        }
    }

    public void SelectRBumper(int ID)
    {
        // Отключаем все задние бамперы
        foreach (var rb in RearBumpers)
        {
            if (rb.rearBumper != null)
                rb.rearBumper.SetActive(false);
        }

        // Включаем выбранный задний бампер
        if (ID >= 0 && ID < RearBumpers.Length && RearBumpers[ID].rearBumper != null)
        {
            RearBumpers[ID].rearBumper.SetActive(true);
            RBumperSelected = ID;
            PlayerPrefs.SetInt(carName + "_RBumperSelected", RBumperSelected);
        }
        else
        {
            Debug.LogWarning("Неверный ID заднего бампера");
        }
    }

    public void SelectSpoiler(int ID)
    {
        // Отключаем все спойлеры
        foreach (var sp in Spoilers)
        {
            if (sp.spoiler != null)
                sp.spoiler.SetActive(false);
        }

        // Включаем выбранный спойлер
        if (ID >= 0 && ID < Spoilers.Length && Spoilers[ID].spoiler != null)
        {
            Spoilers[ID].spoiler.SetActive(true);
            SpoilerSelected = ID;
            PlayerPrefs.SetInt(carName + "_SpoilerSelected", SpoilerSelected);
        }
        else
        {
            Debug.LogWarning("Неверный ID спойлера");
        }
    }

    public void SelectTLight(int ID)
    {
        // Отключаем все задние фары
        foreach (var tl in TailLights)
        {
            if (tl.tailLights != null)
                tl.tailLights.SetActive(false);
        }

        // Включаем выбранные задние фары
        if (ID >= 0 && ID < TailLights.Length && TailLights[ID].tailLights != null)
        {
            TailLights[ID].tailLights.SetActive(true);
            TLightSelected = ID;
            PlayerPrefs.SetInt(carName + "_TLightSelected", TLightSelected);
        }
        else
        {
            Debug.LogWarning("Неверный ID задних фар");
        }
    }

    public void SelectExhaust(int ID)
    {
        // Отключаем все выхлопные трубы
        foreach (var ex in Exhausts)
        {
            if (ex.exhaust != null)
                ex.exhaust.SetActive(false);
        }

        // Включаем выбранную выхлопную трубу
        if (ID >= 0 && ID < Exhausts.Length && Exhausts[ID].exhaust != null)
        {
            Exhausts[ID].exhaust.SetActive(true);
            ExhaustSelected = ID;
            PlayerPrefs.SetInt(carName + "_ExhaustSelected", ExhaustSelected);
        }
        else
        {
            Debug.LogWarning("Неверный ID выхлопной трубы");
        }
    }

    public void ChangeAllWheels(int ID)
    {
        // Удаляем существующие колеса
        if (FLWheelObject != null) Destroy(FLWheelObject);
        if (FRWheelObject != null) Destroy(FRWheelObject);
        if (RLWheelObject != null) Destroy(RLWheelObject);
        if (RRWheelObject != null) Destroy(RRWheelObject);

        if (ExtraWheelsObject != null)
        {
            foreach (var ew in ExtraWheelsObject)
            {
                if (ew != null)
                    Destroy(ew);
            }
        }

        // Проверяем наличие Wheel_list и колес в нем
        if (Wheel_list == null || Wheel_list.wheels == null || Wheel_list.wheels.Length == 0)
        {
            Debug.LogWarning("Wheel_list не задан или пустой!");
            return;
        }

        // Проверяем корректность ID колеса
        if (ID >= 0 && ID < Wheel_list.wheels.Length)
        {
            GameObject wheelPrefab = Wheel_list.wheels[ID].Wheel_Prefab;

            if (wheelPrefab != null)
            {
                // Переднее левое колесо
                FLWheelObject = Instantiate(wheelPrefab, FrontLeftWheelTransform);
                FLWheelObject.transform.localPosition = Vector3.zero;
                FLWheelObject.transform.localRotation = Quaternion.identity;

                // Переднее правое колесо
                FRWheelObject = Instantiate(wheelPrefab, FrontRightWheelTransform);
                FRWheelObject.transform.localPosition = Vector3.zero;
                FRWheelObject.transform.localRotation = Quaternion.identity;

                // Заднее левое колесо
                RLWheelObject = Instantiate(wheelPrefab, RearLeftWheelTransform);
                RLWheelObject.transform.localPosition = Vector3.zero;
                RLWheelObject.transform.localRotation = Quaternion.identity;

                // Заднее правое колесо
                RRWheelObject = Instantiate(wheelPrefab, RearRightWheelTransform);
                RRWheelObject.transform.localPosition = Vector3.zero;
                RRWheelObject.transform.localRotation = Quaternion.identity;

                // Дополнительные колеса (если есть)
                if (ExtraWheelsTransform != null && ExtraWheelsTransform.Length > 0)
                {
                    ExtraWheelsObject = new GameObject[ExtraWheelsTransform.Length];
                    for (int i = 0; i < ExtraWheelsTransform.Length; i++)
                    {
                        ExtraWheelsObject[i] = Instantiate(wheelPrefab, ExtraWheelsTransform[i]);
                        ExtraWheelsObject[i].transform.localPosition = Vector3.zero;
                        ExtraWheelsObject[i].transform.localRotation = Quaternion.identity;
                    }
                }

                WhlSelected = ID;
                PlayerPrefs.SetInt(carName + "_WhlSelected", WhlSelected);
            }
            else
            {
                Debug.LogWarning("Префаб колеса не задан для ID: " + ID);
            }
        }
        else
        {
            Debug.LogWarning("Неверный ID колеса");
        }
    }
}
