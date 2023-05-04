using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class NetworkManagerLobby : NetworkManager
{
    [Scene] [SerializeField] private string menuScene = string.Empty;
    [Scene] [SerializeField] private string gameScene = string.Empty;

    [SerializeField] private PlayerManager playerManagerPrefab;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        OnClientDisconnected?.Invoke();
    }

    //public override void OnServerConnect(NetworkConnectionToClient conn)
    //{
    //    if(numPlayers >= maxConnections)
    //    {
    //        conn.Disconnect();
    //        return;
    //    }

    //    if(SceneManager.GetActiveScene().name != menuScene)
    //    {
    //        conn.Disconnect();
    //        return;
    //    }
    //}

    //public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    //{
    //    if(SceneManager.GetActiveScene().name == menuScene)
    //    {
    //        GameObject playerInstance = Instantiate(playerPrefab);

    //        NetworkServer.AddPlayerForConnection(conn, playerInstance);
    //    }
    //}

    public void StartGame()
    {
        ServerChangeScene(gameScene);
    }
}
