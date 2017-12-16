using System.Collections.Generic;
using System.Linq;

public class CardSlot
{
    private string color;
    private List<Card> cards;
    public List<Card> Cards
    {
        get { return cards; }
    }

    public CardSlot(string color)
    {
        this.color = color;
        this.cards = new List<Card>();
    }

    public void Add(Card card)
    {
        cards.Add(card);
    }

    public int GetTotalPower()
    {
        int sum = 0;
        for (int i = 0; i < cards.Count; i ++)
        {
            sum += cards.ElementAt(i).Power;
        }
        return sum;
    }

    internal List<Card> TakeDamage(int num)
    {
        List<Card> killed = new List<Card>();
        if (cards.Count > 0)
        {
            cards = cards.OrderBy(card => card.Power).ToList();

            int index = 0;
            while (num > 0 && index < cards.Count)
            {
                Card current = cards.ElementAt(index);
                if (current.Power > num)
                {
                    // aborbed the damage
                    current.Power -= num;
                    num = 0;
                }
                else
                {
                    // killed by this damage
                    num -= current.Power;
                    current.Power = 0;
                    cards.Remove(current);
                    killed.Add(current);
                    index--;
                }
                index++;
            }
        }
        return killed;
    }
}
