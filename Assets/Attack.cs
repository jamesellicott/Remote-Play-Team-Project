using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Attack : NetworkBehaviour
{
    PlayerManager playerManager;

    private void Start()
    {
        //get the player manager from the client that owns this card
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        playerManager = networkIdentity.GetComponent<PlayerManager>();
    }

    public void OnClick()
    {
        if(isOwned && GetComponent<Card>().cardState == Card.CardState.PLAYED)
        {
            playerManager.CmdOwnedCardSelected(gameObject);
        }
        else if (!isOwned && GetComponent<Card>().cardState == Card.CardState.PLAYED)
        {
            playerManager.CmdEnemyCardSelected(gameObject);
        }
    }
}
