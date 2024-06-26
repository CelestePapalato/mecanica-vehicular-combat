using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static event Action<PlayerInput> onPlayerAdded;
    public UnityEvent onFirstPlayerJoined;
    public UnityEvent onMultiplayer;
    public UnityEvent onMultiplayerDisabled;
    public UnityEvent onNoPlayersLeft;

    public static PlayerInput[] CurrentPlayers { get => players.ToArray(); }
    private static List<PlayerInput> players = new List<PlayerInput>();

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

    private PlayerInputManager playerInputManager;

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
        DontDestroyOnLoad(this);
    }

    private void OnEnable()
    {
        playerInputManager.playerJoinedEvent.AddListener(AddPlayer);
        playerInputManager.playerLeftEvent.AddListener(PlayerLeft);
    }

    private void OnDisable()
    {
        playerInputManager.playerJoinedEvent?.RemoveListener(AddPlayer);
        playerInputManager.playerLeftEvent?.RemoveListener(PlayerLeft);
    }

    private void AddPlayer(PlayerInput player)
    {
        players.Add(player);

        Transform playerTransform = player.transform;
        if (startingPoints.Count > 0) {

            playerTransform.position = startingPoints[players.Count - 1].position;
        }

        // MOVER ESTO AL GAME MANAGER

        Transform playerCar = playerTransform.Find("Car").transform;
        
        Instantiate(carPrefabs[players.Count-1], playerCar);

        player.gameObject.SetActive(false);

        Debug.Log("Jugador " + players.Count + " ha entrado a la partida");

        onPlayerAdded?.Invoke(player);
        if(players.Count == 0)
        {
            onFirstPlayerJoined?.Invoke();
        }
        else
        {
            onMultiplayer?.Invoke();
        }
        /*
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

        if(players.Count > 1)
        {
            float x = 0;
            foreach(PlayerInput _player in players)
            {
                Camera[] cameras = _player.GetComponentsInChildren<Camera>();
                foreach (Camera cam in cameras)
                {
                    cam.rect = new Rect(x, 0, 0.5f, 1);
                }
                x += 0.5f;
            }
        }
        */
    }

    private void PlayerLeft(PlayerInput player)
    {
        players.Remove(player);
        if (players.Count == 1)
        {
            Camera[] cameras = players[0].GetComponentsInChildren<Camera>();
            foreach (Camera cam in cameras)
            {
                cam.rect = new Rect(0, 0, 1, 1);
            }
            onMultiplayerDisabled?.Invoke();
        }

        if(players.Count == 0)
        {
            onNoPlayersLeft?.Invoke();
        }
    }
}
