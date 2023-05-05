using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyClientStart : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby networkManager;

    [Header("Canvas's")]
    public GameObject joinGameCanvas;
    public GameObject lobbyCanvas;

    [Header("UI")]
    public Button joinButton;
    public TMP_InputField gameCodeInputField;
    public TextMeshProUGUI lobbyGameCodeTxt;

    public Button startGameButton;

    private void OnEnable()
    {
        NetworkManagerLobby.OnClientConnected += HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        NetworkManagerLobby.OnClientConnected -= HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobbyClient()
    {
        networkManager.networkAddress = gameCodeInputField.text;
        networkManager.StartClient();

        joinButton.interactable = false;
    }

    public void Back()
    {
        gameObject.SetActive(false);
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;
        startGameButton.interactable = false;

        lobbyGameCodeTxt.text += gameCodeInputField.text;

        //joinGameCanvas.SetActive(false);
        lobbyCanvas.SetActive(true);
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;

        lobbyCanvas.SetActive(false);
        joinGameCanvas.SetActive(true);
    }
}
