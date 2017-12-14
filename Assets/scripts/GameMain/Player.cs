using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    private int id;
    private Deck deck = null;
    private List<Card> hand;

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

    public Deck Deck
    {
        get
        {
            return deck;
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