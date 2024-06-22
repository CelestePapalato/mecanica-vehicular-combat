using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Reverse : CarState
{
    [SerializeField] float torqueSpeedOverflow;
    [SerializeField] float speedOverflowReference;

    public override void Actualizar()
    {
        base.Actualizar();

        if (wheels == null) { return; }

        float forwardSpeed = car.ForwardSpeed();

        float currentReverseMotorTorque;

        if (forwardSpeed < -car.maxReverseSpeed)
        {

            float overflowFactor = Mathf.InverseLerp(-car.maxReverseSpeed, -speedOverflowReference, forwardSpeed);
            currentReverseMotorTorque = Mathf.Lerp(0, torqueSpeedOverflow, overflowFactor);
        }
        else
        {
            float reverseSpeedFactor = Mathf.InverseLerp(0, -car.maxReverseSpeed, forwardSpeed);
            currentReverseMotorTorque = Mathf.Lerp(-car.reverseMotorTorque, 0, reverseSpeedFactor);
        }

        foreach (WheelControl wheel in wheels)
        {
            if (wheel.motorized)
            {
                wheel.WheelCollider.motorTorque = currentReverseMotorTorque;
            }
            wheel.WheelCollider.brakeTorque = 0;
        }
    }

    public override void Salir()
    {
        base.Salir();
        if (wheels == null) { return; };
        foreach (WheelControl wheel in wheels)
        {
            wheel.WheelCollider.motorTorque = 0;
        }
    }
}
