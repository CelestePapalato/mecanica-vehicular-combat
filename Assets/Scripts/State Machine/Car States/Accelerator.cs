using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accelerator : CarState
{

    public override void Actualizar()
    {
        base.Actualizar();

        if (wheels == null) { return; }

        float forwardSpeed = Vector3.Dot(car.transform.forward, car.rigidBody.velocity);

        float speedFactor = Mathf.InverseLerp(0, car.maxSpeed * car.SpeedMultiplier, forwardSpeed);
        float currentMotorTorque = Mathf.Lerp(car.motorTorque * car.MotorTorqueMultiplier, 0, speedFactor);

        foreach (WheelControl wheel in wheels)
        {
            if (wheel.motorized)
            {
                wheel.WheelCollider.motorTorque = currentMotorTorque;
            }
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
