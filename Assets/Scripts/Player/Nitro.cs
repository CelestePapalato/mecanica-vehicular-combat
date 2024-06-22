using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nitro : MonoBehaviour, IBuffable
{
    [SerializeField] int nitroGaugeMax;
    [SerializeField] float nitroSubstractionRate;
    [SerializeField] float nitroSpeedMultiplier;
    [SerializeField] float nitroMotorTorqueMultiplier;

    int currentNitro = 0;

    bool nitroInput = false;

    Car car;

    private void Start()
    {
        car = GetComponentInChildren<Car>();
    }

    public void AddNitro(int nitroPoints)
    {
        nitroPoints = Mathf.Abs(nitroPoints);
        currentNitro = Mathf.Min(currentNitro + nitroPoints, nitroGaugeMax);
        Debug.Log("Nitro : " + currentNitro + " / " + nitroGaugeMax);
    }

    private void OnNitro()
    {
        StopAllCoroutines();
        nitroInput = !nitroInput;
        StartCoroutine(ControlNitro());
    }

    IEnumerator ControlNitro()
    {
        Debug.Log("Nitro ON");
        car.SpeedMultiplier = nitroSpeedMultiplier;
        car.MotorTorqueMultiplier = nitroMotorTorqueMultiplier;
        while (nitroInput && currentNitro > 0)
        {
            yield return new WaitForSeconds(nitroSubstractionRate);
            currentNitro--;
        }
        car.SpeedMultiplier = 1;
        car.MotorTorqueMultiplier = 1;
        Debug.Log("Nitro OFF");
    }

    public void Accept(IBuff buff)
    {
        buff?.Buff(this);
    }
}
