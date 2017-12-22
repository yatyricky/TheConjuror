using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DrawCardView
{
    public DrawCardView(List<Card> cardList, PlayerObjectBehaviour pob)
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            GameObject co = CardObjectBehaviour.Create(cardList.ElementAt(i), pob);
            pob.HandArea.GetComponent<HandObjectBehaviour>().AddCard(co);
        }
        pob.Deck.GetComponent<DeckObjectBehaviour>().UpdateDeckNumber();
    }

}
