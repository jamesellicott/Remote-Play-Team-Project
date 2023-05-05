using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    [Header("Player Details")]
    public float maxHealth = 200;
    public float maxMana = 200;
    public float health = 200;
    public float mana = 200;

    [Header("Cards")]
    public GameObject card1;
    public GameObject card2;
    public GameObject card3;
    public GameObject card4;

    [Header("Zones")]
    public GameObject playerArea;
    public GameObject enemyArea;
    public GameObject dropZoneP;
    public GameObject dropZoneE;

    [Header("PlayerDetails")]
    public Image playerHealthBar;
    public TextMeshProUGUI playerHealthText;
    public Image playerManaBar;
    public TextMeshProUGUI playerManaText;

    [Header("EnemyDetails")]
    public Image enemyHealthBar;
    public TextMeshProUGUI enemyHealthText;
    public Image enemyManaBar;
    public TextMeshProUGUI enemyManaText;

    List<GameObject> cards = new List<GameObject>();

    [Header("Attack")] [SyncVar]
    public GameObject attackingCard;

    [Header("Turns")]
    TurnManager turnManager;
    public Button endTurnBtn;


    //called on the client when the game object spawns on the client or when the client connects to a server
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            playerArea = GameObject.Find("PlayerZone");
            enemyArea = GameObject.Find("EnemyZone");
            dropZoneP = GameObject.Find("DropZonePlayer");
            dropZoneE = GameObject.Find("DropZoneEnemy");

            playerHealthBar = GameObject.Find("PlayerHealthBar").GetComponent<Image>();
            playerHealthText = GameObject.Find("PlayerHealthText").GetComponent<TextMeshProUGUI>();
            playerManaBar = GameObject.Find("PlayerManaBar").GetComponent<Image>();
            playerManaText = GameObject.Find("PlayerManaText").GetComponent<TextMeshProUGUI>();

            enemyHealthBar = GameObject.Find("EnemyHealthBar").GetComponent<Image>();
            enemyHealthText = GameObject.Find("EnemyHealthText").GetComponent<TextMeshProUGUI>();
            enemyManaBar = GameObject.Find("EnemyManaBar").GetComponent<Image>();
            enemyManaText = GameObject.Find("EnemyManaText").GetComponent<TextMeshProUGUI>();

            playerHealthText.text = health.ToString();
            playerManaText.text = mana.ToString();

            enemyHealthText.text = health.ToString();
            enemyManaText.text = mana.ToString();

            turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
            endTurnBtn = GameObject.Find("EndTurnBtn").GetComponent<Button>();

            Debug.Log("Btn: " + endTurnBtn);
        }
    }

    //called on the server when this game object is spawned on the server
    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();

        cards.Add(card1);
        cards.Add(card2);
        cards.Add(card3);
        cards.Add(card4);
    }

    public void DrawCards(int ammount)
    {
        CmdDrawCards(ammount);
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
                card.GetComponent<Card>().FlipCard();
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
                card.GetComponent<Card>().FlipCard();
            }
        }
    }

    [Command]
    public void CmdOwnedCardSelected(uint cardNetID)
    {
        RpcSelectAttackingCard(cardNetID);
    }

    [ClientRpc]
    private void RpcSelectAttackingCard(uint cardNetID)
    {
        if (attackingCard != null)
            attackingCard.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        attackingCard = NetworkClient.spawned[cardNetID].gameObject;
        attackingCard.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
    }

    [Command]
    public void CmdAttack(uint cardNetID)
    {
        Debug.Log("Attack Card");
        if (attackingCard == null) return;

        GameObject opponenetsCard = NetworkServer.spawned[cardNetID].gameObject;

        if (opponenetsCard.GetComponent<Card>().health < attackingCard.GetComponent<Card>().attackPower)
        {
            //Destroy losing card
            NetworkServer.Destroy(opponenetsCard);

            //Deal damage to the losing player
            int damageToBeDealt = attackingCard.GetComponent<Card>().attackPower - opponenetsCard.GetComponent<Card>().health;
            NetworkIdentity opponentsIdentity = opponenetsCard.GetComponent<NetworkIdentity>().connectionToClient.identity;

            RpcUpdateHealth(opponentsIdentity, damageToBeDealt);
        }
    }

    [ClientRpc]
    private void RpcUpdateHealth(NetworkIdentity oppIdentity, int damageToBeDealt)
    {
        oppIdentity.GetComponent<PlayerManager>().health -= damageToBeDealt;

        if (isOwned)
        {
            enemyHealthBar.fillAmount = oppIdentity.GetComponent<PlayerManager>().health / maxHealth;
            enemyHealthText.text = oppIdentity.GetComponent<PlayerManager>().health.ToString();
        }
        else
        {
            playerHealthBar.fillAmount = oppIdentity.GetComponent<PlayerManager>().health / maxHealth;
            playerHealthText.text = oppIdentity.GetComponent<PlayerManager>().health.ToString();
        }

        attackingCard.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        attackingCard = null;
    }

    [Command]
    public void CmdTakeMana(int manaToBeTaken)
    {
        RpcTakeMana(manaToBeTaken);
    }

    [ClientRpc]
    private void RpcTakeMana(int manaToBeTaken)
    {
        mana -= manaToBeTaken;
    }

    [Command]
    public void CmdUpdateManaBar()
    {
        RpcUpdateMana();
    }

    [ClientRpc]
    private void RpcUpdateMana()
    {
        if(isOwned)
        {
            playerManaBar.fillAmount = mana / maxMana;
            playerManaText.text = mana.ToString();
        }
        else
        {
            enemyManaBar.fillAmount = mana / maxMana;
            enemyManaText.text = mana.ToString();
        }
    }

    public void TurnOnEndTurnBtn()
    {
        endTurnBtn.gameObject.SetActive(true);
    }
    public void TurnOffEndTurnBtn()
    {
        endTurnBtn.gameObject.SetActive(false);
    }

    //Debug Stuff

    private void Update()
    {
        if (isLocalPlayer)
            Debug.Log("Attacking Card: " + attackingCard + " Player Identity: " + NetworkClient.connection.identity);
    }
}
