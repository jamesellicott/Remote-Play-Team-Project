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
        Debug.Log("Clicked");
        if(isOwned && GetComponent<Card>().cardState == Card.CardState.PLAYED)
        {
            playerManager.SetAttackingCard(gameObject);
            Debug.Log("Setting Card " + gameObject);
        }
        else if(!isOwned)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            //Debug.Log("Players: " + players[0] + ", " + players[1]);
            foreach (GameObject player in players)
            {
                if(player != playerManager.gameObject)
                    player.GetComponent<PlayerManager>().Attack(gameObject);
            }
        }
    }
}
