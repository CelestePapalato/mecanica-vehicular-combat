using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : StateMachine, IBuffable
{
    [Header("Car Parameters")]
    public float motorTorque = 2000;
    public float reverseMotorTorque = 1900;
    public float brakeTorque = 2000;
    public float maxSpeed = 20;
    public float maxReverseSpeed = 10;
    public float steeringRange = 30;
    public float steeringRangeAtMaxSpeed = 10;
    public float centreOfGravityOffset = -1f;

    private float speedMultiplier = 1f;
    public float SpeedMultiplier
    {
        get => speedMultiplier;
        set => speedMultiplier = (value >= 1) ? value : speedMultiplier;
    }

    private float motorTorqueMultiplier = 1f;
    public float MotorTorqueMultiplier
    {
        get => motorTorqueMultiplier;
        set => motorTorqueMultiplier = (value >= 1) ? value : motorTorqueMultiplier;
    }

    WheelControl[] wheels;
    public WheelControl[] Wheels
    {
        get
        {
            if (wheels == null)
            {
                wheels = GetComponentsInChildren<WheelControl>();
            }
            return (WheelControl[]) wheels.Clone();
        }
    }

    private Rigidbody rigidBody;
    public Rigidbody RigidBody { get => rigidBody; }

    private Vector2 steeringWheelInput = Vector2.zero;
    public Vector2 SteeringWheelInput { get => steeringWheelInput; set => steeringWheelInput = value; }

    protected override void Awake()
    {
        base.Awake();
        
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;
        wheels = GetComponentsInChildren<WheelControl>();
    }

    public override void CambiarEstado(State nuevoEstado)
    {
        base.CambiarEstado(nuevoEstado);
        CarState carState = nuevoEstado as CarState;
        if (carState) { carState.CurrentCar = this; }
    }

    protected override void Update()
    {
        base.Update();

        if (wheels == null || wheels[0].WheelCollider == null)
        {
            return;
        }

        float hInput = steeringWheelInput.x;

        float forwardSpeed = ForwardSpeed();

        float speedFactor = Mathf.InverseLerp(0, maxSpeed * SpeedMultiplier, forwardSpeed);
        float reverseSpeedFactor = Mathf.InverseLerp(0, -maxReverseSpeed, forwardSpeed);

        float currentSteerRange;

        if (forwardSpeed < 0)
        {
            currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, reverseSpeedFactor);
        }
        else
        {
            currentSteerRange = Mathf.Lerp(steeringRange, steeringRange, speedFactor);
        }

        foreach (var wheel in wheels)
        {
            if (wheel.steerable)
            {
                wheel.WheelCollider.steerAngle = hInput * currentSteerRange;
            }

        }
    }

    public float ForwardSpeed()
    {
        return Vector3.Dot(transform.forward, rigidBody.velocity);
    }

    // # ---------------- POWER UP ---------------- #

    public void Accept(IBuff buff)
    {
        buff.Buff(this);
    }
}

public class CarState : State
{
    protected Car car;
    public Car CurrentCar
    {
        get => car;
        set
        {
            if (value == null)
            {
                car = null;
                wheels = null;
                return;
            }

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