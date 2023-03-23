using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    [Header("Player Details")]
    public int health = 10;
    public int mana = 10;

    [Header("Cards")]
    public GameObject card1;
    public GameObject card2;
    public GameObject card3;

    [Header("Zones")]
    public GameObject playerArea;
    public GameObject enemyArea;
    public GameObject dropZoneP;
    public GameObject dropZoneE;

    List<GameObject> cards = new List<GameObject>();

    [Header("Attack")]
    public GameObject attackingCard;

    //called on the client when the game object spawns on the client or when the client connects to a server
    public override void OnStartClient()
    {
        base.OnStartClient();

        playerArea = GameObject.Find("PlayerZone");
        enemyArea = GameObject.Find("EnemyZone");
        dropZoneP = GameObject.Find("DropZonePlayer");
        dropZoneE = GameObject.Find("DropZoneEnemy");
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
            NetworkServer.Spawn(card, connectionToClient);
            RpcShowCard(card, "Dealt");
        }
    }

    public void PlayCard(GameObject card)
    {
        CmdPlayCard(card);
    }

    [Command]
    void CmdPlayCard(GameObject card)
    {
        RpcShowCard(card, "Played");
    }

    [ClientRpc]
    void RpcShowCard(GameObject card, string type)
    {
        if (type == "Dealt")
        {
            if (isOwned)
            {
                card.transform.SetParent(playerArea.transform, false);
            }
            else
            {
                card.transform.SetParent(enemyArea.transform, false);
            }
        }
        else if (type == "Played")
        {
            if (isOwned)
            {
                card.transform.SetParent(dropZoneP.transform, false);
            }
            else
            {
                card.transform.SetParent(dropZoneE.transform, false);
            }
        }
    }

    [Command]
    public void CmdSetAttackingCard(GameObject card)
    {
        Debug.Log("CardWasSetTo: " + attackingCard);
        attackingCard = card;
        Debug.Log("CardSetTo: " + attackingCard);
    }

    private void Update()
    {
        Debug.Log("Attacking Card: " + attackingCard + " Player Identity: " + NetworkClient.connection.identity);
    }

    public void Attack(GameObject attackedCard)
    {
        Debug.Log("Attack");
        Debug.Log("Attakcing Card Set: " + attackingCard + " Player Identity: " + NetworkClient.connection.identity);
        //if (attackingCard != null)
        //{
        //    CmdAttackCard(attackedCard);
        //}
    }

    //[Command]
    //void CmdAttackCard(GameObject attackedCard)
    //{
    //    Debug.Log("Attack Command");
    //    if (attackedCard.GetComponent<Card>().health < attackingCard.GetComponent<Card>().attackPower)
    //    {
    //        Debug.Log("Destroy");
    //        RpcCardDestroyed(attackedCard);
    //    }
    //}

    //[ClientRpc]
    //void RpcCardDestroyed(GameObject destroyedCard)
    //{
    //    Debug.Log("Destroy RPC");
    //    //Destroy attacked card
    //    NetworkServer.Destroy(destroyedCard);
    //    //Deal damage to enemy
    //}
}
