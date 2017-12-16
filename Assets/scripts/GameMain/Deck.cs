using System;
using System.Linq;
using System.Collections.Generic;

public class Deck
{
    private string name;
    private List<Card> cards;
    private Random rnd;

    public string Name { get { return name; } set { name = value; } }
    public List<Card> Cards { get { return cards; } }
    public int Size { get { return cards.Count; } }

    public Deck()
    {
        rnd = new Random();
        cards = new List<Card>();
    }

    public Card DrawRandom()
    {
        if (cards.Count > 0)
        {
            int index = rnd.Next(cards.Count);
            Card ret = cards.ElementAt(index);
            cards.RemoveAt(index);
            return ret;
        }
        else
        {
            throw new Exception("Drawing card from empty deck");
        }
    }
}
