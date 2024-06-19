using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
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


    CarControl carControl;

    float x_AxisRotation = 0f;

    Vector2 delta = Vector2.zero;
    Vector2 cameraInput = Vector2.zero;

    Camera mainCamera;

    private void Awake()
    {
        carControl = GetComponentInChildren<CarControl>();
        x_AxisRotation = weaponPivot.localEulerAngles.x;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        RotateGunPivot();
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
        carControl.MovementInput = input;
    }

    private void OnCamera(InputValue inputValue)
    {
        Vector2 input = inputValue.Get<Vector2>();
        cameraInput = inputValue.Get<Vector2>();
    }

    private void OnAccelerator(InputValue inputValue)
    {
        carControl.Accelerator = !carControl.Accelerator;
    }

    private void OnReverse(InputValue inputValue)
    {
        carControl.Reverse = !carControl.Reverse;
    }
}
