using UnityEngine;
using UnityEditor;
using RGSK;
using System.Collections.Generic;

public class Window_VehicleSetup : EditorWindow
{
    public enum VehicleType
    {
        Car,
        Motorbike
    }

    public enum VehiclePresets
    {
        SmallSportsCar,
        SportsCar,
        SuperCar,
        F1,
        GoKart,
        SportBike,
        DirtBike
    }

    public enum WheelDrive
    {
        RWD,
        FWD,
        AWD
    }

    GUIStyle centerLabelStyle;

    public Transform vehicle;
    public string vehicleName;
    public VehicleType vehicleType;
    public VehiclePresets vehiclePreset = VehiclePresets.SportsCar;
    public WheelDrive wheelDrive;

    public bool createCollider = true;
    public bool addNitro = false;
    public bool addSlipstream = false;

    //Wheels
    public Transform WheelFL;
    public Transform WheelFR;
    public Transform WheelRL;
    public Transform WheelRR;

    //Motorbike Weels
    public Transform frontMotorbikeWheel;
    public Transform rearMotorbikeWheel;

    //Specs
    public float vehicleMass = 1000;
    public float MaxEngineTorque = 200;
    public float MaxEngineTorqueRPM = 6000;
    public float MinRPM = 1000;
    public float MaxRPM = 8000;
    public float[] gearRatios;
    public float finalDrive;

    private string debugString = "";
    private VehicleType lastVehicleType;
    private VehiclePresets lastPreset;

    void OnEnable()
    {
        lastVehicleType = vehicleType;
        lastPreset = vehiclePreset;
        SetVehiclePropertiesFromPreset(vehiclePreset);

        centerLabelStyle = new GUIStyle();
        centerLabelStyle.fontStyle = FontStyle.Normal;
        centerLabelStyle.normal.textColor = Color.white;
        centerLabelStyle.alignment = TextAnchor.MiddleCenter;
    }


    void OnGUI()
    {
        //--Asset Logo--
        //if (Editor_Helper.Logo())
        //{
            //GUILayout.Label(Editor_Helper.Logo(), GUILayout.Height(40));
        //}

        //---Main Setup---
        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("General Setup", centerLabelStyle);
        EditorGUILayout.Space();

        vehicle = EditorGUILayout.ObjectField("Vehicle", vehicle, typeof(Transform), true) as Transform;
        vehicleName = EditorGUILayout.TextField("Vehicle Name", vehicleName);
        vehicleType = (VehicleType)EditorGUILayout.EnumPopup("Vehicle Type", vehicleType);
        if(vehicleType == VehicleType.Car)wheelDrive = (WheelDrive)EditorGUILayout.EnumPopup("Drivetrain", wheelDrive);
        vehiclePreset = (VehiclePresets)EditorGUILayout.EnumPopup("Vehicle Preset", vehiclePreset);
        createCollider = EditorGUILayout.Toggle("Create Collider", createCollider);
        addNitro = EditorGUILayout.Toggle("Nitro Component", addNitro);
        addSlipstream = EditorGUILayout.Toggle("Slipstream Component", addSlipstream);


        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        //---Wheels---
        EditorGUILayout.LabelField("Wheel Setup", centerLabelStyle);
        EditorGUILayout.Space();

        if (vehicleType == VehicleType.Car)
        {
            WheelFL = EditorGUILayout.ObjectField("Front Left Wheel", WheelFL, typeof(Transform), true) as Transform;
            WheelFR = EditorGUILayout.ObjectField("Front Right Wheel", WheelFR, typeof(Transform), true) as Transform;
            WheelRL = EditorGUILayout.ObjectField("Rear Left Wheel", WheelRL, typeof(Transform), true) as Transform;
            WheelRR = EditorGUILayout.ObjectField("Rear Right Wheel", WheelRR, typeof(Transform), true) as Transform;
        }

        if (vehicleType == VehicleType.Motorbike)
        {
            frontMotorbikeWheel = EditorGUILayout.ObjectField("Front Wheel", frontMotorbikeWheel, typeof(Transform), true) as Transform;
            rearMotorbikeWheel = EditorGUILayout.ObjectField("Rear Wheel", rearMotorbikeWheel, typeof(Transform), true) as Transform;
        }

        GUILayout.EndVertical();

        GUILayout.Space(20);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        //Configure
        if (GUILayout.Button("Configure Vehicle"))
        {
            ConfigureVehicle();
        }

        EditorGUILayout.LabelField(debugString);


        if (GUI.changed)
        {
            UpdateGUIChanged();
        }
    }


