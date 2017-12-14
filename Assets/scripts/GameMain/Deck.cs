using System;
using System.Linq;
using System.Collections.Generic;

public class Deck : ICloneable
{
    private string name;
    private List<Card> cards;

    public Deck()
    {
        cards = new List<Card>();
    }

    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }

    public List<Card> Cards
    {
        get
        {
            return cards;
        }
    }

    public int Size
    {
        get
        {
            return cards.Count;
        }
    }

    public object Clone()
    {
        return MemberwiseClone();
    }

    public Card DrawRandom()
    {
        if (cards.Count > 0)
        {
            System.Random rnd = new System.Random();
            int index = rnd.Next(cards.Count);
            Card ret = cards.ElementAt(index);
            cards.RemoveAt(index);
            return ret;
        }
        else
        {
            return null;
        }
    }
}
