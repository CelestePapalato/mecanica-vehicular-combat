using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinnerCanvas : MonoBehaviour
{
    [SerializeField] private TMP_Text winnerNameText;
    public Color player1Color;
    public Color player2Color;

    Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }

    private void OnEnable()
    {
        GameManager.onWinner += EnableScreen;
    }

    private void OnDisable()
    {
        GameManager.onWinner -= EnableScreen;
    }

    public void EnableScreen(string name)
    {
        Debug.Log("arrr");
        winnerNameText.text = name;
        winnerNameText.color = (name.Contains('1')) ? player1Color : player2Color;
        canvas.enabled = true;
    }
}
