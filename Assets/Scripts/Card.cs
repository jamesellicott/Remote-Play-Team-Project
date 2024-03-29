using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Card : NetworkBehaviour
{
    public enum CardState
    {
        DECK,
        DRAWN,
        PLAYED,
        DESTROYED
    }

    PlayerManager playerManager;

    public CardState cardState = CardState.DRAWN;

    [Header("Card Details")]
    public int health = 0;
    public int attackPower = 0;
    public int manaRequirement = 0;

    public Color startingColor;
    public Color backColor;

    [Header("Drag & Drop")]
    GameObject canvas;

    GameObject startParent;
    Vector2 startPos;
    GameObject dropZone;
    bool isOverDropZone;

    bool isDragging = false;
    bool isDraggable = false;

    private TurnManager turnManager;

    void Start()
    {
        canvas = GameObject.Find("MainCanvas");

        if (isOwned)
            isDraggable = true;

        //startingColor = gameObject.GetComponent<Image>().color;
        //backColor = Color.white;

        //get the player manager from the client that owns this card
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        playerManager = networkIdentity.GetComponent<PlayerManager>();

        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
    }

    void Update()
    {
        if (isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            transform.SetParent(canvas.transform, true);
        }
    }

    public void StartDrag()
    {
        if ((isServer && turnManager.hostsTurn) || (!isServer && !turnManager.hostsTurn))
        {
            if (isDraggable)
            {
                isDragging = true;

                startParent = transform.parent.gameObject;
                startPos = transform.position;
            }
        }
    }

    public void StopDrag()
    {
        if ((isServer && turnManager.hostsTurn) || (!isServer && !turnManager.hostsTurn))
        {
            if (isDraggable)
            {
                isDragging = false;

                if (isOverDropZone && playerManager.mana >= manaRequirement)
                {
                    playerManager.CmdTakeMana(manaRequirement);
                    playerManager.CmdUpdateManaBar();
                    cardState = CardState.PLAYED;
                    transform.SetParent(dropZone.transform, false);
                    isDraggable = false;
                    playerManager.PlayCard(gameObject);
                }
                else
                {
                    transform.position = startPos;
                    transform.SetParent(startParent.transform, false);
                }
            }
        }
    }

    public void FlipCard()
    {
        Color currColor = gameObject.GetComponent<Image>().color;

        if(currColor == startingColor)
        {
            gameObject.GetComponent<Image>().color = backColor;
        }
        else
        {
            gameObject.GetComponent<Image>().color = startingColor;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isOverDropZone = true;
        dropZone = collision.gameObject;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isOverDropZone = false;
        dropZone = null;
    }
}
