using System;
using TMPro;
using UnityEngine;

public class Car : MonoBehaviour
{

    // f: Front, l: Left, r: Right, b: Back, c: Collider, m: Mesh
    public WheelCollider frwc, flwc, brwc, blwc;
    public MeshRenderer frwm, flwm, brwm, blwm;

    [SerializeField] private TextMeshPro _speedometer;

    public Rigidbody rb;
    private float _forward = 0;
    private float _rpm = 1000f;

    private float _steer = 0;

    private WheelHit[] _wheelHits = new WheelHit[4];
    private float _averageWheelHitForwardSlip = 0f;


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

        frwc.motorTorque = _rpm * _forward;
        flwc.motorTorque = _rpm * _forward;

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

        if (Math.Abs(_averageWheelHitForwardSlip) < 0.45 && Math.Abs(_forward) > 0.75)
        {
            if (Math.Abs(_steer) < 0.3f)
            {
                _rpm += 10;
            }
            else
            {
                _rpm += 5;
            }
        }
        if (Math.Abs(_averageWheelHitForwardSlip) >= 0.45)
        {
            if (_rpm > 100)
            {
                _rpm -= 10;
            }
            else
            {
                _rpm -= 5;
            }
        }


    }

    void Update()
    {
        SyncWheelModelAndCollider(flwc, flwm);
        SyncWheelModelAndCollider(frwc, frwm);
        SyncWheelModelAndCollider(blwc, blwm);
        SyncWheelModelAndCollider(brwc, brwm);
        _speedometer.text = Math.Floor(rb.linearVelocity.magnitude).ToString() + " km/h";
    }

    void SyncWheelModelAndCollider(WheelCollider wheelCollider, MeshRenderer meshRenderer)
    {
        wheelCollider.GetWorldPose(out Vector3 position, out Quaternion rotation);
        meshRenderer.transform.rotation = rotation;
        //TODO: position?
    }

}
