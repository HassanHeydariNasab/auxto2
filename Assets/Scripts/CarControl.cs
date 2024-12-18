using System;
using TMPro;
using UnityEngine;

public class CarControl : MonoBehaviour
{

    // f: Front, l: Left, r: Right, b: Back, c: Collider, m: Mesh
    public WheelCollider frwc, flwc, brwc, blwc;
    public MeshRenderer frwm, flwm, brwm, blwm;

    [SerializeField] private TMP_Text _speedometer;
    [SerializeField] private TMP_Text _rpmText;

    [SerializeField] private TMP_Text _scoreText;

    public Rigidbody rb;
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
    }

    void FixedUpdate()
    {

        _forward = Input.acceleration.y == 0 ? 0 : Input.acceleration.y + 0.5f;
        _steer = Input.acceleration.x == 0 ? 0 : Input.acceleration.x;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            _forward = 1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            _forward = -1f;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _steer = -0.3f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            _steer = 0.3f;
        }

        frwc.motorTorque = _rpm;
        flwc.motorTorque = _rpm;

        frwc.steerAngle = 80f * _steer;
        flwc.steerAngle = 80f * _steer;



        if ((Input.touchSupported && Input.touchCount > 0) || Input.GetKey(KeyCode.Space))
        {
            brwc.brakeTorque = 10000f;
            blwc.brakeTorque = 10000f;
        }
        else if ((!Input.touchSupported) || (Input.touchCount == 0))
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


        if (_rpm < 4000 && _rpm > -1000 && Math.Abs(_forward) >= 0.25)
        {
            _rpm += 7 * (1 - Math.Abs(_averageWheelHitForwardSlip)) * _forward;// * (_measuredSpeed + 10) / 10;
        }
        else if (Math.Abs(_forward) < 0.25)
        {

            if (
                _rpm > 0
            )
            {
                _rpm -= 10;
            }
            else
            {
                _rpm += 10;
            }
        }
    }

    void Update()
    {
        SyncWheelModelAndCollider(flwc, flwm);
        SyncWheelModelAndCollider(frwc, frwm);
        SyncWheelModelAndCollider(blwc, blwm);
        SyncWheelModelAndCollider(brwc, brwm);

        _measuredSpeed = Math.Abs(rb.linearVelocity.magnitude);

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
        engineSound.pitch = Mathf.Clamp(_rpm / 1000, 0.5f, 2.5f);

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
