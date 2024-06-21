using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Reverse : CarState
{
    public override void Actualizar()
    {
        base.Actualizar();

        if (wheels == null) { return; }

        float forwardSpeed = car.ForwardSpeed();

        float reverseSpeedFactor = Mathf.InverseLerp(0, -car.maxReverseSpeed, forwardSpeed);
        float currentReverseMotorTorque = Mathf.Lerp(-car.reverseMotorTorque, 0, reverseSpeedFactor);

        Debug.Log("speed " + forwardSpeed + " | " + "factor " + reverseSpeedFactor + " | torque " + currentReverseMotorTorque);

        foreach (WheelControl wheel in wheels)
        {
            if (wheel.motorized)
            {
                wheel.WheelCollider.motorTorque = currentReverseMotorTorque;
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
