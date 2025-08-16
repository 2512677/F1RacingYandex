using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using RGSK; // <-- ���� ���� PlayerData � MenuVehicleInstantiator � ���������� RGSK

public class TuningMenuUIController : MonoBehaviour
{
    // Enum ��������� (�����, ����� �������������� ������� ��� OnClick(int))
    public enum CurrentPart
    {
        Rims = 0,
        FBumper = 1,
        HeadLight = 2,
        Hood = 3,
        Fender = 4,
        Roof = 5,
        Mirror = 6,
        Door = 7,
        Skirt = 8,
        Trunk = 9,
        Spoiler = 10,
        RBumper = 11,
        TailLight = 12,
        Exhaust = 13
    }

    public CurrentPart currentPart = CurrentPart.Rims;

    // ������ (������/������) ��� ������ ��������� (������ �� ������������, ���� �� �����)
    public GameObject panelRims;
    public GameObject panelFBumpers;
    public GameObject panelHeadLights;
    public GameObject panelHoods;
    public GameObject panelFenders;
    public GameObject panelRoofs;
    public GameObject panelMirrors;
    public GameObject panelDoors;
    public GameObject panelSkirts;
    public GameObject panelTrunks;
    public GameObject panelSpoilers;
    public GameObject panelRBumpers;
    public GameObject panelTailLights;
    public GameObject panelExhausts;

    // ������ ������, ���� �� ������� ��������/���� ������
    public GameObject partButtonPrefab;
    // ���������, ���� ����� ����������� ������
    public Transform partsContainer;

    // UI ��� "�������" ���� (����� ������������ � ��� ��������� �������, �� ������� ��� � ��������)
    [Header("Wheel Purchase UI")]
    public Text partPriceText;
    public Button buyPartButton;
    public GameObject insufficientFundsPanel;

    // ���������� ����
    private CustomizeController currentCustomize;
    private int selectedWheelIndex = -1; // ��� ����
    private string selectedPartKey = ""; // ��� �������� ����� ������� ������ ��� �������

    // ���������� ��� ��������� ���� �������
    private void OnEnable()
    {
        currentCustomize = GetCurrentCustomize();
        RefreshTuningCategories();

        // ������ ������� ���� � ������ "������������ �������"
        if (buyPartButton) buyPartButton.gameObject.SetActive(false);
        if (partPriceText) partPriceText.text = "";
        if (insufficientFundsPanel) insufficientFundsPanel.SetActive(false);
    }

    /// <summary>
    /// ���������� ��� ����� �� ������ ��������� (OnClick -> SelectPartType(int)).
    /// �������� id � ������ �� enum CurrentPart.
    /// </summary>
    public void SelectPartType(int id)
    {
        currentPart = (CurrentPart)id;
        Debug.Log("������� ���������: " + currentPart);

        HideAllCategoryPanels();

        // ������� ���������, ����� ������� ������ ������
        if (partsContainer)
        {
            foreach (Transform child in partsContainer)
            {
                Destroy(child.gameObject);
            }
        }

        // � ����������� �� ��������� � ���������� ������ (���� ����) � ������ ������
        switch (currentPart)
        {
            case CurrentPart.Rims:
                if (panelRims) panelRims.SetActive(true);
                // ��� ���� ������� ��������� �����, ���� ������ ����������� �� ��������
                // �� � ������� ����� ���������� �����, ��. ������ OnSelectWheel() / OnBuyWheel().
                // ���� ������ ����������� � ����� ������� CreateRimsButtons();
                break;

            case CurrentPart.FBumper:
                if (panelFBumpers) panelFBumpers.SetActive(true);
                CreateFBumperButtons();
                break;

            case CurrentPart.HeadLight:
                if (panelHeadLights) panelHeadLights.SetActive(true);
                CreateHeadlightButtons();
                break;

            case CurrentPart.Hood:
                if (panelHoods) panelHoods.SetActive(true);
                CreateHoodButtons();
                break;

            case CurrentPart.Fender:
                if (panelFenders) panelFenders.SetActive(true);
                CreateFenderButtons();
                break;

            case CurrentPart.Roof:
                if (panelRoofs) panelRoofs.SetActive(true);
                CreateRoofButtons();
                break;

            case CurrentPart.Mirror:
                if (panelMirrors) panelMirrors.SetActive(true);
                CreateMirrorButtons();
                break;

            case CurrentPart.Door:
                if (panelDoors) panelDoors.SetActive(true);
                CreateDoorButtons();
                break;

            case CurrentPart.Skirt:
                if (panelSkirts) panelSkirts.SetActive(true);
                CreateSkirtButtons();
                break;

            case CurrentPart.Trunk:
                if (panelTrunks) panelTrunks.SetActive(true);
                CreateTrunkButtons();
                break;

            case CurrentPart.Spoiler:
                if (panelSpoilers) panelSpoilers.SetActive(true);
                CreateSpoilerButtons();
                break;

            case CurrentPart.RBumper:
                if (panelRBumpers) panelRBumpers.SetActive(true);
                CreateRBumperButtons();
                break;

            case CurrentPart.TailLight:
                if (panelTailLights) panelTailLights.SetActive(true);
                CreateTailLightButtons();
                break;

            case CurrentPart.Exhaust:
                if (panelExhausts) panelExhausts.SetActive(true);
                CreateExhaustButtons();
                break;

            default:
                Debug.LogWarning("��������� " + currentPart + " ��� �� �����������!");
                break;
        }
    }

