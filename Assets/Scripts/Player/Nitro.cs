using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Nitro : MonoBehaviour, IBuffable
{
    public UnityAction<int, int> onNitroUpdated;

    [SerializeField] int nitroGaugeMax;
    [SerializeField] float nitroSubstractionRate;
    [SerializeField] float nitroSpeedMultiplier;
    [SerializeField] float nitroMotorTorqueMultiplier;

    int currentNitro = 0;

    public int NitroGaugeMax { get => nitroGaugeMax; }

    bool nitroInput = false;

    Car car;

    private void Start()
    {
        car = GetComponentInChildren<Car>();
        onNitroUpdated?.Invoke(currentNitro, nitroGaugeMax);
    }

    public void AddNitro(int nitroPoints)
    {
        nitroPoints = Mathf.Abs(nitroPoints);
        currentNitro = Mathf.Min(currentNitro + nitroPoints, nitroGaugeMax);
        onNitroUpdated?.Invoke(currentNitro, nitroGaugeMax);
    }

    public void ActivateNitro()
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
            onNitroUpdated?.Invoke(currentNitro, nitroGaugeMax);
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
