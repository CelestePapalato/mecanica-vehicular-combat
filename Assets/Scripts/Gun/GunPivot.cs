using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPivot : MonoBehaviour
{
    PlayerCursor playerCursor;

    float y_value;

    private void Awake()
    {
        playerCursor = GameObject.FindGameObjectWithTag("Player Cursor").GetComponent<PlayerCursor>();
        y_value = transform.position.y;
    }

    private void Update()
    {
        if (!playerCursor) { return; }
        Vector3 target = playerCursor.GetWorldPoint();
        target.y = y_value;
        transform.LookAt(target);
    }
}