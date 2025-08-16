//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

#if RCC_PHOTON && PHOTON_UNITY_NETWORKING
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon;
using Photon.Pun;

/// <summary>
/// Streaming player input or receiving data from the server, then feeding these inputs into the RCC.
/// This class handles synchronization of car inputs, transforms, and collision events via Photon.
/// </summary>
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(RCC_CarControllerV4))]
public class RCC_PhotonSync : Photon.Pun.MonoBehaviourPunCallbacks, IPunObservable {

    /// <summary>
    /// Indicates if the Photon connection is established.
    /// </summary>
    public bool connected = false;

    /// <summary>
    /// Indicates if this Photon vehicle instance is owned locally.
    /// </summary>
    public bool isMine = false;

    /// <summary>
    /// Reference to the main RCC car controller.
    /// </summary>
    private RCC_CarControllerV4 carController;

    /// <summary>
    /// Reference to the vehicle's Rigidbody component.
    /// </summary>
    private Rigidbody rigid;

    /// <summary>
    /// The position of the vehicle synchronized over the network.
    /// </summary>
    private Vector3 correctPlayerPos;

    /// <summary>
    /// The rotation of the vehicle synchronized over the network.
    /// </summary>
    private Quaternion correctPlayerRot;

    /// <summary>
    /// The current velocity used for projected (interpolated) positioning.
    /// </summary>
    private Vector3 currentVelocity;

    /// <summary>
    /// The current angular velocity used for projected (interpolated) rotation.
    /// </summary>
    private Vector3 currentAngularVelocity;

    /// <summary>
    /// Timestamp used for interpolation calculations.
    /// </summary>
    private float updateTime = 0;

    /// <summary>
    /// The gas (throttle) input to be sent or received.
    /// </summary>
    private float gasInput = 0f;

    /// <summary>
    /// The brake input to be sent or received.
    /// </summary>
    private float brakeInput = 0f;

    /// <summary>
    /// The steering input to be sent or received.
    /// </summary>
    private float steerInput = 0f;

    /// <summary>
    /// The handbrake input to be sent or received.
    /// </summary>
    private float handbrakeInput = 0f;

    /// <summary>
    /// The boost input to be sent or received.
    /// </summary>
    private float boostInput = 0f;

    /// <summary>
    /// The clutch input to be sent or received.
    /// </summary>
    private float clutchInput = 0f;

    /// <summary>
    /// Current gear of the vehicle.
    /// </summary>
    private int gear = 0;

    /// <summary>
    /// Current directional state of the vehicle (1 for forward, -1 for reverse).
    /// </summary>
    private int direction = 1;

    /// <summary>
    /// Indicates if the vehicle is currently changing gear.
    /// </summary>
    private bool changingGear = false;

    /// <summary>
    /// Indicates if the vehicle is using a semi-automatic gear system.
    /// </summary>
    private bool semiAutomaticGear = false;

    /// <summary>
    /// Current fuel input level of the vehicle.
    /// </summary>
    private float fuelInput = 1f;

    /// <summary>
    /// Indicates if the vehicle's engine is running.
    /// </summary>
    private bool engineRunning = false;

    /// <summary>
    /// Indicates if low-beam head lights are on.
    /// </summary>
    private bool lowBeamHeadLightsOn = false;

    /// <summary>
    /// Indicates if high-beam head lights are on.
    /// </summary>
    private bool highBeamHeadLightsOn = false;

    /// <summary>
    /// Current state of the turn indicators.
    /// </summary>
    private RCC_CarControllerV4.IndicatorsOn indicatorsOn;

    /// <summary>
    /// The TextMesh used to display the player's nickname above the vehicle.
    /// </summary>
    private TextMesh nicknameText;

    private Vector3 refVel;

    /// <summary>
    /// Unity's Start method. Initializes references, sets up nickname text, and configures synchronization.
    /// </summary>
    private void Start() {

        // Getting RCC, Rigidbody. 
        carController = GetComponent<RCC_CarControllerV4>();
        rigid = GetComponent<Rigidbody>();

        GameObject nicknameTextGO = new GameObject("NickName TextMesh");
        nicknameTextGO.transform.SetParent(transform, false);
        nicknameTextGO.transform.localPosition = new Vector3(0f, 2f, 0f);
        nicknameTextGO.transform.localScale = new Vector3(.25f, .25f, .25f);
        nicknameText = nicknameTextGO.AddComponent<TextMesh>();
        nicknameText.anchor = TextAnchor.MiddleCenter;
        nicknameText.fontSize = 25;

        photonView.Synchronization = ViewSynchronization.Unreliable;

        GetValues();

        // Setting name of the gameobject with Photon View ID.
        gameObject.name = gameObject.name + photonView.ViewID;

    }


