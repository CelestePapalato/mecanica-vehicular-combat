using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : StateMachine
{
    [SerializeField]
    private CarControl carControl;
    private WheelControl[] wheels;

    private Vector2 steeringWheelInput = Vector2.zero;
    public Vector2 SteeringWheelInput { get => steeringWheelInput; set => steeringWheelInput = value; }

    protected override void Start()
    {
        if (carControl)
        {
            wheels = carControl.Wheels;
        }
        base.Start();
    }

    public override void CambiarEstado(State nuevoEstado)
    {
        base.CambiarEstado(nuevoEstado);
        CarState carState = estadoActual as CarState;
        if (carState)
        {
            carState.Car = carControl;
        }
    }

    protected override void Update()
    {
        if (carControl)
        {
            float hInput = steeringWheelInput.x;

            float forwardSpeed = ForwardSpeed();

            float speedFactor = Mathf.InverseLerp(0, carControl.maxSpeed * carControl.SpeedMultiplier, forwardSpeed);
            float reverseSpeedFactor = Mathf.InverseLerp(0, -carControl.maxReverseSpeed, forwardSpeed);

            float currentSteerRange;

            if (forwardSpeed < 0)
            {
                currentSteerRange = Mathf.Lerp(carControl.steeringRange, carControl.steeringRangeAtMaxSpeed, reverseSpeedFactor);
            }
            else
            {
                currentSteerRange = Mathf.Lerp(carControl.steeringRange, carControl.steeringRange, speedFactor);
            }

            foreach (var wheel in wheels)
            {
                if (wheel.steerable)
                {
                    wheel.WheelCollider.steerAngle = hInput * currentSteerRange;
                }
            }
        }
        base.Update();
    }

    public float ForwardSpeed()
    {
        return Vector3.Dot(transform.forward, carControl.rigidBody.velocity);
    }
}

public class CarState : State
{
    protected CarControl car;
    public CarControl Car
    {
        get => car;
        set
        {
            if (car != value)
            {
                car = value;
                wheels = car.Wheels;
            }
        }
    }

    protected WheelControl[] wheels;

    public override void Actualizar()
    {
        if (!car) { return; }
        CheckForWheelsExistence();
    }

    public override void ActualizarFixed()
    {
        if (!car) { return; }
        CheckForWheelsExistence();
    }

    protected void CheckForWheelsExistence()
    {
        if (wheels == null)
        {
            wheels = car?.Wheels;
        }
    }
}