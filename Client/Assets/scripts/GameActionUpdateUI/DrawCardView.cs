using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DrawCardView : GameActionUpdateUIView
{

    public override void DoAction()
    {
        List<Card> cardList = (List<Card>)Payload;
        for (int i = 0; i < cardList.Count; i++)
        {
            GameObject co = CardObjectBehaviour.Create(cardList.ElementAt(i), POB);
            POB.HandArea.GetComponent<HandObjectBehaviour>().AddCard(co);
        }
        POB.Deck.GetComponent<DeckObjectBehaviour>().UpdateDeckNumber();
    }
}
