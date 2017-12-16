using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataManager : MonoBehaviour
{
    public TextAsset allCards;
    public TextAsset decks;

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
        string[,] cardsCSV = CSVReader.SplitCsvGrid(allCards.text);
        cardBase = new Dictionary<int, CardData>();
        int idIndex = 0, nameIndex = 0, colorIndex = 0, typeIndex = 0, costIndex = 0, powerIndex = 0, descIndex = 0;
        for (int i = 0; i < cardsCSV.GetLength(0); i++)
        {
            if (cardsCSV[i, 0] != null)
            {
                if (cardsCSV[i, 0].Equals("id"))
                    idIndex = i;
                if (cardsCSV[i, 0].Equals("name"))
                    nameIndex = i;
                if (cardsCSV[i, 0].Equals("color"))
                    colorIndex = i;
                if (cardsCSV[i, 0].Equals("type"))
                    typeIndex = i;
                if (cardsCSV[i, 0].Equals("cost"))
                    costIndex = i;
                if (cardsCSV[i, 0].Equals("power"))
                    powerIndex = i;
                if (cardsCSV[i, 0].Equals("description"))
                    descIndex = i;
            }
        }
        for (int i = 1; i < cardsCSV.GetLength(1); i++)
        {
            if (cardsCSV[0, i] != null)
            {
                int id = -1;
                if (!System.Int32.TryParse(cardsCSV[idIndex, i], out id))
                    throw new Exception("Bad csv format, id is not number");
                string name = cardsCSV[nameIndex, i];
                string color = cardsCSV[colorIndex, i];
                string type = cardsCSV[typeIndex, i];
                int cost = -1;
                if (!System.Int32.TryParse(cardsCSV[costIndex, i], out cost))
                    throw new Exception("Bad csv format, cost is not number");
                string description = cardsCSV[descIndex, i];
                int power = 0;
                System.Int32.TryParse(cardsCSV[powerIndex, i], out power);

                CardData cardData = new CardData(name, color, type, cost, description, power);
                cardBase.Add(id, cardData);
            }
        }

        // parse deck prototypes
        string[,] decksCSV = CSVReader.SplitCsvGrid(decks.text);
        deckBase = new Dictionary<string, List<DeckData>>();
        List<DeckData> aDeck = null;
        for (int i = 0; i < decksCSV.GetLength(1); i++)
        {
            if (decksCSV[0, i] != null)
            {
                if (decksCSV[0, i][0] != '1')
                {
                    aDeck = new List<DeckData>();
                    deckBase.Add(decksCSV[0, i], aDeck);
                }
                else
                {
                    int n = -1;
                    if (!System.Int32.TryParse(decksCSV[3, i], out n))
                        throw new Exception("Bad csv format, deck num is not number");
                    int id = -1;
                    if (!System.Int32.TryParse(decksCSV[0, i], out id))
                        throw new Exception("Bad csv format, deck id is not number");
                    aDeck.Add(new DeckData(id, n));
                }
            }
        }
    }
}
