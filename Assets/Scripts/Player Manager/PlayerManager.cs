using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private List<LayerMask> playerLayers;
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

        Debug.Log("Jugador " + players.Count + " ha entrado a la partida");

        int layerToAdd = (int)Mathf.Log(playerLayers[players.Count -1].value, 2);

        player.GetComponentInChildren<CinemachineVirtualCamera>().gameObject.layer = layerToAdd;
        player.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;
    }
}
