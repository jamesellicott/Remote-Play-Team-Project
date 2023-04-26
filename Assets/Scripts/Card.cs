using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [Header("Drag & Drop")]
    GameObject canvas;

    GameObject startParent;
    Vector2 startPos;
    GameObject dropZone;
    bool isOverDropZone;

    bool isDragging = false;
    bool isDraggable = false;

    void Start()
    {
        canvas = GameObject.Find("MainCanvas");

        if (isOwned)
            isDraggable = true;

        //get the player manager from the client that owns this card
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        playerManager = networkIdentity.GetComponent<PlayerManager>();
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
        if (isDraggable)
        {
            isDragging = true;

            startParent = transform.parent.gameObject;
            startPos = transform.position;
        }
    }

    public void StopDrag()
    {
        if (isDraggable)
        {
            isDragging = false;

            if (isOverDropZone && playerManager.mana >= manaRequirement)
            {
                playerManager.mana -= manaRequirement;
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
