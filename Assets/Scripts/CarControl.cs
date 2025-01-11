using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.InputSystem.Utilities;

public class CarControl : MonoBehaviour
{

    // f: Front, l: Left, r: Right, b: Back, c: Collider, m: Mesh
    public WheelCollider frwc, flwc, brwc, blwc;
    public MeshRenderer frwm, flwm, brwm, blwm;

    [SerializeField] private TMP_Text _speedometer;
    [SerializeField] private TMP_Text _rpmText;

    [SerializeField] private TMP_Text _scoreText;

    [SerializeField] private Rigidbody _rb;

    [SerializeField] InputActionReference _forwardAction;
    [SerializeField] InputActionReference _steerAction;
    [SerializeField] InputActionReference _handbrakeAction;
    Vector3 _acceleration = new Vector3(0, 0, 0);
    bool _hasAcceleration = false;
    private float _forward = 0;
    private float _rpm = 0f;

    private float _measuredSpeed = 0f;

    private float _steer = 0;

    private WheelHit[] _wheelHits = new WheelHit[4];
    private float _averageWheelHitForwardSlip = 0f;

    public Light rearLeftLight, rearRightLight;

    public AudioSource engineSound;
    public AudioSource skidSound;

    private int _score = 0;

    public int Score
    {
        get
        {
            return _score;
        }
        set
        {
            _score = value;
            _scoreText.SetText(_score.ToString() + " Star" + (_score == 1 ? "" : "s"));
        }
    }


    void Start()
    {
        _hasAcceleration = SystemInfo.supportsAccelerometer;
        Debug.Log("Accelerometer supported: " + _hasAcceleration);
        InputSystem.EnableDevice(Accelerometer.current);
    }

    void FixedUpdate()
    {
        if (_hasAcceleration)
        {
            _acceleration = Accelerometer.current.acceleration.ReadValue();
        }

        //Debug.Log("Acceleration: " + _acceleration);

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
            _steer = steerActionValue * 0.3f;
        }

        //Debug.Log("forward: " + forwardAction.ReadValue<float>() + "steer: " + steerAction.ReadValue<float>() + "handbrake: " + handbrakeAction.ReadValue<float>());

        frwc.motorTorque = _rpm;
        flwc.motorTorque = _rpm;

        frwc.steerAngle = 80f * _steer;
        flwc.steerAngle = 80f * _steer;



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



        // Player tries to go with forward gear
        if (_rpm < 4000 && _forward > 0)
        {
            _rpm += 10 * (1 - Math.Abs(_averageWheelHitForwardSlip)) * _forward;
        }

        // Player tries to go with neutral gear
        else if (_forward == 0 & _rpm != 0)
        {
            if (_rpm > 0)
            {
                _rpm -= 10;
            }
            else if (_rpm < 0)
            {
                _rpm += 10;
            }
        }

        // reverse gear while going forward
        else if (_rpm > 0 && _forward <= 0)
        {
            _rpm += 500 * (1 - Math.Abs(_averageWheelHitForwardSlip)) * _forward;
        }
        // reverse gear while going backward
        else if (_rpm < 0 && _forward <= 0 && _rpm > -1000)
        {
            _rpm += 7 * (1 - Math.Abs(_averageWheelHitForwardSlip)) * _forward;
        }

        if ((_forward < 0 && _rpm > 0 || _forward > 0 && _rpm < 0) && Math.Abs(_measuredSpeed) > 5f)
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

        _measuredSpeed = Math.Abs(_rb.linearVelocity.magnitude);

        _speedometer.SetText(Math.Floor(_measuredSpeed * 3.6).ToString() + " km/h");
        _rpmText.SetText(Math.Floor(_rpm).ToString() + " RPM");

        if (brwc.brakeTorque > 0 || blwc.brakeTorque > 0)
        {
            rearLeftLight.intensity = 0.5f;
            rearRightLight.intensity = 0.5f;
        }
        else
        {
            rearLeftLight.intensity = 0.1f;
            rearRightLight.intensity = 0.1f;
        }

        //engineSound.pitch = Mathf.Clamp(rb.linearVelocity.magnitude / 10, 0.5f, 2.5f);
        engineSound.pitch = Mathf.Clamp(Math.Abs(_rpm) / 1000, 0.5f, 2.5f);

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

}
