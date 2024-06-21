using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brake : CarState
{
    bool wheelsNull = false;

    public override void Entrar(StateMachine personajeActual)
    {
        base.Entrar(personajeActual);

        if (wheels == null)
        {
            wheelsNull = true;
            return;
        }

        foreach (WheelControl wheel in wheels)
        {
            wheel.WheelCollider.brakeTorque = car.brakeTorque;
            wheel.WheelCollider.motorTorque = 0;
        }
    }

    public override void Actualizar()
    {
        if(!wheelsNull) { return; }
        base.Actualizar();
        if(wheels == null) { return; }

        foreach (WheelControl wheel in wheels)
        {
            wheel.WheelCollider.brakeTorque = car.brakeTorque;
            wheel.WheelCollider.motorTorque = 0;
        }
    }

    public override void Salir()
    {
        base.Salir();
        if(wheels == null) { return; }
        foreach (WheelControl wheel in wheels)
        {
            wheel.WheelCollider.brakeTorque = 0;
        }
    }
}
