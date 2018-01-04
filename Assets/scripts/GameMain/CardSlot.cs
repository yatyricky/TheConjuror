using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

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
            Card item = cards.ElementAt(i);
            if (item.CanBattle())
            {
                sum += item.Power;
            }
        }
        return sum;
    }

    private Card GetLowestPower()
    {
        if (cards.Count > 0)
        {
            Card ret = null;
            int min = 9999;
            for(int i = 0; i < cards.Count; i ++)
            {
                Card current = cards.ElementAt(i);
                if (current.CanBattle())
                {
                    int power = current.Power;
                    if (power < min)
                    {
                        min = power;
                        ret = current;
                    }
                }
            }
            return ret;
        }
        else
        {
            return null;
        }
    }

    internal List<Card> TakeDamage(int num)
    {
        List<Card> killed = new List<Card>();
        if (cards.Count > 0)
        {
            bool exhausted = false;
            while (num > 0 && !exhausted)
            {
                Card current = GetLowestPower();
                if (current == null)
                {
                    exhausted = true;
                }
                else
                {
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
                    }
                }
            }
        }
        return killed;
    }

    internal void CheckBuffsEndTurn(Player caster)
    {
        foreach(Card card in cards)
        {
            card.CheckBuffsEndTurn(caster);
        }
    }

    internal List<Tuple<int, int>> GetModifiers(bool attacker)
    {
        List<Tuple<int, int>> res = new List<Tuple<int, int>>();
        for (int i = 0; i < cards.Count; i ++)
        {
            Card item = cards.ElementAt(i);
            if (item.CanBattle())
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Type type = null;
                try
                {
                    type = assembly.GetTypes().First(t => t.Name == "Card" + item.Id);
                }
                catch (InvalidOperationException ioe)
                {
                    Debug.LogWarning(ioe);
                }
                if (type != null)
                {
                    CardAbility obj = (CardAbility)Activator.CreateInstance(type);
                    obj.Card = item;
                    int mod = obj.GetAttackModifier(attacker);
                    if (mod != 0)
                    {
                        res.Add(new Tuple<int, int>(item.Id, mod));
                    }
                }
            }
        }
        return res;
    }
}
