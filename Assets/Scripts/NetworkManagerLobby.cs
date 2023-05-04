using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class NetworkManagerLobby : NetworkManager
{
    [Scene] [SerializeField] private string menuScene = string.Empty;

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

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if(numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if(SceneManager.GetActiveScene().name != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if(SceneManager.GetActiveScene().name == menuScene)
        {
            PlayerManager playerManagerInstance = Instantiate(playerManagerPrefab);

            NetworkServer.AddPlayerForConnection(conn, playerManagerInstance.gameObject);
        }
    }
}