    void Awake()
    {
        // если нет комнаты и не OfflineMode → скрипт не нужен
        if (!PhotonNetwork.InRoom && !PhotonNetwork.OfflineMode)
        {
            enabled = false;
            return;
        }
    }


    /// <summary>
    /// Retrieves the initial values from the RCC car controller for synchronization.
    /// </summary>
    private void GetValues() {



        correctPlayerPos = transform.position;
        correctPlayerRot = transform.rotation;

        gasInput = carController.throttleInput;
        brakeInput = carController.brakeInput;
        steerInput = carController.steerInput;
        handbrakeInput = carController.handbrakeInput;
        boostInput = carController.boostInput;
        clutchInput = carController.clutchInput;
        gear = carController.currentGear;
        direction = carController.direction;
        changingGear = carController.changingGear;
        semiAutomaticGear = carController.semiAutomaticGear;

        fuelInput = carController.fuelInput;
        engineRunning = carController.engineRunning;
        lowBeamHeadLightsOn = carController.lowBeamHeadLightsOn;
        highBeamHeadLightsOn = carController.highBeamHeadLightsOn;
        indicatorsOn = carController.indicatorsOn;

    }

    /// <summary>
    /// Unity's FixedUpdate method. Handles ownership logic, interpolation for remote vehicles, and nickname display.
    /// </summary>
    private void FixedUpdate() {

        if (!PhotonNetwork.IsConnectedAndReady)
            return;

        if (!carController)
            return;

        isMine = photonView.IsMine;

        if (photonView.OwnershipTransfer == OwnershipOption.Fixed) {

            // If we are the owner of this vehicle, disable external controller and enable controller of the vehicle. Do opposite if we don't own this.
            carController.externalController = !isMine;
            carController.canControl = isMine;

        }

        // If we are not owner of this vehicle, receive all inputs from server.
        if (!isMine) {

            Vector3 projectedPosition = correctPlayerPos + currentVelocity * (Time.time - updateTime);

            float teleportDistanceThreshold = 10f;

            if (Vector3.Distance(transform.position, correctPlayerPos) < teleportDistanceThreshold) {

                transform.position = Vector3.SmoothDamp(transform.position, projectedPosition, ref refVel, 0.1f);
                transform.rotation = Quaternion.Slerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5f);

            } else {

                transform.position = correctPlayerPos;
                transform.rotation = correctPlayerRot;

            }

            rigid.linearVelocity = currentVelocity;
            rigid.angularVelocity = currentAngularVelocity;

            carController.throttleInput = gasInput;
            carController.brakeInput = brakeInput;
            carController.steerInput = steerInput;
            carController.handbrakeInput = handbrakeInput;
            carController.boostInput = boostInput;
            carController.clutchInput = clutchInput;
            carController.currentGear = gear;
            carController.direction = direction;
            carController.changingGear = changingGear;
            carController.semiAutomaticGear = semiAutomaticGear;

            carController.fuelInput = fuelInput;
            carController.engineRunning = engineRunning;
            carController.lowBeamHeadLightsOn = lowBeamHeadLightsOn;
            carController.highBeamHeadLightsOn = highBeamHeadLightsOn;
            carController.indicatorsOn = indicatorsOn;

            if (nicknameText && photonView.Owner != null)
                nicknameText.text = photonView.Owner.NickName;
            else
                nicknameText.text = "";

            if (RCC_SceneManager.Instance.activeMainCamera) {

                Vector3 lookDirection = (nicknameText.transform.position - RCC_SceneManager.Instance.activeMainCamera.transform.position).normalized;
                nicknameText.transform.rotation = Quaternion.LookRotation(lookDirection);

            }

        } else {

            if (nicknameText)
                nicknameText.text = "";

        }

    }

    /// <summary>
    /// When a collision occurs on the owner’s side, send collision data to other clients.
    /// </summary>
    /// <param name="collision">The collision data provided by Unity.</param>
    private void OnCollisionEnter(Collision collision) {

        if (photonView.IsMine) {

            if (collision.contacts.Length > 0) {

                // Use the first contact point for simplicity.
                Vector3 contactPoint = collision.contacts[0].point;
                Vector3 relativeVelocity = collision.relativeVelocity;

                // Approximate impact force
                float impactForce = collision.impulse.magnitude / 1f;

                RCC_PhotonSync syncVehicle;
                syncVehicle = collision.gameObject.GetComponentInParent<RCC_PhotonSync>();

                if (!syncVehicle)
                    return;

                // Send collision data to other clients.
                syncVehicle.photonView.RPC("RPC_SyncCollision", RpcTarget.AllBuffered, contactPoint, relativeVelocity, impactForce, syncVehicle.photonView.ViewID, rigid.angularVelocity);

            }

        }

    }

    /// <summary>
    /// RPC method to sync collision data.
    /// </summary>
    /// <param name="collisionPoint">The point where the collision occurred.</param>
    /// <param name="relativeVelocity">The relative velocity of the collision.</param>
    /// <param name="impactForce">An approximation of the impact force.</param>
    /// <param name="viewID">The view ID of the vehicle involved in the collision.</param>
    [PunRPC]
    public void RPC_SyncCollision(Vector3 collisionPoint, Vector3 relativeVelocity, float impactForce, int viewID, Vector3 angularVelocity) {

        if (viewID != photonView.ViewID)
            return;

        // Apply a collision effect locally, for example, by adding an impulse force.
        impactForce = Mathf.Clamp(impactForce, 0, 10000f);

        // Setting angular velocity.
        rigid.angularVelocity = angularVelocity;

        // Apply force in the direction of the relative velocity.
        rigid.AddForceAtPosition(-relativeVelocity.normalized * impactForce, collisionPoint, ForceMode.Impulse);
        rigid.angularVelocity = angularVelocity;

    }

    /// <summary>
    /// Implements IPunObservable to send or receive data through the Photon network.
    /// </summary>
    /// <param name="stream">PhotonStream for sending or receiving data.</param>
    /// <param name="info">Information about the message (sender, timestamp, etc.).</param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {

        if (!carController)
            return;

        // Sending all inputs, position, rotation, and velocity to the server.
        if (stream.IsWriting) {

            //We own this player: send the others our data
            stream.SendNext(carController.throttleInput);
            stream.SendNext(carController.brakeInput);
            stream.SendNext(carController.steerInput);
            stream.SendNext(carController.handbrakeInput);
            stream.SendNext(carController.boostInput);
            stream.SendNext(carController.clutchInput);
            stream.SendNext(carController.currentGear);
            stream.SendNext(carController.direction);
            stream.SendNext(carController.changingGear);
            stream.SendNext(carController.semiAutomaticGear);

            stream.SendNext(carController.fuelInput);
            stream.SendNext(carController.engineRunning);
            stream.SendNext(carController.lowBeamHeadLightsOn);
            stream.SendNext(carController.highBeamHeadLightsOn);
            stream.SendNext(carController.indicatorsOn);

            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(rigid.linearVelocity);
            stream.SendNext(rigid.angularVelocity);

        } else {

            // Network player, receiving all inputs, position, rotation, and velocity from server. 
            gasInput = (float)stream.ReceiveNext();
            brakeInput = (float)stream.ReceiveNext();
            steerInput = (float)stream.ReceiveNext();
            handbrakeInput = (float)stream.ReceiveNext();
            boostInput = (float)stream.ReceiveNext();
            clutchInput = (float)stream.ReceiveNext();
            gear = (int)stream.ReceiveNext();
            direction = (int)stream.ReceiveNext();
            changingGear = (bool)stream.ReceiveNext();
            semiAutomaticGear = (bool)stream.ReceiveNext();

            fuelInput = (float)stream.ReceiveNext();
            engineRunning = (bool)stream.ReceiveNext();
            lowBeamHeadLightsOn = (bool)stream.ReceiveNext();
            highBeamHeadLightsOn = (bool)stream.ReceiveNext();
            indicatorsOn = (RCC_CarControllerV4.IndicatorsOn)stream.ReceiveNext();

            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();
            currentVelocity = (Vector3)stream.ReceiveNext();
            currentAngularVelocity = (Vector3)stream.ReceiveNext();

            updateTime = Time.time;

        }

    }

}
#endif
