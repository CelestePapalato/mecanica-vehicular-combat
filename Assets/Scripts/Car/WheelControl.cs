using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelControl : MonoBehaviour
{
    [SerializeField] Transform wheelTransform;
    [SerializeField] bool steerable = false;

    public bool Steerable { get => steerable; }

    public float SteerAngle { get => wheelCollider.steerAngle; set => wheelCollider.steerAngle = value; }
    public float MotorTorque { get => wheelCollider.motorTorque; set => wheelCollider.motorTorque = value; }
    public float BrakeTorque { get => wheelCollider.brakeTorque; set => wheelCollider.brakeTorque = value; }

    private WheelCollider wheelCollider;

    private void Awake()
    {
        wheelCollider = GetComponent<WheelCollider>();
    }

    private void Update()
    {
        Vector3 position;
        Quaternion rotation;
        wheelCollider.GetWorldPose(out position, out rotation);
        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }
}
