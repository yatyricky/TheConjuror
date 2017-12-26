using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DataManager : MonoBehaviour
{
    private Dictionary<string, List<DeckData>> deckBase;
    public Dictionary<string, List<DeckData>> DeckBase { get { return deckBase; } }
    public class DeckData
    {
        public int CardId;
        public int Num;

        public DeckData(int cardId, int num)
        {
            CardId = cardId;
            Num = num;
        }
    }

    private Dictionary<int, CardData> cardBase;
    public Dictionary<int, CardData> CardBase { get { return cardBase; } }
    public class CardData
    {
        public string Name;
        public string Color;
        public string Type;
        public int Cost;
        public string Description;
        public int Power;

        public CardData(string name, string color, string type, int cost, string description, int power)
        {
            Name = name;
            Color = color;
            Type = type;
            Cost = cost;
            Description = description;
            Power = power;
        }
    }

    private void Awake()
    {
        // parse cards
        string bulkText = Resources.Load<TextAsset>("data/all_cards").text;
        int idIndex = 0, nameIndex = 0, colorIndex = 0, typeIndex = 0, costIndex = 0, powerIndex = 0, descIndex = 0;
        StringReader reader = new StringReader(bulkText);
        string line = reader.ReadLine();
        string[] tokens = line.Split(',');
        for (int i = 0; i < tokens.Length; i ++)
        {
            if (tokens[i].Equals("id"))
                idIndex = i;
            if (tokens[i].Equals("name"))
                nameIndex = i;
            if (tokens[i].Equals("color"))
                colorIndex = i;
            if (tokens[i].Equals("type"))
                typeIndex = i;
            if (tokens[i].Equals("cost"))
                costIndex = i;
            if (tokens[i].Equals("power"))
                powerIndex = i;
            if (tokens[i].Equals("description"))
                descIndex = i;
        }
        cardBase = new Dictionary<int, CardData>();
        while ((line = reader.ReadLine()) != null)
        {
            tokens = line.Split(',');
            if (tokens[1] == "")
                break;

            int id = -1;
            if (!System.Int32.TryParse(tokens[idIndex], out id))
                throw new Exception("Bad csv format, id is not number");
            string name = tokens[nameIndex];
            string color = tokens[colorIndex];
            string type = tokens[typeIndex];
            int cost = -1;
            if (!System.Int32.TryParse(tokens[costIndex], out cost))
                throw new Exception("Bad csv format, cost is not number");
            string description = tokens[descIndex];
            int power = 0;
            System.Int32.TryParse(tokens[powerIndex], out power);

            CardData cardData = new CardData(name, color, type, cost, description, power);
            cardBase.Add(id, cardData);
        }

        // parse deck prototypes
        bulkText = Resources.Load<TextAsset>("data/decks").text;
        deckBase = new Dictionary<string, List<DeckData>>();
        List<DeckData> aDeck = null;
        reader = new StringReader(bulkText);
        while ((line = reader.ReadLine()) != null)
        {
            tokens = line.Split(',');

            if (tokens[0][0] != '1')
            {
                aDeck = new List<DeckData>();
                deckBase.Add(tokens[0], aDeck);
            }
            else
            {
                int n = -1;
                if (!System.Int32.TryParse(tokens[3], out n))
                    throw new Exception("Bad csv format, deck num is not number");
                int id = -1;
                if (!System.Int32.TryParse(tokens[0], out id))
                    throw new Exception("Bad csv format, deck id is not number");
                aDeck.Add(new DeckData(id, n));
            }
        }
    }
}
