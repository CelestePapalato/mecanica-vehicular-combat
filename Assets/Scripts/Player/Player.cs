using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using System.Linq;

public class Player : MonoBehaviour
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

    PlayerInput playerInput;

    bool enableInput = false;

    Nitro nitro;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        carStateMachine = GetComponentInChildren<Car>();
        nitro = carStateMachine.GetComponent<Nitro>();
        health = GetComponentInChildren<Health>();
        damage = GetComponentInChildren<Damage>();
        damageModifier = GetComponentInChildren<DamageModifier>();
        damageModifier.car = carStateMachine;
        Healthbar healthbar = GetComponentInChildren<Healthbar>();
        healthbar.healthComponent = health;
        x_AxisRotation = cameraPivot.localEulerAngles.x;
    }

    private void OnEnable()
    {
        if (health)
        {
            health.onDamage += Damaged;
        }
        GameManager.onGameStarted += EnableInput;
    }

    private void OnDisable()
    {
        if (health)
        {
            health.onDamage -= Damaged;
        }
        GameManager.onGameStarted -= EnableInput;
    }

    private void FixedUpdate()
    {
        RotateCameraPivot();
    }

    private void Update()
    {        
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
            DisableInput();
        }
    }

    // # ---------------- INPUT ---------------- #

    private void EnableInput()
    {
        enableInput = true;
    }

    private void DisableInput()
    {
        enableInput = false;
        isAccelerating = false;
        reverseInput = false;
        acceleratorInput = false;
        carStateMachine.CambiarEstado(Brake);
    }

    private void RotateCameraPivot()
    {
        delta.x = Mathf.SmoothStep(delta.x, cameraInput.x * sensitivy * Time.deltaTime, smoothing);
        delta.y = Mathf.SmoothStep(delta.y, cameraInput.y * sensitivy * Time.deltaTime, smoothing);

        x_AxisRotation += delta.y;
        x_AxisRotation = Mathf.Clamp(x_AxisRotation, lowerLookLimit, upperLookLimit);

        cameraPivot.localRotation = Quaternion.Euler(x_AxisRotation, delta.x + cameraPivot.localEulerAngles.y, 0);
    }

    private void OnMove(InputValue inputValue){
        if (!enableInput)
        {
            return;
        }
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

        if (!acceleratorInput && reverseInput && enableInput)
        {
            reverseInput = !reverseInput;
            OnReverse(inputValue);
            return;
        }

        if(!enableInput)
        {
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
        if(acceleratorInput && !reverseInput && enableInput)
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

        if (!enableInput)
        {
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

    private void OnNitro()
    {
        if (!enableInput)
        {
            return;
        }
        nitro?.ActivateNitro();
    }

    // # ----------------- UI ---------------- #

    private void UpdateSpeedUI()
    {
        if (speedText)
        {
            speedText.text = (Mathf.Abs(carStateMachine.ForwardSpeed())).ToString("F2");
        }
    }
}
