using UnityEngine;

public interface IDamageDealer
{
    public int DamagePoints { get; }
    public float Impulse { get; }
    public Vector3 Position { get; }

}