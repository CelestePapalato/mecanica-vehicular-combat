using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] State Accelerator;
    [SerializeField] State Reverse;
    [SerializeField] State Brake;

    [SerializeField]
    Transform weaponPivot;

    [Header("Camera Parameters")]
    [SerializeField] float sensitivy;
    [SerializeField]
    [Range(0f, 0.3f)] float smoothing;
    [Header("Angles")]
    [SerializeField]
    [Range(-90, 90)] float upperLookLimit;
    [SerializeField]
    [Range(-90, 90)] float lowerLookLimit;


    Car carStateMachine;

    float x_AxisRotation = 0f;

    Vector2 delta = Vector2.zero;
    Vector2 cameraInput = Vector2.zero;

    Camera mainCamera;

    bool acceleratorInput = false;
    bool reverseInput = false;

    bool isReversing = false;

    private void Awake()
    {
        carStateMachine = GetComponent<Car>();
        x_AxisRotation = weaponPivot.localEulerAngles.x;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if(!carStateMachine) { return; }
        Debug.Log(carStateMachine.ForwardSpeed());
        StopBraking();
        //RotateGunPivot();
    }

    private void RotateGunPivot()
    {
        delta.x = Mathf.SmoothStep(delta.x, cameraInput.x * sensitivy * Time.deltaTime, smoothing);
        delta.y = Mathf.SmoothStep(delta.y, cameraInput.y * sensitivy * Time.deltaTime, smoothing);

        x_AxisRotation += delta.y;
        x_AxisRotation = Mathf.Clamp(x_AxisRotation, lowerLookLimit, upperLookLimit);

        weaponPivot.localRotation = Quaternion.Euler(x_AxisRotation, delta.x + weaponPivot.localEulerAngles.y, 0);
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
        if(!acceleratorInput && reverseInput)
        {
            isReversing = false;
            carStateMachine.CambiarEstado(Brake);
            return;
        }
        if (acceleratorInput)
        {
            isReversing = false;
            if (!carStateMachine) { return; }
            carStateMachine?.CambiarEstado(Accelerator);
        }
    }

    private void OnReverse(InputValue inputValue)
    {
        reverseInput = !reverseInput;
        if(acceleratorInput && reverseInput)
        {
            isReversing = false;
            return;
        }
        if(!acceleratorInput && !reverseInput)
        {
            isReversing = false;
            carStateMachine?.CambiarEstado(Brake);
            return;
        }

        if (carStateMachine?.ForwardSpeed() < -.1f)
        {
            isReversing = true;
            carStateMachine?.CambiarEstado(Reverse);
        }
        else
        {
            isReversing = false;
            carStateMachine?.CambiarEstado(Brake);
        }
    }

    private void StopBraking()
    {
        if(!(reverseInput && !isReversing))
        {
            return;
        }
        if(Mathf.Abs(carStateMachine.ForwardSpeed()) < .1f) {
            isReversing = true;
            carStateMachine?.CambiarEstado(Reverse);
        }
    }
}
