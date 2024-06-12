using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeAlive : MonoBehaviour
{
    [SerializeField] float timeAlive;

    private void Start()
    {
        Destroy(gameObject, timeAlive);
    }
}