// using System.Collections.Generic;
// using UnityEngine;
//
// public class DeckViewer : MonoBehaviour
// {
//     public GameObject cardPrefab; // The prefab for the card object
//     public Transform deckParent; // The parent object for the deck
//     public List<CardData> deck; // The list of cards in the deck
//
//     private List<GameObject> cards = new List<GameObject>(); // The list of instantiated card objects
//
//     void Start()
//     {
//         // Instantiate card objects for each card in the deck
//         foreach (CardData cardData in deck)
//         {
//             GameObject cardObject = Instantiate(cardPrefab, deckParent);
//             cardObject.GetComponent<Card>().Initialize(cardData);
//             cards.Add(cardObject);
//         }
//     }
//
//     public void UpdateView()
//     {
//         // Destroy existing card objects
//         foreach (GameObject cardObject in cards)
//         {
//             Destroy(cardObject);
//         }
//         cards.Clear();
//
//         // Instantiate new card objects for each card in the deck
//         foreach (CardData cardData in deck)
//         {
//             GameObject cardObject = Instantiate(cardPrefab, deckParent);
//             cardObject.GetComponent<Card>().Initialize(cardData);
//             cards.Add(cardObject);
//         }
//     }
// }
