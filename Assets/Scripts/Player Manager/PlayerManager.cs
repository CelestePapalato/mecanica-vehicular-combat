using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> carPrefabs;
    [SerializeField]
    private List<LayerMask> playerLayers;
    [SerializeField]
    private List<LayerMask> hurtboxLayers;
    [SerializeField]
    private List<LayerMask> hitboxLayers;
    [SerializeField]
    private List<Transform> startingPoints;

    private List<PlayerInput> players = new List<PlayerInput>();

    private PlayerInputManager playerInputManager;

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
    }

    private void OnEnable()
    {
        playerInputManager.playerJoinedEvent.AddListener(AddPlayer);
    }

    private void OnDisable()
    {
        playerInputManager.playerJoinedEvent?.RemoveListener(AddPlayer);
    }

    private void AddPlayer(PlayerInput player)
    {
        players.Add(player);
        Transform playerTransform = player.transform;
        playerTransform.position = startingPoints[players.Count -1].position;

        Transform playerCar = playerTransform.Find("Car").transform;

        Instantiate(carPrefabs[players.Count-1], playerCar);

        Debug.Log("Jugador " + players.Count + " ha entrado a la partida");

        int layerToAdd = (int)Mathf.Log(playerLayers[players.Count -1].value, 2);
        int hurtboxLayer = (int)Mathf.Log(hurtboxLayers[players.Count - 1].value, 2);
        int hitboxLayer = (int)Mathf.Log(hitboxLayers[players.Count - 1].value, 2);

        player.GetComponentInChildren<CinemachineVirtualCamera>().gameObject.layer = layerToAdd;
        player.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;
        player.GetComponentInChildren<Health>().gameObject.layer = hurtboxLayer;

        Damage[] hitboxs = player.GetComponentsInChildren<Damage>();
        
        foreach (Damage hitbox in hitboxs)
        {
            hitbox.gameObject.layer = hitboxLayer;
        }
    }
}