    void ConfigureVehicle()
    {
        if (!vehicle)
        {
            debugString = "Assign a vehicle!";
            return;
        }

        if (vehicleType == VehicleType.Car)
        {
            if (!WheelFL || !WheelFR || !WheelRL || !WheelRR)
            {
                debugString = "Assign all wheels!";
                return;
            }
        }

        if (vehicleType == VehicleType.Motorbike)
        {
            if (!frontMotorbikeWheel || !rearMotorbikeWheel)
            {
                debugString = "Assign all wheels!";
                return;
            }
        }

        //Add components to the vehicle
        Rigidbody rigid = !vehicle.gameObject.GetComponent<Rigidbody>() ? vehicle.gameObject.AddComponent<Rigidbody>()
                      : vehicle.gameObject.GetComponent<Rigidbody>();

        rigid.mass = vehicleMass;
        rigid.interpolation = RigidbodyInterpolation.Interpolate;

        //Create a collider for the vehicle
        if (createCollider)
        {
            CreateVehicleCollider();
        }

        //Add a CameraMount gameObject where all the camera mounts will go
        GameObject camMounts = new GameObject("CameraMounts");
        camMounts.transform.SetParent(vehicle, false);

        //Add a COM gameObject
        GameObject com = new GameObject("CenterOfMass");
        com.transform.SetParent(vehicle, false);

        //Configure Car
        if (vehicleType == VehicleType.Car)
        {
            GameObject wheelColliderParent = new GameObject("Wheel Colliders");
            wheelColliderParent.transform.SetParent(vehicle, false);

            WheelCollider wheelColliderFL = new GameObject("Wheel Collider FL").AddComponent<WheelCollider>();
            WheelCollider wheelColliderFR = new GameObject("Wheel Collider FR").AddComponent<WheelCollider>();
            WheelCollider wheelColliderRL = new GameObject("Wheel Collider RL").AddComponent<WheelCollider>();
            WheelCollider wheelColliderRR = new GameObject("Wheel Collider RR").AddComponent<WheelCollider>();

            wheelColliderFL.transform.SetParent(wheelColliderParent.transform);
            wheelColliderFL.transform.position = WheelFL.position;
            wheelColliderFL.center = new Vector3(0, 0.15f, 0);
            wheelColliderFL.radius = WheelColliderRaduis(WheelFL);
            wheelColliderFL.mass = 60;
            wheelColliderFL.gameObject.layer = LayerMask.NameToLayer("Vehicle");
            wheelColliderFL.gameObject.AddComponent<VehicleWheel>();

            wheelColliderFR.transform.SetParent(wheelColliderParent.transform);
            wheelColliderFR.transform.position = WheelFR.position;
            wheelColliderFR.center = new Vector3(0, 0.15f, 0);
            wheelColliderFR.radius = WheelColliderRaduis(WheelFR);
            wheelColliderFR.mass = 60;
            wheelColliderFR.gameObject.layer = LayerMask.NameToLayer("Vehicle");
            wheelColliderFR.gameObject.AddComponent<VehicleWheel>();

            wheelColliderRL.transform.SetParent(wheelColliderParent.transform);
            wheelColliderRL.transform.position = WheelRL.position;
            wheelColliderRL.center = new Vector3(0, 0.15f, 0);
            wheelColliderRL.radius = WheelColliderRaduis(WheelRL);
            wheelColliderRL.mass = 60;
            wheelColliderRL.gameObject.layer = LayerMask.NameToLayer("Vehicle");
            wheelColliderRL.gameObject.AddComponent<VehicleWheel>();

            wheelColliderRR.transform.SetParent(wheelColliderParent.transform);
            wheelColliderRR.transform.position = WheelRR.position;
            wheelColliderRR.center = new Vector3(0, 0.15f, 0);
            wheelColliderRR.radius = WheelColliderRaduis(WheelRR);
            wheelColliderRR.mass = 60;
            wheelColliderRR.gameObject.layer = LayerMask.NameToLayer("Vehicle");
            wheelColliderRR.gameObject.AddComponent<VehicleWheel>();

            RGSKCar car = !vehicle.gameObject.GetComponent<RGSKCar>() ? vehicle.gameObject.AddComponent<RGSKCar>()
                            : vehicle.gameObject.GetComponent<RGSKCar>();

            car.engineTorque = MaxEngineTorque;
            car.engineTorqueRPM = MaxEngineTorqueRPM;
            car.minEngineRPM = MinRPM;
            car.maxEngineRPM = MaxRPM;
            car.minShiftDownRPM = (MaxRPM / 2) - 500;
            car.maxShiftDownRPM = (MaxRPM / 2) + 1000;
            car.gearRatios = this.gearRatios;
            car.finalDriveRatio = this.finalDrive;

            //Clear the wheels incase this is a reconfig
            car.vehicleWheels.Clear();

            car.vehicleWheels.Add(new RGSKCar.WheelInfo(wheelColliderFL, WheelFL,
                wheelDrive == WheelDrive.FWD || wheelDrive == WheelDrive.AWD, true, true, false));

            car.vehicleWheels.Add(new RGSKCar.WheelInfo(wheelColliderFR, WheelFR,
                wheelDrive == WheelDrive.FWD || wheelDrive == WheelDrive.AWD, true, true, false));

            car.vehicleWheels.Add(new RGSKCar.WheelInfo(wheelColliderRL, WheelRL,
                wheelDrive == WheelDrive.RWD || wheelDrive == WheelDrive.AWD, false, true, true));

            car.vehicleWheels.Add(new RGSKCar.WheelInfo(wheelColliderRR, WheelRR,
                wheelDrive == WheelDrive.RWD || wheelDrive == WheelDrive.AWD, false, true, true));

            //Configure axles
            car.vehicleWheels[0].wheelAxle = WheelAxle.Front;
			car.vehicleWheels [1].wheelAxle = WheelAxle.Front;
            car.vehicleWheels[2].wheelAxle = WheelAxle.Rear;
            car.vehicleWheels[3].wheelAxle = WheelAxle.Rear;
        }

        //Congigure Motorbike
        if (vehicleType == VehicleType.Motorbike)
        {
            GameObject chassis = new GameObject("Chassis");
            chassis.transform.SetParent(vehicle, false);

            GameObject wheelColliderParent = new GameObject("Wheel Colliders");
            wheelColliderParent.transform.SetParent(vehicle, false);

            WheelCollider wheelColliderFront = new GameObject("Wheel Collider Front").AddComponent<WheelCollider>();
            WheelCollider wheelColliderRear = new GameObject("Wheel Collider Rear").AddComponent<WheelCollider>();

            wheelColliderFront.transform.SetParent(wheelColliderParent.transform);
            wheelColliderFront.transform.position = frontMotorbikeWheel.position;
            wheelColliderFront.center = new Vector3(0, 0.15f, 0);
            wheelColliderFront.radius = WheelColliderRaduis(frontMotorbikeWheel);
            wheelColliderFront.mass = 60;
            wheelColliderFront.gameObject.layer = LayerMask.NameToLayer("Vehicle");
            wheelColliderFront.gameObject.AddComponent<VehicleWheel>();

            wheelColliderRear.transform.SetParent(wheelColliderParent.transform);
            wheelColliderRear.transform.position = rearMotorbikeWheel.position;
            wheelColliderRear.center = new Vector3(0, 0.15f, 0);
            wheelColliderRear.radius = WheelColliderRaduis(rearMotorbikeWheel);
            wheelColliderRear.mass = 60;
            wheelColliderRear.gameObject.layer = LayerMask.NameToLayer("Vehicle");
            wheelColliderRear.gameObject.AddComponent<VehicleWheel>();

            //Do motorbike instead
            RGSKMotorbike bike = !vehicle.gameObject.GetComponent<RGSKMotorbike>() ? vehicle.gameObject.AddComponent<RGSKMotorbike>()
                           : vehicle.gameObject.GetComponent<RGSKMotorbike>();

            bike.chassis = chassis.transform;
            bike.engineTorque = MaxEngineTorque;
            bike.engineTorqueRPM = MaxEngineTorqueRPM;
            bike.minEngineRPM = MinRPM;
            bike.maxEngineRPM = MaxRPM;
            bike.minShiftDownRPM = (MaxRPM / 2) - 500;
            bike.maxShiftDownRPM = (MaxRPM / 2) + 1000;
            bike.gearRatios = this.gearRatios;
            bike.finalDriveRatio = this.finalDrive;

            //Clear the wheels incase this is a reconfig
            bike.vehicleWheels.Clear();

            bike.vehicleWheels.Add(new RGSKMotorbike.WheelInfo(wheelColliderFront, frontMotorbikeWheel,
                false, true, true, false));

            bike.vehicleWheels.Add(new RGSKMotorbike.WheelInfo(wheelColliderRear, rearMotorbikeWheel,
                true, false, true, true));

            //Set the front wheel to front axle
            bike.vehicleWheels[0].wheelAxle = WheelAxle.Front;
            bike.vehicleWheels[1].wheelAxle = WheelAxle.Rear;

            //Preset the fricton curve to 2
            bike.frictionCurveID = 2;

            //Make the chassis the parent of all
            List<Transform> list = new List<Transform>();
            foreach (Transform t in vehicle)
            {
                if (t != chassis.transform && t != wheelColliderParent.transform)
                    list.Add(t);
            }
            foreach (Transform c in list)
            {
                c.parent = chassis.transform;
            }
        }

        //Add race / input components
        if (!vehicle.gameObject.GetComponent<PlayerInput>())
        {
            vehicle.gameObject.AddComponent<PlayerInput>();
        }

        //Get the statistics compoment
        RacerStatistics statistics = !vehicle.gameObject.GetComponent<RacerStatistics>() ? vehicle.gameObject.AddComponent<RacerStatistics>()
                            : vehicle.gameObject.GetComponent<RacerStatistics>();

        //Create the player tag
        vehicle.tag = "Player";

        //Assign a vehicle name
        if(statistics.GetVehicle() == string.Empty)
            statistics.racerInformation.vehicleName = vehicleName;

        //Add the nitro component
        if (addNitro)
        {
            if (!vehicle.gameObject.GetComponent<Nitro>())
                vehicle.gameObject.AddComponent<Nitro>();
        }

        //Add the slipstream component
        if (addSlipstream)
        {
            if (!vehicle.gameObject.GetComponent<Slipstream>())
                vehicle.gameObject.AddComponent<Slipstream>();
        }

        //Finish
        debugString = "The vehicle has been succesfully configured";

        //Clear all the values incase user wants to configure more vehicles
        vehicle = null;
        WheelFL = null;
        WheelFR = null;
        WheelRL = null;
        WheelRR = null;
        frontMotorbikeWheel = null;
        rearMotorbikeWheel = null;
        vehicleName = string.Empty;
    }


