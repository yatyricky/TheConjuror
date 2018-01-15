using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DeckObjectBehaviour : MonoBehaviour
{
    public Text CardsRemaining;
    public GameObject PlayerObject;

    [HideInInspector] public PlayerObjectBehaviour Pob;

    private void Awake()
    {
        Pob = PlayerObject.GetComponent<PlayerObjectBehaviour>();
    }

    internal void UpdateDeckNumber(int deckN)
    {
        CardsRemaining.text = deckN.ToString();
    }

    internal void DrawCardsUI(List<Card> cards, int deckN)
    {
        new DrawCardView(Pob, cards, deckN);
    }
}
