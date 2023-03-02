using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour
{
    public GameObject card1;
    public GameObject card2;
    public GameObject card3;
    public GameObject playerArea;
    public GameObject enemyArea;
    public GameObject dropZone;

    List<GameObject> cards = new List<GameObject>();

    private void Awake()
    {
        playerArea = GameObject.Find("PlayerArea");
        enemyArea = GameObject.Find("EnemyArea");
        dropZone = GameObject.Find("DropZone");

        cards.Add(card1);
        cards.Add(card2);
        cards.Add(card3);
    }

    private void DrawCards()
    {

    }
}
