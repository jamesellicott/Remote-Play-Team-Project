using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuButtons : MonoBehaviour
{

    [SerializeField] private NetworkManagerLobby networkManager;

    [Header("Canvas's")]
    public GameObject mainMenuCanvas;
    public GameObject joinGameCanvas;
    public GameObject lobbyCanvas;
    public GameObject deckViewerCanvas;

    [Header("Scenes")]
    public string mainMenuScene = "MainMenu";
    public string settingsScene = "SettingsMenu";

    [Header("UI")]
    public GameObject clientJoinPanel;
    public TextMeshProUGUI gameCodeText;

    public void LoadMainMenu()
    {
        mainMenuCanvas.SetActive(true);
        joinGameCanvas.SetActive(false);
        lobbyCanvas.SetActive(false);
        deckViewerCanvas.SetActive(false);
    }

    public void LoadJoinGame()
    {
        mainMenuCanvas.SetActive(false);
        joinGameCanvas.SetActive(true);
    }

    public void LoadDeckViewer()
    {
        mainMenuCanvas.SetActive(false);
        deckViewerCanvas.SetActive(true);
    }
    
    public void LoadSettings()
    {
        SceneManager.LoadScene(settingsScene);
    }

    public void LoadMainMenuFromSettings()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void StartAsHost()
    {
        networkManager.StartHost();

        joinGameCanvas.SetActive(false);
        lobbyCanvas.SetActive(true);

        string computerName = Dns.GetHostName();

        for (int i = 0; i <= Dns.GetHostEntry(computerName).AddressList.Length - 1; i++)
        {
            if (Dns.GetHostEntry(computerName).AddressList[i].IsIPv6LinkLocal == false)
            {
                gameCodeText.text = Dns.GetHostEntry(computerName).AddressList[i].ToString();
            }
        }
    }

    public void StartAsClient()
    {
        clientJoinPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
