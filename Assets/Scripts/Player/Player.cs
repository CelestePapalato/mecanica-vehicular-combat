using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

public class Player : MonoBehaviour, IBuffable
{  
    public static event Action<Player> onDead;

    [Header("Input")]
    [SerializeField] State Accelerator;
    [SerializeField] State Reverse;
    [SerializeField] State Brake;

    [SerializeField]
    Transform cameraPivot;

    [Header("Camera Parameters")]
    [SerializeField] float sensitivy;
    [SerializeField]
    [Range(0f, 0.3f)] float smoothing;
    [Header("Angles")]
    [SerializeField]
    [Range(-90, 90)] float upperLookLimit;
    [SerializeField]
    [Range(-90, 90)] float lowerLookLimit;

    [Header("UI")]
    [SerializeField] TMP_Text speedText;

    Car carStateMachine;

    float x_AxisRotation = 0f;

    Vector2 delta = Vector2.zero;
    Vector2 cameraInput = Vector2.zero;

    bool acceleratorInput = false;
    bool reverseInput = false;

    bool isAccelerating = false;

    Health health;
    Damage damage;
    DamageModifier damageModifier;

    private void Awake()
    {
        carStateMachine = GetComponentInChildren<Car>();
        health = GetComponentInChildren<Health>();
        health.onDamage += Damaged;
        damage = GetComponentInChildren<Damage>();
        damageModifier = GetComponentInChildren<DamageModifier>();
        damageModifier.car = carStateMachine;
        x_AxisRotation = cameraPivot.localEulerAngles.x;
    }

    private void Update()
    {
        RotateCameraPivot();
        if (!carStateMachine) { return; }
        ControlBraking();
    }

    private void LateUpdate()
    {
        UpdateSpeedUI();
    }

    // # ---------------- EVENTS ---------------- #

    private void Damaged(int current, int maxHealth)
    {
        if(current == 0) {
            onDead?.Invoke(this);
            Destroy(gameObject);
        }
    }

    // # ---------------- INPUT ---------------- #

    private void RotateCameraPivot()
    {
        delta.x = Mathf.SmoothStep(delta.x, cameraInput.x * sensitivy * Time.deltaTime, smoothing);
        delta.y = Mathf.SmoothStep(delta.y, cameraInput.y * sensitivy * Time.deltaTime, smoothing);

        x_AxisRotation += delta.y;
        x_AxisRotation = Mathf.Clamp(x_AxisRotation, lowerLookLimit, upperLookLimit);

        cameraPivot.localRotation = Quaternion.Euler(x_AxisRotation, delta.x + cameraPivot.localEulerAngles.y, 0);
    }

    private void OnMove(InputValue inputValue){
        Vector2 input = inputValue.Get<Vector2>();
        //input = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0) * input;
        carStateMachine.SteeringWheelInput = input;
    }

    private void OnCamera(InputValue inputValue)
    {
        Vector2 input = inputValue.Get<Vector2>();
        cameraInput = inputValue.Get<Vector2>();
    }

    private void OnAccelerator(InputValue inputValue)
    {
        acceleratorInput = !acceleratorInput;
        if (!acceleratorInput && !reverseInput)
        {
            carStateMachine?.CambiarEstado(Brake);
            return;
        }

        if (!acceleratorInput && reverseInput)
        {
            reverseInput = !reverseInput;
            OnReverse(inputValue);
            return;
        }

        if (carStateMachine.ForwardSpeed() > .1f)
        {
            isAccelerating = false;
            carStateMachine?.CambiarEstado(Accelerator);
        }
        else
        {
            isAccelerating = true;
            carStateMachine?.CambiarEstado(Brake);
        }
    }

    private void OnReverse(InputValue inputValue)
    {
        reverseInput = !reverseInput;
        if(acceleratorInput && reverseInput)
        {
            return;
        }
        if(acceleratorInput && !reverseInput)
        {
            acceleratorInput = !acceleratorInput;
            OnAccelerator(inputValue);
            return;
        }

        if(!acceleratorInput && !reverseInput)
        {
            isAccelerating = false;
            carStateMachine?.CambiarEstado(Brake);
            return;
        }

        if (carStateMachine?.ForwardSpeed() < -.1f)
        {
            isAccelerating = false;
            carStateMachine?.CambiarEstado(Reverse);
        }
        else
        {
            isAccelerating = true;
            carStateMachine?.CambiarEstado(Brake);
        }
    }

    private void ControlBraking()
    {
        if (!isAccelerating)
        {
            return;
        }
        if (Mathf.Abs(carStateMachine.ForwardSpeed()) < .1f)
        {
            if (reverseInput && !acceleratorInput)
            {
                isAccelerating = false;
                carStateMachine?.CambiarEstado(Reverse);
            }

            if (acceleratorInput)
            {
                isAccelerating = false;
                carStateMachine?.CambiarEstado(Accelerator);

            }
        }
    }

    // # ---------------- POWER UP ---------------- #

    public void Accept(IBuff buff)
    {
        //buff.Buff(carStateMachine);
    }

    // # ----------------- UI ---------------- #

    private void UpdateSpeedUI()
    {
        if (speedText)
        {
            speedText.text = (Mathf.Abs(carStateMachine.ForwardSpeed())) + "";
        }
    }
}
