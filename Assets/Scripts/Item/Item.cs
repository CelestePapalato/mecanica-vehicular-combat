using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] PowerUp powerUp;

    private void OnTriggerEnter(Collider collider)
    {
        IBuffable[] buffable = collider.gameObject.GetComponentsInChildren<IBuffable>();

        if (buffable.Length == 0)
        {
            return;
        }

        foreach(IBuffable b in buffable) {
            b.Accept(powerUp);
        }

        Destroy(gameObject);
    }
}
