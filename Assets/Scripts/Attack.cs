using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Attack : NetworkBehaviour
{
    PlayerManager playerManager;

    TurnManager turnManager;

    private void Start()
    {
        //get the player manager from the client that owns this card
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        playerManager = networkIdentity.GetComponent<PlayerManager>();

        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
    }

    public void OnClick()
    {
        if ((isServer && turnManager.hostsTurn) || (!isServer && !turnManager.hostsTurn))
        {
            if (isOwned && GetComponent<Card>().cardState == Card.CardState.PLAYED)
            {
                playerManager.CmdOwnedCardSelected(gameObject.GetComponent<NetworkIdentity>().netId);
            }
            else if (!isOwned && GetComponent<Card>().cardState == Card.CardState.PLAYED)
            {
                playerManager.CmdAttack(gameObject.GetComponent<NetworkIdentity>().netId);
            }
        }
    }
}
