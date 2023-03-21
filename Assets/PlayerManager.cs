using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    public GameObject card1;
    public GameObject card2;
    public GameObject card3;

    public GameObject playerArea;
    public GameObject enemyArea;
    public GameObject dropZone;

    List<GameObject> cards = new List<GameObject>();

    //called on the client when the game object spawns on the client or when the client connects to a server
    public override void OnStartClient()
    {
        base.OnStartClient();

        playerArea = GameObject.Find("PlayerZone");
        enemyArea = GameObject.Find("EnemyZone");
        dropZone = GameObject.Find("DropZone");
    }

    //called on the server when this game object is spawned on the server
    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();

        cards.Add(card1);
        cards.Add(card2);
        cards.Add(card3);
    }

    //Function that will run on the server when it is called on the client
    [Command]
    public void CmdDrawCards(int ammount)
    {
        //instanciate 5 random cards and snap them to the player zone
        for(int i = 0; i < ammount; i++)
        {
            GameObject card = Instantiate(cards[Random.Range(0, cards.Count)], new Vector2(0, 0), Quaternion.identity);
            //Spawns the card on the network  connectionToClient means this client has authority over the card gameObject
            NetworkServer.Spawn(card);
            RpcShowCard(card, "Dealt");
        }
    }

    [ClientRpc]
    void RpcShowCard(GameObject card, string type)
    {
        if(type == "Dealt")
        {
            if(isOwned)
            {
                card.transform.SetParent(playerArea.transform, false);
            }
            else
            {
                card.transform.SetParent(enemyArea.transform, false);
            }
        }
        else if(type == "Played")
        {

        }
    }
}