    private void HideAllCategoryPanels()
    {
        if (panelRims) panelRims.SetActive(false);
        if (panelFBumpers) panelFBumpers.SetActive(false);
        if (panelHeadLights) panelHeadLights.SetActive(false);
        if (panelHoods) panelHoods.SetActive(false);
        if (panelFenders) panelFenders.SetActive(false);
        if (panelRoofs) panelRoofs.SetActive(false);
        if (panelMirrors) panelMirrors.SetActive(false);
        if (panelDoors) panelDoors.SetActive(false);
        if (panelSkirts) panelSkirts.SetActive(false);
        if (panelTrunks) panelTrunks.SetActive(false);
        if (panelSpoilers) panelSpoilers.SetActive(false);
        if (panelRBumpers) panelRBumpers.SetActive(false);
        if (panelTailLights) panelTailLights.SetActive(false);
        if (panelExhausts) panelExhausts.SetActive(false);
    }

    public void RefreshTuningCategories()
    {
        currentCustomize = GetCurrentCustomize();
        if (!currentCustomize)
        {
            Debug.LogWarning("��� CustomizeController �� ������� ������!");
            return;
        }

        // ���� ������, ������ ����� ��������/���������� ������ ���������,
        // ���� � ������ 0 ������� ��� ���� ���������
        // ��������:
        // if (panelHoods) panelHoods.SetActive(currentCustomize.Hoods != null && currentCustomize.Hoods.Length > 0);
        // ...
    }

