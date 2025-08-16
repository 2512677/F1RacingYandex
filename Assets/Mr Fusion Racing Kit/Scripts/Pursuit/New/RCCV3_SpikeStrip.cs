using RGSK;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("BoneCracker Games/Realistic Car Controller/Misc/RCC v4 SpikeStrip (Permanent, Player + AI)")]
[RequireComponent(typeof(BoxCollider))]
public class RCCV3_SpikeStrip : MonoBehaviour
{
    [Header("Параметры прокола")]
    [Tooltip("Во сколько раз уменьшится радиус колеса при проколе")]
    public float deflatedRadiusMultiplier = 0.8f;
    [Tooltip("Во сколько раз умножается stiffness при проколе")]
    public float deflatedStiffnessMultiplier = 0.5f;

    [Space]
    [Header("Параметры снижения скорости")]
    [Tooltip("Максимальная скорость автомобиля после прокола (км/ч)")]
    public float spikeMaxSpeed = 30f;

    private BoxCollider _col;

    // оригинальные maxspeed для каждой машины (игрок или бот)
    private readonly Dictionary<RCC_CarControllerV4, float> _originalMaxSpeeds
        = new Dictionary<RCC_CarControllerV4, float>();

    // оригинальные параметры каждого проколотого колеса
    private readonly Dictionary<RCC_WheelCollider, OriginalWheelData> _deflatedWheels
        = new Dictionary<RCC_WheelCollider, OriginalWheelData>();

    private class OriginalWheelData
    {
        public float originalRadius;
        public float originalForwardStiffness;
        public float originalSidewaysStiffness;
    }

    private void Awake()
    {
        _col = GetComponent<BoxCollider>();
        _col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var root = other.transform.root;
        if (root == null) return;

        RCC_CarControllerV4 car = root.GetComponent<RCC_CarControllerV4>();
        var aiLogic = root.GetComponent<AiLogic>();
        if (car == null && aiLogic != null)
            car = root.GetComponentInChildren<RCC_CarControllerV4>();

        if (car != null)
        {
            if (!_originalMaxSpeeds.ContainsKey(car))
            {
                _originalMaxSpeeds[car] = car.maxspeed;
                car.maxspeed = spikeMaxSpeed;
                Debug.Log($"[SpikeStrip] Машина '{car.name}' попала на шипы. maxspeed → {spikeMaxSpeed}");
            }
        }

        var wheel = other.GetComponent<RCC_WheelCollider>();
        if (wheel != null)
            TryDeflateWheel(wheel);
    }

    private void TryDeflateWheel(RCC_WheelCollider wheel)
    {
        if (_deflatedWheels.ContainsKey(wheel))
            return;

        var orig = new OriginalWheelData
        {
            originalRadius = wheel.WheelCollider.radius,
            originalForwardStiffness = wheel.WheelCollider.forwardFriction.stiffness,
            originalSidewaysStiffness = wheel.WheelCollider.sidewaysFriction.stiffness
        };
        _deflatedWheels[wheel] = orig;

        float newRadius = orig.originalRadius * deflatedRadiusMultiplier;
        wheel.WheelCollider.radius = newRadius;

        var ffc = wheel.WheelCollider.forwardFriction;
        ffc.stiffness = orig.originalForwardStiffness * deflatedStiffnessMultiplier;
        wheel.WheelCollider.forwardFriction = ffc;

        var sfc = wheel.WheelCollider.sidewaysFriction;
        sfc.stiffness = orig.originalSidewaysStiffness * deflatedStiffnessMultiplier;
        wheel.WheelCollider.sidewaysFriction = sfc;

        Debug.Log($"[SpikeStrip] Проколотое колесо '{wheel.name}'.");
    }
}
