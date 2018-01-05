using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Card
{
    public int Id;
    public string Name;
    public string Color;
    public string Type;
    public int Cost;
    public string Description;

    public int Power; // creature only
    public int MaxPower;

    public CardObjectBehaviour COB;

    public List<Buff> Buffs;

    public Card(int id)
    {
        DataManager dm = GameObject.FindGameObjectWithTag("GameData").GetComponent<DataManager>();
        DataManager.CardData cd;
        if (dm.CardBase.TryGetValue(id, out cd))
        {
            Id = id;
            Name = cd.Name;
            Color = cd.Color;
            Type = cd.Type;
            Cost = cd.Cost;
            Description = cd.Description;
            Power = cd.Power;
            MaxPower = Power;
            Buffs = new List<Buff>();
        }
        else
        {
            throw new Exception("Creating card from unknown id:" + id.ToString());
        }
    }

    internal void AddBuff(Buff buff)
    {
        Buffs.Add(buff);
    }

    internal bool CanBattle()
    {
        bool can = true;
        foreach (Buff buff in Buffs)
        {
            if (buff.Type == Buff.BuffType.NO_BATTLE)
            {
                can = false;
            }
        }
        return can;
    }

    internal void CheckBuffsEndTurn(Player caster)
    {
        for (int i = 0; i < Buffs.Count; i ++)
        {
            Buff item = Buffs.ElementAt(i);
            if (item.ShouldRemoveEndTurn() && caster == item.Caster)
            {
                RemoveBuff(item);
                i--;
            }
        }
    }

    private void RemoveBuff(Buff buff)
    {
        Buffs.Remove(buff);
        COB.RemoveBuff(buff.BO);
    }
}