    // ------------------ ������: ������ ������ ��� �������� �������� ------------------
    private void CreateFBumperButtons()
    {
        if (!partsContainer || !partButtonPrefab) return;
        if (!currentCustomize || currentCustomize.FrontBumpers == null) return;

        // ���� ������ ��������
        var array = currentCustomize.FrontBumpers;
        string carID = currentCustomize.carName; // ��� ������������ �����

        for (int i = 0; i < array.Length; i++)
        {
            var fb = array[i];
            // ���� ��� PlayerPrefs, ����� ������, ������ �� ���� ������
            // ��������: PartUnlocked_{carName}_FBumper_{i}
            string partKey = $"PartUnlocked_{carID}_FBumper_{i}";

            bool isUnlocked = PlayerPrefs.GetInt(partKey, 0) == 1;

            // ������ ������
            GameObject btn = Instantiate(partButtonPrefab, partsContainer);
            Text btnText = btn.GetComponentInChildren<Text>();

            if (isUnlocked)
            {
                // ���� ��� �������, ����� "Owned"
                btnText.text = $"FBumper {i} (Owned)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    // ��������� �����
                    currentCustomize.SelectFBumper(index);
                });
            }
            else
            {
                // �� ������� � ����� ����
                btnText.text = $"FBumper {i} ({fb.price} CR)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    // ������� �������
                    OnBuyPart(partKey, fb.price, () =>
                    {
                        // �������� �������: ���������
                        currentCustomize.SelectFBumper(index);
                    });
                });
            }
        }
    }

    // ------------------ ���������� ��� ��� ------------------
    private void CreateHeadlightButtons()
    {
        if (!partsContainer || !partButtonPrefab) return;
        if (!currentCustomize || currentCustomize.HeadLights == null) return;

        var array = currentCustomize.HeadLights;
        string carID = currentCustomize.carName;

        for (int i = 0; i < array.Length; i++)
        {
            var hl = array[i];
            string partKey = $"PartUnlocked_{carID}_HeadLight_{i}";
            bool isUnlocked = PlayerPrefs.GetInt(partKey, 0) == 1;

            GameObject btn = Instantiate(partButtonPrefab, partsContainer);
            Text btnText = btn.GetComponentInChildren<Text>();

            if (isUnlocked)
            {
                btnText.text = $"HeadLight {i} (Owned)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    currentCustomize.SelectHLight(index);
                });
            }
            else
            {
                btnText.text = $"HeadLight {i} ({hl.price} CR)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnBuyPart(partKey, hl.price, () =>
                    {
                        currentCustomize.SelectHLight(index);
                    });
                });
            }
        }
    }

    // ------------------ ���������� ��� ������� ------------------
    private void CreateHoodButtons()
    {
        if (!partsContainer || !partButtonPrefab) return;
        if (!currentCustomize || currentCustomize.Hoods == null) return;

        var array = currentCustomize.Hoods;
        string carID = currentCustomize.carName;

        for (int i = 0; i < array.Length; i++)
        {
            var hood = array[i];
            string partKey = $"PartUnlocked_{carID}_Hood_{i}";
            bool isUnlocked = PlayerPrefs.GetInt(partKey, 0) == 1;

            GameObject btn = Instantiate(partButtonPrefab, partsContainer);
            Text btnText = btn.GetComponentInChildren<Text>();

            if (isUnlocked)
            {
                btnText.text = $"Hood {i} (Owned)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    currentCustomize.SelectHood(index);
                });
            }
            else
            {
                btnText.text = $"Hood {i} ({hood.price} CR)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnBuyPart(partKey, hood.price, () =>
                    {
                        currentCustomize.SelectHood(index);
                    });
                });
            }
        }
    }

    // ------------------ ���������� ��� ������� (Fenders) ------------------
    private void CreateFenderButtons()
    {
        if (!partsContainer || !partButtonPrefab) return;
        if (!currentCustomize || currentCustomize.Fenders == null) return;

        var array = currentCustomize.Fenders;
        string carID = currentCustomize.carName;

        for (int i = 0; i < array.Length; i++)
        {
            var fender = array[i];
            string partKey = $"PartUnlocked_{carID}_Fender_{i}";
            bool isUnlocked = PlayerPrefs.GetInt(partKey, 0) == 1;

            GameObject btn = Instantiate(partButtonPrefab, partsContainer);
            Text btnText = btn.GetComponentInChildren<Text>();

            if (isUnlocked)
            {
                btnText.text = $"Fender {i} (Owned)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    currentCustomize.SelectFender(index);
                });
            }
            else
            {
                btnText.text = $"Fender {i} ({fender.price} CR)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnBuyPart(partKey, fender.price, () =>
                    {
                        currentCustomize.SelectFender(index);
                    });
                });
            }
        }
    }

    // ------------------ � ��� ����� ��� Roof, Mirrors, Doors, Skirts, Trunks, Spoilers, RBumpers, TailLights, Exhaust ------------------
    private void CreateRoofButtons()
    {
        if (!partsContainer || !partButtonPrefab) return;
        if (!currentCustomize || currentCustomize.Roofs == null) return;

        var array = currentCustomize.Roofs;
        string carID = currentCustomize.carName;

        for (int i = 0; i < array.Length; i++)
        {
            var roof = array[i];
            string partKey = $"PartUnlocked_{carID}_Roof_{i}";
            bool isUnlocked = PlayerPrefs.GetInt(partKey, 0) == 1;

            GameObject btn = Instantiate(partButtonPrefab, partsContainer);
            Text btnText = btn.GetComponentInChildren<Text>();

            if (isUnlocked)
            {
                btnText.text = $"Roof {i} (Owned)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    currentCustomize.SelectRoof(index);
                });
            }
            else
            {
                btnText.text = $"Roof {i} ({roof.price} CR)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnBuyPart(partKey, roof.price, () =>
                    {
                        currentCustomize.SelectRoof(index);
                    });
                });
            }
        }
    }

    private void CreateMirrorButtons()
    {
        if (!partsContainer || !partButtonPrefab) return;
        if (!currentCustomize || currentCustomize.Mirrors == null) return;

        var array = currentCustomize.Mirrors;
        string carID = currentCustomize.carName;

        for (int i = 0; i < array.Length; i++)
        {
            var mirror = array[i];
            string partKey = $"PartUnlocked_{carID}_Mirror_{i}";
            bool isUnlocked = PlayerPrefs.GetInt(partKey, 0) == 1;

            GameObject btn = Instantiate(partButtonPrefab, partsContainer);
            Text btnText = btn.GetComponentInChildren<Text>();

            if (isUnlocked)
            {
                btnText.text = $"Mirror {i} (Owned)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    currentCustomize.SelectMirrors(index);
                });
            }
            else
            {
                btnText.text = $"Mirror {i} ({mirror.price} CR)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnBuyPart(partKey, mirror.price, () =>
                    {
                        currentCustomize.SelectMirrors(index);
                    });
                });
            }
        }
    }

    private void CreateDoorButtons()
    {
        if (!partsContainer || !partButtonPrefab) return;
        if (!currentCustomize || currentCustomize.Doors == null) return;

        var array = currentCustomize.Doors;
        string carID = currentCustomize.carName;

        for (int i = 0; i < array.Length; i++)
        {
            var door = array[i];
            string partKey = $"PartUnlocked_{carID}_Door_{i}";
            bool isUnlocked = PlayerPrefs.GetInt(partKey, 0) == 1;

            GameObject btn = Instantiate(partButtonPrefab, partsContainer);
            Text btnText = btn.GetComponentInChildren<Text>();

            if (isUnlocked)
            {
                btnText.text = $"Door {i} (Owned)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    currentCustomize.SelectDoor(index);
                });
            }
            else
            {
                btnText.text = $"Door {i} ({door.price} CR)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnBuyPart(partKey, door.price, () =>
                    {
                        currentCustomize.SelectDoor(index);
                    });
                });
            }
        }
    }

    private void CreateSkirtButtons()
    {
        if (!partsContainer || !partButtonPrefab) return;
        if (!currentCustomize || currentCustomize.Skirts == null) return;

        var array = currentCustomize.Skirts;
        string carID = currentCustomize.carName;

        for (int i = 0; i < array.Length; i++)
        {
            var skirt = array[i];
            string partKey = $"PartUnlocked_{carID}_Skirt_{i}";
            bool isUnlocked = PlayerPrefs.GetInt(partKey, 0) == 1;

            GameObject btn = Instantiate(partButtonPrefab, partsContainer);
            Text btnText = btn.GetComponentInChildren<Text>();

            if (isUnlocked)
            {
                btnText.text = $"Skirt {i} (Owned)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    currentCustomize.SelectSkirt(index);
                });
            }
            else
            {
                btnText.text = $"Skirt {i} ({skirt.price} CR)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnBuyPart(partKey, skirt.price, () =>
                    {
                        currentCustomize.SelectSkirt(index);
                    });
                });
            }
        }
    }

    private void CreateTrunkButtons()
    {
        if (!partsContainer || !partButtonPrefab) return;
        if (!currentCustomize || currentCustomize.Trunks == null) return;

        var array = currentCustomize.Trunks;
        string carID = currentCustomize.carName;

        for (int i = 0; i < array.Length; i++)
        {
            var trunk = array[i];
            string partKey = $"PartUnlocked_{carID}_Trunk_{i}";
            bool isUnlocked = PlayerPrefs.GetInt(partKey, 0) == 1;

            GameObject btn = Instantiate(partButtonPrefab, partsContainer);
            Text btnText = btn.GetComponentInChildren<Text>();

            if (isUnlocked)
            {
                btnText.text = $"Trunk {i} (Owned)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    currentCustomize.SelectTrunk(index);
                });
            }
            else
            {
                btnText.text = $"Trunk {i} ({trunk.price} CR)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnBuyPart(partKey, trunk.price, () =>
                    {
                        currentCustomize.SelectTrunk(index);
                    });
                });
            }
        }
    }

    public void CreateSpoilerButtons()
    {
        if (!partsContainer || !partButtonPrefab) return;
        if (!currentCustomize || currentCustomize.Spoilers == null) return;

        var array = currentCustomize.Spoilers;
        string carID = currentCustomize.carName;

        for (int i = 0; i < array.Length; i++)
        {
            var sp = array[i];
            string partKey = $"PartUnlocked_{carID}_Spoiler_{i}";
            bool isUnlocked = PlayerPrefs.GetInt(partKey, 0) == 1;

            GameObject btn = Instantiate(partButtonPrefab, partsContainer);
            Text btnText = btn.GetComponentInChildren<Text>();

            if (isUnlocked)
            {
                btnText.text = $"Spoiler {i} (Owned)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    currentCustomize.SelectSpoiler(index);
                });
            }
            else
            {
                btnText.text = $"Spoiler {i} ({sp.price} CR)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnBuyPart(partKey, sp.price, () =>
                    {
                        currentCustomize.SelectSpoiler(index);
                    });
                });
            }
        }
    }

    private void CreateRBumperButtons()
    {
        if (!partsContainer || !partButtonPrefab) return;
        if (!currentCustomize || currentCustomize.RearBumpers == null) return;

        var array = currentCustomize.RearBumpers;
        string carID = currentCustomize.carName;

        for (int i = 0; i < array.Length; i++)
        {
            var rb = array[i];
            string partKey = $"PartUnlocked_{carID}_RBumper_{i}";
            bool isUnlocked = PlayerPrefs.GetInt(partKey, 0) == 1;

            GameObject btn = Instantiate(partButtonPrefab, partsContainer);
            Text btnText = btn.GetComponentInChildren<Text>();

            if (isUnlocked)
            {
                btnText.text = $"RearBumper {i} (Owned)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    currentCustomize.SelectRBumper(index);
                });
            }
            else
            {
                btnText.text = $"RearBumper {i} ({rb.price} CR)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnBuyPart(partKey, rb.price, () =>
                    {
                        currentCustomize.SelectRBumper(index);
                    });
                });
            }
        }
    }

    private void CreateTailLightButtons()
    {
        if (!partsContainer || !partButtonPrefab) return;
        if (!currentCustomize || currentCustomize.TailLights == null) return;

        var array = currentCustomize.TailLights;
        string carID = currentCustomize.carName;

        for (int i = 0; i < array.Length; i++)
        {
            var tl = array[i];
            string partKey = $"PartUnlocked_{carID}_TailLight_{i}";
            bool isUnlocked = PlayerPrefs.GetInt(partKey, 0) == 1;

            GameObject btn = Instantiate(partButtonPrefab, partsContainer);
            Text btnText = btn.GetComponentInChildren<Text>();

            if (isUnlocked)
            {
                btnText.text = $"TailLight {i} (Owned)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    currentCustomize.SelectTLight(index);
                });
            }
            else
            {
                btnText.text = $"TailLight {i} ({tl.price} CR)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnBuyPart(partKey, tl.price, () =>
                    {
                        currentCustomize.SelectTLight(index);
                    });
                });
            }
        }
    }

    private void CreateExhaustButtons()
    {
        if (!partsContainer || !partButtonPrefab) return;
        if (!currentCustomize || currentCustomize.Exhausts == null) return;

        var array = currentCustomize.Exhausts;
        string carID = currentCustomize.carName;

        for (int i = 0; i < array.Length; i++)
        {
            var ex = array[i];
            string partKey = $"PartUnlocked_{carID}_Exhaust_{i}";
            bool isUnlocked = PlayerPrefs.GetInt(partKey, 0) == 1;

            GameObject btn = Instantiate(partButtonPrefab, partsContainer);
            Text btnText = btn.GetComponentInChildren<Text>();

            if (isUnlocked)
            {
                btnText.text = $"Exhaust {i} (Owned)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    currentCustomize.SelectExhaust(index);
                });
            }
            else
            {
                btnText.text = $"Exhaust {i} ({ex.price} CR)";
                int index = i;
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnBuyPart(partKey, ex.price, () =>
                    {
                        currentCustomize.SelectExhaust(index);
                    });
                });
            }
        }
    }

    // ------------------ ������ ��� ������ (Rims) ------------------
    // ����� �������� ��� � ���������� �������:
    public void OnSelectWheel(int index)
    {
        if (!currentCustomize || currentCustomize.Wheel_list == null || currentCustomize.Wheel_list.wheels == null)
            return;

        selectedWheelIndex = index;

        var wheelData = currentCustomize.Wheel_list.wheels[index];
        int price = wheelData.price;
        string rimName = wheelData.wheelName;

        bool isUnlocked = PlayerPrefs.GetInt("WheelUnlocked_" + rimName, 0) == 1;
        if (isUnlocked)
        {
            if (buyPartButton) buyPartButton.gameObject.SetActive(false);
            if (partPriceText) partPriceText.text = "";
            currentCustomize.ChangeAllWheels(index);
        }
        else
        {
            if (buyPartButton) buyPartButton.gameObject.SetActive(true);
            if (partPriceText) partPriceText.text = price + " CR";
        }
    }

    public void OnBuyWheel()
    {
        if (selectedWheelIndex < 0 || !currentCustomize || currentCustomize.Wheel_list == null)
            return;

        var wheelData = currentCustomize.Wheel_list.wheels[selectedWheelIndex];
        int price = wheelData.price;
        string rimName = wheelData.wheelName;

        float playerMoney = PlayerData.instance.playerData.playerCurrency;
        if (playerMoney >= price)
        {
            PlayerData.instance.AddPlayerCurrecny(-price);
            PlayerPrefs.SetInt("WheelUnlocked_" + rimName, 1);
            PlayerPrefs.Save();

            currentCustomize.ChangeAllWheels(selectedWheelIndex);

            if (buyPartButton) buyPartButton.gameObject.SetActive(false);
            if (partPriceText) partPriceText.text = "";
        }
        else
        {
            Debug.Log("������������ ������� ��� ������� �����: " + rimName);
            if (insufficientFundsPanel) insufficientFundsPanel.SetActive(true);
        }
    }

    // ------------------ ������������� ����� ������� ������ ------------------
    // partKey - ���� ��� PlayerPrefs, price - ����, onSuccess - ������� ��� �������� �������
    private void OnBuyPart(string partKey, int price, System.Action onSuccess)
    {
        float playerMoney = PlayerData.instance.playerData.playerCurrency;
        if (playerMoney >= price)
        {
            // ��������� ������
            PlayerData.instance.AddPlayerCurrecny(-price);

            // ���������, ��� ������ �������
            PlayerPrefs.SetInt(partKey, 1);
            PlayerPrefs.Save();

            // �������� ������� (��������� ������)
            onSuccess?.Invoke();
        }
        else
        {
            // ������������ �������
            Debug.Log("������������ ������� ��� ������� ������: " + partKey);
            if (insufficientFundsPanel) insufficientFundsPanel.SetActive(true);
        }
    }

    private CustomizeController GetCurrentCustomize()
    {
        var mvi = MenuVehicleInstantiator.Instance;
        if (!mvi) return null;

        GameObject currentCar = mvi.GetCurrentVehicle();
        if (!currentCar) return null;

        return currentCar.GetComponent<CustomizeController>();
    }
}
