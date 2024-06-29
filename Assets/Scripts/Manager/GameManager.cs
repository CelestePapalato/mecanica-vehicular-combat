using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static event Action onGameStarted;
    public static event Action onGameFinished;
    public static event Action<string> onWinner;

    [SerializeField] int countdown = 3;
    [SerializeField] Canvas countdownCanvas;
    [SerializeField] TMP_Text countdownText;
    [SerializeField] int reloadSceneAfter;

    int timer;

    [Header("AI")]
    [SerializeField]
    GameObject AI_prefab;

    [Header("Player instancing")]
    [SerializeField]
    private List<LayerMask> playerLayers;
    [SerializeField]
    private List<LayerMask> hurtboxLayers;
    [SerializeField]
    private List<LayerMask> hitboxLayers;
    [SerializeField]
    private List<Transform> startingPoints;

    List<Car> currentCars = new List<Car>();

    bool gameStarted = false;

    private void Awake()
    {
        Time.timeScale = 0;
        gameStarted = false;
        if (countdownCanvas)
        {
            countdownCanvas.enabled = false;
        }
    }

    private void OnEnable()
    {
        Car.onDestroy += OnCarDestroyed;
    }

    private void OnDisable()
    {
        Car.onDestroy -= OnCarDestroyed;
    }


    [ContextMenu("Start Game")]
    public void StartGame()
    {
        if(PlayerManager.CurrentPlayers.Length == 0)
        {
            return;
        }
        PrepareCharacters();
        ActivatePlayerCamera();
        Time.timeScale = 1;
        StartCoroutine(Countdown());
    }

    private void OnCarDestroyed(Car car)
    {
        if (!gameStarted)
        {
            return;
        }

        if (!currentCars.Contains(car))
        {
            return;
        }

        int i =  currentCars.IndexOf(car);
        Debug.Log("Gana " + i + 1);
        onGameFinished?.Invoke();

        string winnerName = "Player";

        if(PlayerManager.CurrentPlayers.Length < 2 && i > 0)
        {
            winnerName = "AI";
        }
        else
        {
            winnerName = winnerName + " " + (i + 1);
        }

        onWinner?.Invoke(winnerName);
        Invoke(nameof(SceneReload), reloadSceneAfter);
    }

    private void PrepareCharacters()
    {
        PlayerInput[] currentPlayers = PlayerManager.CurrentPlayers;
        currentCars.Add(currentPlayers[0].GetComponentInChildren<Car>());
        if (currentPlayers.Length == 1)
        {
            SpawnAI();
        }
        else
        {
            currentCars.Add(currentPlayers[1].GetComponentInChildren<Car>());
        }

        foreach (var player in currentPlayers) {
            player.gameObject.SetActive(true);
        }

        for(int i = 0; i < currentCars.Count; i++)
        {
            Car car = currentCars[i];
            Transform parent = car.transform.parent;
            parent.position = startingPoints[i].position;

            int layerToAdd = (int)Mathf.Log(playerLayers[i].value, 2);
            int hurtboxLayer = (int)Mathf.Log(hurtboxLayers[i].value, 2);
            int hitboxLayer = (int)Mathf.Log(hitboxLayers[i].value, 2);

            parent.GetComponentInChildren<CinemachineVirtualCamera>().gameObject.layer = layerToAdd;
            parent.GetComponentInChildren<Camera>().cullingMask |= 1 << layerToAdd;
            parent.GetComponentInChildren<Health>().gameObject.layer = hurtboxLayer;
            parent.GetComponentInChildren<Rigidbody>().isKinematic = false;

            Damage[] hitboxs = parent.GetComponentsInChildren<Damage>();

            foreach (Damage hitbox in hitboxs)
            {
                hitbox.gameObject.layer = hitboxLayer;
            }
        }
    }

    private void SpawnAI()
    {
        GameObject AI = Instantiate(AI_prefab);
        currentCars.Add(AI.GetComponentInChildren<Car>());
    }

    private void ActivatePlayerCamera()
    {
        PlayerInput[] players = PlayerManager.CurrentPlayers;

        float x = 0;
        foreach (var player in players)
        {
            Camera[] cameras = player.GetComponentsInChildren<Camera>();

            foreach(var camera in cameras)
            {
                camera.enabled = true;
            }

            if (players.Length > 1)
            {
                foreach (Camera cam in cameras)
                {
                    cam.rect = new Rect(x, 0, 0.5f, 1);
                }
                x += 0.5f;
            }
        }
    }

    IEnumerator Countdown()
    {
        if (countdownCanvas)
        {
            countdownCanvas.enabled = true;
        }
        timer = countdown;
        UpdateCountdownText(timer);
        while (timer > 0) {
            yield return new WaitForSeconds(1);
            timer--;
            UpdateCountdownText(timer);
        }
        if (countdownCanvas)
        {
            countdownCanvas.enabled = false;
        }
        gameStarted = true;
        onGameStarted?.Invoke();
    }

    private void UpdateCountdownText(int time)
    {
        if (countdownText)
        {
            countdownText.text = time.ToString();
        }
    }

    private void SceneReload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
