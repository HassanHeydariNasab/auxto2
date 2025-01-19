using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarControl : MonoBehaviour
{

    // f: Front, l: Left, r: Right, b: Back, c: Collider, m: Mesh
    public WheelCollider frwc, flwc, brwc, blwc;
    public MeshRenderer frwm, flwm, brwm, blwm;

    [SerializeField] private MeshRenderer _bodyMeshRenderer;

    [SerializeField] private TMP_Text _speedometer;

    [SerializeField] private Rigidbody _rb;

    [SerializeField] private InputActionReference _forwardAction;
    [SerializeField] private InputActionReference _steerAction;
    [SerializeField] private InputActionReference _handbrakeAction;

    [SerializeField] private Light _rearLeftLight, _rearRightLight;

    [SerializeField] private AudioSource engineSound;
    [SerializeField] private AudioSource skidSound;

    Vector3 _acceleration = new Vector3(0, 0, 0);
    bool _hasAcceleration = false;
    private float _forward = 0f;
    private float _motor = 1000f;
    private float _steer = 0f;

    private float _measuredSpeed = 0f;
    private float _forwardVelocity = 0f;

    private WheelHit[] _wheelHits = new WheelHit[4];
    private float _averageWheelHitForwardSlip = 0f;


    void Start()
    {
        _hasAcceleration = SystemInfo.supportsAccelerometer;
        if (Accelerometer.current != null)
        {
            InputSystem.EnableDevice(Accelerometer.current);
        }
    }

    void FixedUpdate()
    {
        if (_hasAcceleration)
        {
            _acceleration = Accelerometer.current.acceleration.ReadValue();
        }

        var forwardActionValue = _forwardAction.action.ReadValue<float>();
        var steerActionValue = _steerAction.action.ReadValue<float>();
        var handbrakeActionValue = _handbrakeAction.action.ReadValue<float>();

        if (_hasAcceleration)
        {
            _forward = _acceleration.y + 0.75f;
            _steer = _acceleration.x;
        }
        else
        {
            _forward = forwardActionValue;
            _steer = steerActionValue;
        }

        frwc.motorTorque = _motor * _forward;
        flwc.motorTorque = _motor * _forward;

        frwc.steerAngle = 30f * _steer;
        flwc.steerAngle = 30f * _steer;

        if (_handbrakeAction.action.IsPressed())
        {
            brwc.brakeTorque = 10000f * handbrakeActionValue;
            blwc.brakeTorque = 10000f * handbrakeActionValue;
        }
        else if (!_handbrakeAction.action.IsPressed())
        {
            brwc.brakeTorque = 0f;
            blwc.brakeTorque = 0f;
        }


        frwc.GetGroundHit(out _wheelHits[0]);
        flwc.GetGroundHit(out _wheelHits[1]);
        brwc.GetGroundHit(out _wheelHits[2]);
        blwc.GetGroundHit(out _wheelHits[3]);

        float wheelHitForwardSlipSum = 0;
        for (int i = 0; i < _wheelHits.Length; i++)
        {
            wheelHitForwardSlipSum += _wheelHits[i].forwardSlip;
        }

        _averageWheelHitForwardSlip = wheelHitForwardSlipSum / _wheelHits.Length;

        if (_forward < 0 && _forwardVelocity > 10 || _forward > 0 && _forwardVelocity < -10)
        {
            frwc.brakeTorque = 10000f;
            flwc.brakeTorque = 10000f;
        }
        else
        {
            frwc.brakeTorque = 0f;
            flwc.brakeTorque = 0f;
        }
    }

    void Update()
    {
        SyncWheelModelAndCollider(flwc, flwm);
        SyncWheelModelAndCollider(frwc, frwm);
        SyncWheelModelAndCollider(blwc, blwm);
        SyncWheelModelAndCollider(brwc, brwm);

        _measuredSpeed = _rb.linearVelocity.magnitude;
        _forwardVelocity = transform.InverseTransformDirection(_rb.linearVelocity).z;

        _speedometer.SetText(Math.Floor(_measuredSpeed * 3.6).ToString() + " km/h");

        if (brwc.brakeTorque > 0 || blwc.brakeTorque > 0 || frwc.brakeTorque > 0 || flwc.brakeTorque > 0)
        {
            _rearLeftLight.intensity = 0.5f;
            _rearRightLight.intensity = 0.5f;
        }
        else
        {
            _rearLeftLight.intensity = 0.1f;
            _rearRightLight.intensity = 0.1f;
        }

        engineSound.pitch = Mathf.Clamp(Math.Abs(_forwardVelocity) / 30f, 0.5f, 2.5f);

        Debug.Log(_averageWheelHitForwardSlip);

        /*
        // FIXME: remove click sound while looping and add start and end sounds
        if (Math.Abs(_averageWheelHitForwardSlip) >= 0.25)
        {
            skidSound.volume = Mathf.Clamp(Math.Abs(_averageWheelHitForwardSlip), 0, 1);
            if (!skidSound.isPlaying)
            {
                skidSound.loop = true;
                skidSound.Play();
            }
        }
        else
        {
            if (skidSound.isPlaying)
            {
                skidSound.loop = false;
            }
        }
        */
    }

    void SyncWheelModelAndCollider(WheelCollider wheelCollider, MeshRenderer meshRenderer)
    {
        wheelCollider.GetWorldPose(out Vector3 position, out Quaternion rotation);
        meshRenderer.transform.rotation = rotation;
        //TODO: position?
    }

    public void OnChangeColor(Color color)
    {
        _bodyMeshRenderer.materials[0].color = color;
    }

}
