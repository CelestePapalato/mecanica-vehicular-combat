using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brake : CarState
{
    [SerializeField] float torqueSpeedOverflow;
    [SerializeField] float speedOverflowReference;
    public override void Actualizar()
    {
        base.Actualizar();
        ApplyBrake();
    }

    public override void Salir()
    {
        base.Salir();
        if(wheels == null) { return; }
        foreach (WheelControl wheel in wheels)
        {
            if (wheel.WheelCollider == null)
            {
                break;
            }
            wheel.WheelCollider.brakeTorque = 0;
            wheel.WheelCollider.motorTorque = 0;
        }
    }

    private void ApplyBrake()
    {
        if (wheels == null) { return; }

        float speed = car.ForwardSpeed();
        float direction = - Mathf.Sign(speed);
        speed = Mathf.Abs(speed);
        float motorTorque = (speed > speedOverflowReference)? motorTorque = torqueSpeedOverflow * direction : 0;

        foreach (WheelControl wheel in wheels)
        {
            if(wheel.WheelCollider == null)
            {
                break;
            }
            WheelCollider wheelCollider = wheel.WheelCollider;
            if (wheel.steerable)
            {
                wheelCollider.brakeTorque = Mathf.Max(car.brakeTorque * wheelCollider.rpm, car.brakeTorque);
            }
            else
            {
                wheelCollider.brakeTorque = Mathf.Max(car.brakeTorque * .45f * wheelCollider.rpm, car.brakeTorque * .45f);
            }
            wheelCollider.motorTorque = motorTorque;
        }
    }
}
