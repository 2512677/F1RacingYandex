using UnityEngine;
using System.Collections;
using UnityEditor;
using RGSK;
using System.IO;

[CustomEditor(typeof(RGSKMotorbike))]
public class Editor_RGSKMotorbike : Editor
{
    RGSKMotorbike _target;
    string[] toolbarTabs = { "Engine", "Transmission", "Wheels", "Steer/Brake", "Suspension", "Audio", "Stability", "More" };
    GUIStyle centerLabelStyle;
    int editorTab;

    //Engine
    SerializedProperty minEngineRPM;
    SerializedProperty maxEngineRPM;
    SerializedProperty engineTorque;
    SerializedProperty engineTorqueRPM;
    SerializedProperty autoGenerateTorqueCurve;
    SerializedProperty torqueCurveValues;
    SerializedProperty engineTorqueCurve;
    SerializedProperty enginePowerCurve;

    //Transmission
    SerializedProperty gearRatios;
    SerializedProperty finalDriveRatio;
    SerializedProperty shiftUpRPM;
    SerializedProperty minShiftDownRPM;
    SerializedProperty maxShiftDownRPM;
    SerializedProperty shiftTime;
    SerializedProperty transmissionType;
    SerializedProperty maxReverseSpeed;

    //Wheel Settings
    SerializedProperty vehicleWheels;
    SerializedProperty frictionCurveID;

    //Steer / Brakes
    SerializedProperty minSteerAngle;
    SerializedProperty maxSteerAngle;
    SerializedProperty steeringAxis;
    SerializedProperty steeringObject;
    SerializedProperty maxSteeringObjectAngle;
    SerializedProperty steeringObjectAxis;

    SerializedProperty brakeTorque;
    SerializedProperty handbrakeTorque;
    SerializedProperty handbrakeFriction;
    SerializedProperty decelerationRate;

    //Suspension
    SerializedProperty frontSuspensionDistance;
    SerializedProperty frontSuspensionSpring;
    SerializedProperty frontSuspensionDamper;
    SerializedProperty rearSuspensionDistance;
    SerializedProperty rearSuspensionSpring;
    SerializedProperty rearSuspensionDamper;
    SerializedProperty applySuspensionSettings;

    //Audio
    SerializedProperty engineAudio;
    SerializedProperty backfireAudio;
    SerializedProperty gearshiftAudio;
    SerializedProperty windAudio;
    SerializedProperty headlightToggleAudio;
    SerializedProperty wheelImpactAudio;
    SerializedProperty engineAudioPosition;
    SerializedProperty backfireAudioPosition;
    SerializedProperty enginePitchCurve;

    //Stability
    SerializedProperty centerOfMassOffset;
    SerializedProperty centerOfMassPosition;
    SerializedProperty downforce;
    SerializedProperty revShakeAmount;

    //Motorbike Chassis
    SerializedProperty chassis;
    SerializedProperty maxLeanAngle;
    SerializedProperty maxLeanSpeed;
    SerializedProperty minLeanHeight;
    SerializedProperty leanDamping;
    SerializedProperty angularDrag;
    SerializedProperty fender;

    //Motorbike Rider
    SerializedProperty bikeRider;
    SerializedProperty minImpactForce;
    SerializedProperty resetTime;

    //Dashboard
    SerializedProperty speedTextMesh;
    SerializedProperty gearTextMesh;
    SerializedProperty rpmTextMesh;
    SerializedProperty fuelTextMesh;

    SerializedProperty tachometerNeedle;
    SerializedProperty minTachoAngle;
    SerializedProperty maxTachoAngle;
    SerializedProperty tachoOffsetValue;

    SerializedProperty speedometerNeedle;
    SerializedProperty minSpeedoAngle;
    SerializedProperty maxSpeedoAngle;
    SerializedProperty speedoOffsetValue;

    SerializedProperty fuelNeedle;
    SerializedProperty minFuelAngle;
    SerializedProperty maxFuelAngle;
    SerializedProperty fuelOffsetValue;

