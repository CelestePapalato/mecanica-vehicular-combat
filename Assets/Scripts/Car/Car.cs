using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : StateMachine
{
    public static event Action<Car> onDestroy;

    [Header("Car Parameters")]
    public float motorTorque = 2000;
    public float reverseMotorTorque = 1900;
    public float brakeTorque = 2000;
    public float maxSpeed = 20;
    public float maxReverseSpeed = 10;
    public float steeringRange = 30;
    public float steeringRangeAtMaxSpeed = 10;
    public float steeringRangeAtMaxReverseSpeed = 20;
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

    private Health health;

    protected override void Awake()
    {
        base.Awake();
        
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;
        wheels = GetComponentsInChildren<WheelControl>();
        health = GetComponentInChildren<Health>();
    }

    private void OnEnable()
    {
        if (health)
        {
            health.onDamage += DamageReceived;
        }
    }

    private void OnDisable()
    {
        if (health)
        {
            health.onDamage -= DamageReceived;
        }
    }

    private void DamageReceived(int health, int maxHealth)
    {
        if(health == 0)
        {
            onDestroy?.Invoke(this);
        }
    }

    // # ---------------- STATE MACHINE ---------------- #

    public override void CambiarEstado(State nuevoEstado)
    {
        base.CambiarEstado(nuevoEstado);
        CarState carState = nuevoEstado as CarState;
        if (carState) { carState.CurrentCar = this; }
    }

    protected override void Update()
    {
        base.Update();

        if (wheels == null)
        {
            return;
        }
        if(wheels[0].WheelCollider == null)
        {
            return;
        }

        float hInput = steeringWheelInput.x;

        float forwardSpeed = ForwardSpeed();

        float currentSteerRange;

        if (forwardSpeed < 0)
        {
            float reverseSpeedFactor = Mathf.InverseLerp(0, -maxReverseSpeed, forwardSpeed);
            currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxReverseSpeed, reverseSpeedFactor);
        }
        else
        {
            float speedFactor = Mathf.InverseLerp(0, maxSpeed * SpeedMultiplier, forwardSpeed);
            currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);
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