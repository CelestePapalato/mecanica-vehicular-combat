using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] float minSpawnTime;
    [SerializeField] float maxSpawnTime;

    [SerializeField] Item[] items;

    Item currentItem;

    private void Awake()
    {
        if(items.Length == 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        if (currentItem != null)
        {
            currentItem.onItemPicked += SpawnItem;
        }
    }

    private void OnDisable()
    {
        if(currentItem != null)
        {
            currentItem.onItemPicked -= SpawnItem;
        }
    }

    private void Start()
    {
        currentItem = GetComponentInChildren<Item>();
        if(currentItem == null)
        {
            SpawnItem();
        }
        else
        {
            currentItem.onItemPicked += SpawnItem;
        }
    }

    private void SpawnItem()
    {
        float t = Random.Range(minSpawnTime, maxSpawnTime);
        Invoke(nameof(InstanceItem), t);
    }

    private void InstanceItem()
    {
        if (currentItem != null)
        {
            currentItem.onItemPicked -= SpawnItem;
        }
        int r = Random.Range(0, items.Length);
        Item item = items[r];
        currentItem = Instantiate(item, transform);
        currentItem.onItemPicked += SpawnItem;
    }

}
