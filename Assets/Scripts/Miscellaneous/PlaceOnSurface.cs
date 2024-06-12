using UnityEngine;

public class PlaceOnSurface : MonoBehaviour
{
    [SerializeField] float raycastDistance;
    [SerializeField] LayerMask _floorLayerMask;

    void Start()
    {
        placeOnSurface();
    }

    void placeOnSurface()
    {
        RaycastHit hit;
        float heightAddition = transform.localScale.y / 2;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, _floorLayerMask))
        {
            Vector3 position = hit.point;
            position.y += heightAddition;
            transform.position = position;
            return;
        }
    }
}