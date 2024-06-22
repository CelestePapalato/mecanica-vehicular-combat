using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Power Up Data", menuName = "ScriptableObjects/Power Up Data", order = 2)]

public class PowerUp : ScriptableObject, IBuff
{
    [SerializeField] float _buffTime;
    [Header("Healing")]
    [SerializeField] int _healPoints;
    [Header("Nitro")]
    [SerializeField] int nitroPoints;
    [Header("Movement")]
    [SerializeField] float _speedMultiplier;

    public void Buff(object o)
    {
        Health healthComponent = o as Health;
        if (healthComponent)
        {
            healthComponent.Heal(_healPoints);
        }
        Player player = o as Player;
        if (player)
        {
            //player.SpeedPowerUp(_speedMultiplier, _buffTime);
            //player.DamagePowerUp(_damageMultiplier, _buffTime);
        }
        Nitro nitro = o as Nitro;
        if (nitro)
        {
            nitro.AddNitro(nitroPoints);
        }
    }
}
