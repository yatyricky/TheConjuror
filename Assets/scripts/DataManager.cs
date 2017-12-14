using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public TextAsset allCards;
    public TextAsset decks;
    public GameObject[] CardSlots;
    public GameObject[] HandAreas;

    private List<Deck> deckTemplates;
    private List<Card> cardBase;

    public List<Deck> DeckTemplates
    {
        get
        {
            return deckTemplates;
        }
    }

    private void Awake()
    {
        // parse cards
        string[,] cardsCSV = CSVReader.SplitCsvGrid(allCards.text);
        cardBase = new List<Card>();
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
                    throw new System.Exception("Bad csv format, id is not number");
                string name = cardsCSV[nameIndex, i];
                string color = cardsCSV[colorIndex, i];
                string type = cardsCSV[typeIndex, i];
                int cost = -1;
                if (!System.Int32.TryParse(cardsCSV[costIndex, i], out cost))
                    throw new System.Exception("Bad csv format, cost is not number");
                string description = cardsCSV[descIndex, i];
                int power = 0;
                System.Int32.TryParse(cardsCSV[powerIndex, i], out power);

                Card card = new Card(id, name, color, type, cost, description, power);
                cardBase.Add(card);
            }
        }

        // parse deck prototypes
        string[,] decksCSV = CSVReader.SplitCsvGrid(decks.text);
        deckTemplates = new List<Deck>();
        Deck aDeck = null;
        for (int i = 0; i < decksCSV.GetLength(1); i++)
        {
            if (decksCSV[0, i] != null)
            {
                if (decksCSV[0, i][0] != '1')
                {
                    aDeck = new Deck
                    {
                        Name = decksCSV[0, i]
                    };
                    deckTemplates.Add(aDeck);
                }
                else
                {
                    int n = -1;
                    if (!System.Int32.TryParse(decksCSV[3, i], out n))
                        throw new System.Exception("Bad csv format, deck num is not number");
                    for (int j = 0; j < n; j++)
                    {
                        int id = -1;
                        if (!System.Int32.TryParse(decksCSV[0, i], out id))
                            throw new System.Exception("Bad csv format, deck id is not number");
                        aDeck.Cards.Add(FindCardById(id));
                    }
                }
            }

        }

    }

    private Card FindCardById(int id)
    {
        for (int i = 0; i < cardBase.Count; i ++)
        {
            if (cardBase.ElementAt(i).id == id)
            {
                return cardBase.ElementAt(i);
            }
        }
        throw new System.Exception("Card not found, id = " + id);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
