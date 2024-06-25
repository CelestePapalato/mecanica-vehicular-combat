using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageModifier : MonoBehaviour
{
    public Car car;
    private Damage damage;

    [SerializeField]
    [Tooltip("Velocidad necesaria para un punto de daño.")]
    private float minSpeed;
    [SerializeField]
    [Tooltip("Velocidad para el daño máximo")]
    private float maxSpeed;
    [SerializeField]
    private int minDamage = 1;
    [SerializeField]
    private int maxDamage = 4;
    [SerializeField]
    private float tolerance = .4f;

    private void Start()
    {
        damage = GetComponent<Damage>();
    }

    private void Update()
    {
        if(!car) { return; }
        float currentSpeed = Mathf.Abs(car.ForwardSpeed());
        if(Mathf.Abs(currentSpeed-minSpeed) <= tolerance) {
            currentSpeed = Mathf.Ceil(currentSpeed);
        }
        float modifier = 0;
        if(currentSpeed >= minSpeed)
        {
            float t = Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed);
            modifier = Mathf.Lerp(minDamage, maxDamage, t);
        }
        damage.DamageMultiplier = modifier;
    }

}