    //Driving Assists
    SerializedProperty TCS;
    SerializedProperty tcsSlip;
    SerializedProperty ABS;
    SerializedProperty absSlip;
    SerializedProperty steerHelper;

    //Backfire Settings
    SerializedProperty enableBackfire;
    SerializedProperty backfireEffects;

    //Light Settings
    SerializedProperty brakeLights;
    SerializedProperty reverseLights;
    SerializedProperty headLightGameObjects;
    SerializedProperty tailLightGameObjects;

    //Fuel Settings
    SerializedProperty maxFuel;
    SerializedProperty fuelConsumption;
    SerializedProperty fuelConsumptionRate;

    //Other
    SerializedProperty showTelemetry;

    void OnEnable()
    {

        _target = (RGSKMotorbike)target;

        centerLabelStyle = new GUIStyle();
        centerLabelStyle.fontStyle = FontStyle.Normal;
        centerLabelStyle.normal.textColor = Color.white;
        centerLabelStyle.alignment = TextAnchor.MiddleCenter;

        //Engine
        minEngineRPM = serializedObject.FindProperty("minEngineRPM");
        maxEngineRPM = serializedObject.FindProperty("maxEngineRPM");
        engineTorque = serializedObject.FindProperty("engineTorque");
        engineTorqueRPM = serializedObject.FindProperty("engineTorqueRPM");
        autoGenerateTorqueCurve = serializedObject.FindProperty("autoGenerateTorqueCurve");
        torqueCurveValues = serializedObject.FindProperty("torqueCurveValues");
        engineTorqueCurve = serializedObject.FindProperty("engineTorqueCurve");
        enginePowerCurve = serializedObject.FindProperty("enginePowerCurve");

        //Transmission
        gearRatios = serializedObject.FindProperty("gearRatios");
        finalDriveRatio = serializedObject.FindProperty("finalDriveRatio");
        shiftUpRPM = serializedObject.FindProperty("shiftUpRPM");
        minShiftDownRPM = serializedObject.FindProperty("minShiftDownRPM");
        maxShiftDownRPM = serializedObject.FindProperty("maxShiftDownRPM");
        shiftTime = serializedObject.FindProperty("shiftTime");
        transmissionType = serializedObject.FindProperty("transmissionType");
        maxReverseSpeed = serializedObject.FindProperty("maxReverseSpeed");

        //Wheel 
        vehicleWheels = serializedObject.FindProperty("vehicleWheels");
        frictionCurveID = serializedObject.FindProperty("frictionCurveID");

        //Steering / Brakes
        minSteerAngle = serializedObject.FindProperty("minSteerAngle");
        maxSteerAngle = serializedObject.FindProperty("maxSteerAngle");
        steeringObject = serializedObject.FindProperty("steeringObject");
        maxSteeringObjectAngle = serializedObject.FindProperty("maxSteeringObjectAngle");
        steeringObjectAxis = serializedObject.FindProperty("steeringObjectAxis");

        brakeTorque = serializedObject.FindProperty("brakeTorque");
        handbrakeTorque = serializedObject.FindProperty("handbrakeTorque");
        handbrakeFriction = serializedObject.FindProperty("handbrakeFriction");
        decelerationRate = serializedObject.FindProperty("decelerationRate");

        //Suspension
        frontSuspensionDistance = serializedObject.FindProperty("frontSuspensionDistance");
        frontSuspensionSpring = serializedObject.FindProperty("frontSuspensionSpring");
        frontSuspensionDamper = serializedObject.FindProperty("frontSuspensionDamper");
        rearSuspensionDistance = serializedObject.FindProperty("rearSuspensionDistance");
        rearSuspensionSpring = serializedObject.FindProperty("rearSuspensionSpring");
        rearSuspensionDamper = serializedObject.FindProperty("rearSuspensionDamper");
        applySuspensionSettings = serializedObject.FindProperty("applySuspensionSettings");

        //Audio
        engineAudio = serializedObject.FindProperty("engineAudio");
        backfireAudio = serializedObject.FindProperty("backfireAudio");
        gearshiftAudio = serializedObject.FindProperty("gearshiftAudio");
        windAudio = serializedObject.FindProperty("windAudio");
        headlightToggleAudio = serializedObject.FindProperty("headlightToggleAudio");
        wheelImpactAudio = serializedObject.FindProperty("wheelImpactAudio");

        engineAudioPosition = serializedObject.FindProperty("engineAudioPosition");
        backfireAudioPosition = serializedObject.FindProperty("backfireAudioPosition");
        enginePitchCurve = serializedObject.FindProperty("enginePitchCurve");

        //Stability
        centerOfMassOffset = serializedObject.FindProperty("centerOfMassOffset");
        centerOfMassPosition = serializedObject.FindProperty("centerOfMassPosition");
        downforce = serializedObject.FindProperty("downForce");
        revShakeAmount = serializedObject.FindProperty("revShakeAmount");

        //Motorbike Chassis
        chassis = serializedObject.FindProperty("chassis");
        maxLeanAngle = serializedObject.FindProperty("maxLeanAngle");
        maxLeanSpeed = serializedObject.FindProperty("maxLeanSpeed");
        minLeanHeight = serializedObject.FindProperty("minLeanHeight");
        leanDamping = serializedObject.FindProperty("leanDamping");
        fender = serializedObject.FindProperty("fender");
        angularDrag = serializedObject.FindProperty("angularDrag");

        //Motorbike Rider
        bikeRider = serializedObject.FindProperty("bikeRider");
        minImpactForce = serializedObject.FindProperty("minImpactForce");
        resetTime = serializedObject.FindProperty("resetTime");

        //Dashboard
        speedTextMesh = serializedObject.FindProperty("speedTextMesh");
        gearTextMesh = serializedObject.FindProperty("gearTextMesh");
        rpmTextMesh = serializedObject.FindProperty("rpmTextMesh");
        fuelTextMesh = serializedObject.FindProperty("fuelTextMesh");

        tachometerNeedle = serializedObject.FindProperty("tachometerNeedle");
        minTachoAngle = serializedObject.FindProperty("minTachoAngle");
        maxTachoAngle = serializedObject.FindProperty("maxTachoAngle");
        tachoOffsetValue = serializedObject.FindProperty("tachoOffsetValue");

        speedometerNeedle = serializedObject.FindProperty("speedometerNeedle");
        minSpeedoAngle = serializedObject.FindProperty("minSpeedoAngle");
        maxSpeedoAngle = serializedObject.FindProperty("maxSpeedoAngle");
        speedoOffsetValue = serializedObject.FindProperty("speedoOffsetValue");

        fuelNeedle = serializedObject.FindProperty("fuelNeedle");
        minFuelAngle = serializedObject.FindProperty("minFuelAngle");
        maxFuelAngle = serializedObject.FindProperty("maxFuelAngle");
        fuelOffsetValue = serializedObject.FindProperty("fuelOffsetValue");

        //Driving Assists
        TCS = serializedObject.FindProperty("TCS");
        tcsSlip = serializedObject.FindProperty("tcsSlip");
        ABS = serializedObject.FindProperty("ABS");
        absSlip = serializedObject.FindProperty("absSlip");
        steerHelper = serializedObject.FindProperty("steerHelper");

        //Backfire Settings
        enableBackfire = serializedObject.FindProperty("enableBackfire");
        backfireEffects = serializedObject.FindProperty("backfireEffects");

        //Light Settings
        brakeLights = serializedObject.FindProperty("brakeLights");
        reverseLights = serializedObject.FindProperty("reverseLights");
        headLightGameObjects = serializedObject.FindProperty("headLightGameObjects");
        tailLightGameObjects = serializedObject.FindProperty("tailLightGameObjects");

        //Fuel Settings
        maxFuel = serializedObject.FindProperty("maxFuel");
        fuelConsumption = serializedObject.FindProperty("fuelConsumption");
        fuelConsumptionRate = serializedObject.FindProperty("fuelConsumptionRate");

        //Other
        showTelemetry = serializedObject.FindProperty("showTelemetry");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        editorTab = GUILayout.SelectionGrid(editorTab, toolbarTabs, 4);

        if (EditorGUI.EndChangeCheck())
        {
            GUI.FocusControl(null);
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        switch (editorTab)
        {
            //ENGINE
            case 0:

                EditorGUILayout.LabelField("Engine Settings", centerLabelStyle);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(minEngineRPM);
                EditorGUILayout.PropertyField(maxEngineRPM);
                EditorGUILayout.PropertyField(engineTorque);
                EditorGUILayout.PropertyField(engineTorqueRPM);
                EditorGUILayout.PropertyField(autoGenerateTorqueCurve);

                if (!_target.autoGenerateTorqueCurve)
                {
                    EditorGUILayout.PropertyField(torqueCurveValues, true);
                }

                EditorGUI.BeginDisabledGroup(false);
                EditorGUILayout.PropertyField(engineTorqueCurve);
                EditorGUILayout.PropertyField(enginePowerCurve);
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Tech Specs", centerLabelStyle);
                EditorGUILayout.Space();

                if (_target.engineTorqueCurve != null && _target.enginePowerCurve != null)
                {
                    if (_target.engineTorqueCurve.length > 0 && _target.enginePowerCurve.length > 0)
                    {
                        EditorGUILayout.LabelField("Max Torque: " + (int)_target.MaximunTorque() + " Nm @ " + _target.MaximunTorqueRPM() + " rpm");
                        EditorGUILayout.LabelField("Max Horsepower: " + (int)_target.MaximumHorsepower() + " Hp @ " + _target.MaximumHorsepowerRPM() + " rpm");
                    }
                }

                if (GUILayout.Button("Update Tech Specs"))
                {
                    _target.SetupEngineTorqueCurve();
                }
                break;

            //TRANSMISSION
            case 1:

                EditorGUILayout.LabelField("Transmission Settings", centerLabelStyle);
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(gearRatios, true);
                EditorGUILayout.PropertyField(maxReverseSpeed);
                EditorGUILayout.PropertyField(finalDriveRatio);
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(transmissionType);
                EditorGUILayout.PropertyField(shiftTime);
                EditorGUILayout.PropertyField(shiftUpRPM);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Shift Down RPM Range");
                EditorGUILayout.PropertyField(minShiftDownRPM, new GUIContent(""));
                EditorGUILayout.PropertyField(maxShiftDownRPM, new GUIContent(""));
                EditorGUILayout.EndHorizontal();
                break;

            //WHEELS
            case 2:
                EditorGUILayout.LabelField("Wheel Settings", centerLabelStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(vehicleWheels, true);
                EditorGUILayout.PropertyField(frictionCurveID);
                break;

            //STEER / BRAKE
            case 3:

                EditorGUILayout.LabelField("Steer Settings", centerLabelStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(minSteerAngle);
                EditorGUILayout.PropertyField(maxSteerAngle);
                EditorGUILayout.PropertyField(steeringObject);
                EditorGUILayout.PropertyField(fender);
                EditorGUILayout.PropertyField(maxSteeringObjectAngle);
                EditorGUILayout.PropertyField(steeringObjectAxis);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Brake Settings", centerLabelStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(brakeTorque);
                EditorGUILayout.PropertyField(handbrakeTorque);
                EditorGUILayout.PropertyField(handbrakeFriction);
                EditorGUILayout.PropertyField(decelerationRate);
                break;

            //SUSPENSION
            case 4:
                EditorGUILayout.LabelField("Suspension Settings", centerLabelStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(frontSuspensionDistance);
                EditorGUILayout.PropertyField(frontSuspensionSpring);
                EditorGUILayout.PropertyField(frontSuspensionDamper);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(rearSuspensionDistance);
                EditorGUILayout.PropertyField(rearSuspensionSpring);
                EditorGUILayout.PropertyField(rearSuspensionDamper);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(applySuspensionSettings);
                break;

            //AUDIO
            case 5:
                EditorGUILayout.LabelField("Audio Settings", centerLabelStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(engineAudio);
                EditorGUILayout.PropertyField(backfireAudio);
                EditorGUILayout.PropertyField(gearshiftAudio);
                EditorGUILayout.PropertyField(windAudio);
                EditorGUILayout.PropertyField(headlightToggleAudio);
                EditorGUILayout.PropertyField(wheelImpactAudio);
                EditorGUILayout.PropertyField(engineAudioPosition);
                EditorGUILayout.PropertyField(backfireAudioPosition);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(enginePitchCurve);
                EditorGUI.EndDisabledGroup();
                break;

            //STABILTY
            case 6:
                EditorGUILayout.LabelField("Stability Settings", centerLabelStyle);
                EditorGUILayout.Space();
                if (_target.centerOfMassPosition == null)
                {
                    EditorGUILayout.HelpBox("The center of mass position has not been assigned. The center of mass offset will be used as the vehicle's center of mass position.", MessageType.Info);
                }
                EditorGUILayout.PropertyField(centerOfMassPosition);
                EditorGUILayout.PropertyField(centerOfMassOffset);
                EditorGUILayout.PropertyField(downforce);
                EditorGUILayout.PropertyField(revShakeAmount, true);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                EditorGUILayout.LabelField("Chassis Settings", centerLabelStyle);
                EditorGUILayout.PropertyField(chassis);
                EditorGUILayout.PropertyField(maxLeanAngle);
                EditorGUILayout.PropertyField(maxLeanSpeed);
                EditorGUILayout.PropertyField(minLeanHeight);
                EditorGUILayout.PropertyField(leanDamping);
                EditorGUILayout.PropertyField(angularDrag);
                break;

            //MORE
            case 7:
                //RIDER
                EditorGUILayout.LabelField("Rider Settings", centerLabelStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(bikeRider);
                EditorGUILayout.PropertyField(minImpactForce);
                EditorGUILayout.PropertyField(resetTime);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                //ASSIST
                EditorGUILayout.LabelField("Assist Settings", centerLabelStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(TCS);
                EditorGUILayout.PropertyField(tcsSlip);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(ABS);
                EditorGUILayout.PropertyField(absSlip);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(steerHelper);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                //BACKFIRE
                EditorGUILayout.LabelField("Backfire Settings", centerLabelStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(enableBackfire);
                EditorGUILayout.PropertyField(backfireEffects, true);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                //LIGHT
                EditorGUILayout.LabelField("Light Settings", centerLabelStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(headLightGameObjects);
                EditorGUILayout.PropertyField(tailLightGameObjects);
                EditorGUILayout.PropertyField(brakeLights);
                EditorGUILayout.PropertyField(reverseLights);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                //FUEL
                EditorGUILayout.LabelField("Fuel Settings", centerLabelStyle);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Fuel: " + _target.fuel);
                EditorGUILayout.PropertyField(fuelConsumption);
                EditorGUILayout.PropertyField(maxFuel);
                EditorGUILayout.PropertyField(fuelConsumptionRate);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                //DASHBOARD
                EditorGUILayout.LabelField("Dashboard Settings", centerLabelStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(speedTextMesh);
                EditorGUILayout.PropertyField(gearTextMesh);
                EditorGUILayout.PropertyField(rpmTextMesh);
                EditorGUILayout.PropertyField(fuelTextMesh);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(speedometerNeedle);
                EditorGUILayout.PropertyField(minSpeedoAngle);
                EditorGUILayout.PropertyField(maxSpeedoAngle);
                EditorGUILayout.PropertyField(speedoOffsetValue);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(tachometerNeedle);
                EditorGUILayout.PropertyField(minTachoAngle);
                EditorGUILayout.PropertyField(maxTachoAngle);
                EditorGUILayout.PropertyField(tachoOffsetValue);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(fuelNeedle);
                EditorGUILayout.PropertyField(minFuelAngle);
                EditorGUILayout.PropertyField(maxFuelAngle);
                EditorGUILayout.PropertyField(fuelOffsetValue);

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                //TELEMETRY
                EditorGUILayout.LabelField("Telemetry", centerLabelStyle);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(showTelemetry);

                break;
        }


        serializedObject.ApplyModifiedProperties();
    }
}
