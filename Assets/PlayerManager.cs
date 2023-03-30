using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    [Header("Player Details")]
    [SyncVar]
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
            card.GetComponent<Card>().cardState = Card.CardState.DRAWN;
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
            card.GetComponent<Card>().cardState = Card.CardState.PLAYED;
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
    public void CmdOwnedCardSelected(GameObject card)
    {
        attackingCard = card;
        //NetworkIdentity opponentIdentity = card.GetComponent<NetworkIdentity>();
        //TargetRpcOwnedCardSelected(opponentIdentity.connectionToClient, card);
    }

    [TargetRpc]
    void TargetRpcOwnedCardSelected(NetworkConnection target, GameObject card)
    {
        attackingCard = card;
        Debug.Log("Attacking Card Set: " + attackingCard);
    }

    [Command]
    public void CmdEnemyCardSelected(GameObject card)
    {
        TargetRpcAttackEnemyCard(card);
    }

    [TargetRpc]
    void TargetRpcAttackEnemyCard(GameObject card)
    {
        Debug.Log("Attack Card Rpc");
        if (attackingCard != null)
            CmdAttack(card);
    }

    [Command]
    void CmdAttack(GameObject attackedCard)
    {
        if (attackedCard.GetComponent<Card>().health < attackingCard.GetComponent<Card>().attackPower)
        {
            //attacker wins encounter
            //Destroy losing card
            RpcDestroyCard(attackedCard);

            //Send a targeted Rpc to the player that lost to deal damage to them
            int damageToBeDealt = attackingCard.GetComponent<Card>().attackPower - attackedCard.GetComponent<Card>().health;
            NetworkIdentity opponentIdentity = attackedCard.GetComponent<NetworkIdentity>();
            TargetRpcDealPlayerDamage(opponentIdentity.connectionToClient, damageToBeDealt);
        }
    }

    [ClientRpc]
    void RpcDestroyCard(GameObject destroyedCard)
    {
        Debug.Log("Destroy RPC");
        //Destroy attacked card
        NetworkServer.Destroy(destroyedCard);
    }

    [TargetRpc]
    void TargetRpcDealPlayerDamage(NetworkConnection target, int damage)
    {
        health -= damage;
        Debug.Log("Player Taken Damage: " + damage + ", Current Health: " + health);
    }

    //Debug Stuff

    private void Update()
    {
        if (isLocalPlayer)
            Debug.Log("Attacking Card: " + attackingCard + " Player Identity: " + NetworkClient.connection.identity);
    }
}
