using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarControl : MonoBehaviour
{
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
        get { 
            if(wheels == null)
            {
                wheels = GetComponentsInChildren<WheelControl>();
            }
            return (WheelControl[])wheels.Clone();
        }
    }

    public Rigidbody rigidBody;

    // Start is called before the first frame update
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();

        // Adjust center of mass vertically, to help prevent the car from rolling
        rigidBody.centerOfMass += Vector3.up * centreOfGravityOffset;

        // Find all child GameObjects that have the WheelControl script attached
        wheels = GetComponentsInChildren<WheelControl>();
    }

    // Update is called once per frame
    void Update()
    {
        /*

        float vInput = movementInput.y;
        float hInput = movementInput.x;

        // Calculate current speed in relation to the forward direction of the car
        // (this returns a negative number when traveling backwards)
        float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.velocity);

        // Calculate how close the car is to top speed
        // as a number from zero to one
        float speedFactor = Mathf.InverseLerp(0, maxSpeed * speedMultiplier, forwardSpeed);
        float reverseSpeedFactor = Mathf.InverseLerp(0, -maxReverseSpeed, forwardSpeed);

        // Use that to calculate how much torque is available 
        // (zero torque at top speed)
        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);
        float currentReverseMotorTorque = Mathf.Lerp(-reverseMotorTorque, 0, reverseSpeedFactor);

        // …and to calculate how much to steer 
        // (the car steers more gently at top speed)
        float currentSteerRange;

        if (reverse)
        {
            currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, reverseSpeedFactor);
        }
        else
        {
            currentSteerRange = Mathf.Lerp(steeringRange, steeringRange, speedFactor);
        }

        // Check whether the user input is in the same direction 
        // as the car's velocity
        //bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);

        Debug.Log(rigidBody.velocity.magnitude);

        foreach (var wheel in wheels)
        {
            // Apply steering to Wheel colliders that have "Steerable" enabled
            if (wheel.steerable)
            {
                wheel.WheelCollider.steerAngle = hInput * currentSteerRange;
            }

            /*

            if(brackes)
            {
                wheel.WheelCollider.brakeTorque = brakeTorque;
                wheel.WheelCollider.motorTorque = 0;
                if(brackes && isReversing)
                {

                }
            }
            else
            {
                wheel.WheelCollider.brakeTorque = 0;

                isReversing = false;

                if (reverse && wheel.motorized)
                {
                    wheel.WheelCollider.motorTorque = currentReverseMotorTorque;
                    isReversing = true;
                }

                if (accelerator && wheel.motorized)
                {
                    wheel.WheelCollider.motorTorque = currentMotorTorque * motorTorqueMultiplier;
                }

                /*
                if (!reverse && !accelerator && wheel.motorized)
                {
                    wheel.WheelCollider.motorTorque = 0;
                }
                */
            // }

            
            //wheel.WheelCollider.brakeTorque = (Reverse) ? brakeTorque : 0;
            /*
            if (accelerator)
            {
                // Apply torque to Wheel colliders that have "Motorized" enabled
                if (wheel.motorized)
                {
                    wheel.WheelCollider.motorTorque = currentMotorTorque;
                }
                wheel.WheelCollider.brakeTorque = 0;
            }
            else
            {
                // If the user is trying to go in the opposite direction
                // apply brakes to all wheels
                wheel.WheelCollider.brakeTorque = brakeTorque;
                wheel.WheelCollider.motorTorque = 0;
            }
            */
        //}
    }

}