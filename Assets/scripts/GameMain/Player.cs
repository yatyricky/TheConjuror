using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private Deck deck = null;
    private List<Card> hand;
    private List<Card> creature;
    private List<Card> grave;
    private CardSlot[] cardSlots;

    private static Player[] players = null;

    public static Player P(int id)
    {
        if (players == null)
        {
            players = new Player[2];
            players[0] = new Player();
            players[1] = new Player();

            players[0].InitDeck("blue_basic");
            players[1].InitDeck("red_basic");
        }
        return players[id];
    }

    private Player()
    {
        hand = new List<Card>();
        creature = new List<Card>();
        grave = new List<Card>();
        cardSlots = new CardSlot[5];
        cardSlots[0] = new CardSlot(Colors.WHITE);
        cardSlots[1] = new CardSlot(Colors.BLUE);
        cardSlots[2] = new CardSlot(Colors.ALL);
        cardSlots[3] = new CardSlot(Colors.GREEN);
        cardSlots[4] = new CardSlot(Colors.RED);
    }

    public Deck Deck
    {
        get
        {
            return deck;
        }
    }

    internal Card DrawRandom()
    {
        Card card = deck.DrawRandom();
        hand.Add(card);
        return card;
    }

    internal void PlayCardFromHand(Card card)
    {
        Card toPlay = null;
        int index = -1;
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand.ElementAt(i) == card)
            {
                toPlay = hand.ElementAt(i);
                index = i;
            }
        }
        if (toPlay == null)
        {
            throw new System.Exception("Trying to play card that not in hand");
        }
        hand.RemoveAt(index);

        if (toPlay.type.Equals(CardTypes.CREATURE))
        {
            creature.Add(toPlay);
        }
        else
        {
            // TODO playing enchantments, spells
            grave.Add(toPlay);
        }
    }

    private void InitDeck(string name)
    {
        Deck deck = null;
        DataManager dm = GameObject.FindGameObjectWithTag("GameData").GetComponent<DataManager>();
        for (int i = 0; i < dm.DeckTemplates.Count; i++)
        {
            if (dm.DeckTemplates.ElementAt(i).Name.Equals(name))
            {
                deck = (Deck)dm.DeckTemplates.ElementAt(i).Clone();
            }
        }
        if (deck == null)
        {
            deck = (Deck)dm.DeckTemplates.ElementAt(0).Clone();
        }
        this.deck = deck;
    }
}