using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accelerator : CarState
{
    [SerializeField] float torqueSpeedOverflow;
    [SerializeField] float speedOverflowReference;

    public override void Actualizar()
    {
        base.Actualizar();

        if (wheels == null) { return; }

        float forwardSpeed = car.ForwardSpeed();


        float currentMotorTorque;
        float currentMaxSpeed = car.maxSpeed * car.SpeedMultiplier;

        if (forwardSpeed > currentMaxSpeed)
        {
            Debug.Log("uwu");
            float overflowFactor = Mathf.InverseLerp(currentMaxSpeed, speedOverflowReference, forwardSpeed);
            currentMotorTorque = Mathf.Lerp(0, -torqueSpeedOverflow, overflowFactor);
        }
        else
        {
            float speedFactor = Mathf.InverseLerp(0, currentMaxSpeed, forwardSpeed);
            currentMotorTorque = Mathf.Lerp(car.motorTorque * car.MotorTorqueMultiplier, 0, speedFactor);
        }

        foreach (WheelControl wheel in wheels)
        {
            if (wheel.motorized)
            {
                wheel.WheelCollider.motorTorque = currentMotorTorque;
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
