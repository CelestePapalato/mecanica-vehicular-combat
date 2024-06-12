using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damage : MonoBehaviour, IDamageDealer
{
    [SerializeField] int damagePoints;
    [SerializeField] float impulse;

    public UnityAction DamageDealed;

    public int DamagePoints { get { return (int)(damagePoints * damageMultiplier); } }

    private float damageMultiplier = 1f;
    public float DamageMultiplier { get => damageMultiplier; set => damageMultiplier = value; }

    public float Impulse { get { return impulse; } }
    public Vector3 Position { get { return transform.position; } }

    private void OnTriggerEnter(Collider other)
    {
        bool damageDealed = false;
        IDamageable enemyDamageable;
        IHittable enemyHittable;
        if (other.TryGetComponent<IDamageable>(out enemyDamageable))
        {
            enemyDamageable.Damage(this);
            damageDealed = true;
        }
        if (other.TryGetComponent<IHittable>(out enemyHittable))
        {
            enemyHittable.Hit(this);
            damageDealed = true;
        }
        if (damageDealed && DamageDealed != null)
        {
            DamageDealed();
        }
    }

}