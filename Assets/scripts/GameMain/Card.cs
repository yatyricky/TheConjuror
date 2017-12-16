using System;
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
        }
        else
        {
            throw new Exception("Creating card from unknown id:" + id.ToString());
        }
    }

}