    float WheelColliderRaduis(Transform targetWheel)
    {
        Bounds bounds = new Bounds();
        Renderer[] renderers = targetWheel.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            bounds = r.bounds;
            bounds.Encapsulate(r.bounds);
        }

        return bounds.extents.y;
    }


    void UpdateGUIChanged()
    {
        if(lastVehicleType != vehicleType)
        {
            //Switch preset to macth type
            if (vehicleType == VehicleType.Car)
                vehiclePreset = VehiclePresets.SportsCar;

            if (vehicleType == VehicleType.Motorbike)
                vehiclePreset = VehiclePresets.SportBike;

            lastVehicleType = vehicleType;
        }

        if (lastPreset != vehiclePreset)
        {
            //Apply Presets
            SetVehiclePropertiesFromPreset(vehiclePreset);
            lastPreset = vehiclePreset;
            debugString = "Setting Vehicle Preset : " + vehiclePreset.ToString();
        }
    }


    void CreateVehicleCollider()
    {
        Quaternion rot = vehicle.rotation;
        Bounds combinedBounds = new Bounds(vehicle.position, Vector3.zero);
        MeshRenderer[] renderers = vehicle.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer r in renderers)
        {
            combinedBounds.Encapsulate(r.bounds);
        }

        vehicle.rotation = Quaternion.identity;
        GameObject collider = new GameObject("Collider");
        collider.transform.SetParent(vehicle);
        collider.transform.position = combinedBounds.center;
        collider.AddComponent<BoxCollider>();

        //Offset the collider to ensure the wheels touch the ground
        Vector3 size = new Vector3(combinedBounds.size.x, combinedBounds.size.y - 0.3f, combinedBounds.size.z);
        Vector3 center = new Vector3(0, 0.15f, 0);

        collider.GetComponent<BoxCollider>().size = size;
        collider.GetComponent<BoxCollider>().center = center;

        collider.layer = LayerMask.NameToLayer("Vehicle");
        vehicle.rotation = rot;
    }


    void SetVehiclePropertiesFromPreset(VehiclePresets preset)
    {
        switch (preset)
        {
            case VehiclePresets.SmallSportsCar:
                MaxEngineTorque = 180;
                MaxEngineTorqueRPM = 5000;
                MaxRPM = 7000;
                MinRPM = 1000;
                vehicleMass = 1000;

                finalDrive = 3.45f;

                gearRatios = new float[8];
                gearRatios[0] = -2.6f; //R
                gearRatios[1] = 0f; //N
                gearRatios[2] = 3.5f; //1
                gearRatios[3] = 2.1f; //2
                gearRatios[4] = 1.4f; //3
                gearRatios[5] = 1f; //4
                gearRatios[6] = 0.7f; //5
                gearRatios[7] = 0.5f; //6
                break;

            case VehiclePresets.SportsCar:
                MaxEngineTorque = 300;
                MaxEngineTorqueRPM = 6500;
                MaxRPM = 8000;
                MinRPM = 1000;
                vehicleMass = 1100;

                finalDrive = 4f;

                gearRatios = new float[8];
                gearRatios[0] = -3f; //R
                gearRatios[1] = 0f; //N
                gearRatios[2] = 3.15f; //1
                gearRatios[3] = 2.10f; //2
                gearRatios[4] = 1.48f; //3
                gearRatios[5] = 1.15f; //4
                gearRatios[6] = 0.91f; //5
                gearRatios[7] = 0.82f; //6
                break;

            case VehiclePresets.SuperCar:
                MaxEngineTorque = 600;
                MaxEngineTorqueRPM = 7000;
                MaxRPM = 9000;
                MinRPM = 1000;
                vehicleMass = 1200;

                finalDrive = 3.8f;

                gearRatios = new float[9];
                gearRatios[0] = -3f; //R
                gearRatios[1] = 0f; //N
                gearRatios[2] = 3.1f; //1
                gearRatios[3] = 2.2f; //2
                gearRatios[4] = 1.6f; //3
                gearRatios[5] = 1.3f; //4
                gearRatios[6] = 1.0f; //5
                gearRatios[7] = 0.85f; //6
                gearRatios[8] = 0.75f; //7
                break;

            case VehiclePresets.F1:
                MaxEngineTorque = 700;
                MaxEngineTorqueRPM = 9000;
                MaxRPM = 12000;
                MinRPM = 3000;
                vehicleMass = 750;

                finalDrive = 3.5f;

                gearRatios = new float[10];
                gearRatios[0] = -3f; //R
                gearRatios[1] = 0f; //N
                gearRatios[2] = 2.6f; //1
                gearRatios[3] = 2.1f; //2
                gearRatios[4] = 1.7f; //3
                gearRatios[5] = 1.4f; //4
                gearRatios[6] = 1.25f; //5
                gearRatios[7] = 1.1f; //6
                gearRatios[8] = 1f; //7
                gearRatios[9] = 0.9f; //8
                break;

            case VehiclePresets.GoKart:
                MaxEngineTorque = 20;
                MaxEngineTorqueRPM = 6000;
                MaxRPM = 10000;
                MinRPM = 1000;
                vehicleMass = 500;

                finalDrive = 5.25f;

                gearRatios = new float[3];
                gearRatios[0] = -3f; //R
                gearRatios[1] = 0f; //N
                gearRatios[2] = 4.2f; //1
                break;

            case VehiclePresets.SportBike:
                MaxEngineTorque = 100;
                MaxEngineTorqueRPM = 12000;
                MaxRPM = 14000;
                MinRPM = 1000;
                vehicleMass = 250;

                finalDrive = 2.65f;

                gearRatios = new float[8];
                gearRatios[0] = -3f; //R
                gearRatios[1] = 0f; //N
                gearRatios[2] = 2.75f; //1
                gearRatios[3] = 2f; //2
                gearRatios[4] = 1.6f; //3
                gearRatios[5] = 1.4f; //4
                gearRatios[6] = 1.3f; //5
                gearRatios[7] = 1.2f; //6
                break;

            case VehiclePresets.DirtBike:
                MaxEngineTorque = 70;
                MaxEngineTorqueRPM = 10000;
                MaxRPM = 13000;
                MinRPM = 1000;
                vehicleMass = 250;

                finalDrive = 2.65f;

                gearRatios = new float[7];
                gearRatios[0] = -3f; //R
                gearRatios[1] = 0f; //N
                gearRatios[2] = 2.75f; //1
                gearRatios[3] = 2f; //2
                gearRatios[4] = 1.6f; //3
                gearRatios[5] = 1.4f; //4
                gearRatios[6] = 1.3f; //5
                break;
        }
    }
}
