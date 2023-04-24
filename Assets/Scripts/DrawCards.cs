using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DrawCards : NetworkBehaviour
{
    public PlayerManager playerManager;

    public void OnClick()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        playerManager = networkIdentity.GetComponent<PlayerManager>();
        playerManager.CmdDrawCards(5);
    }
}
