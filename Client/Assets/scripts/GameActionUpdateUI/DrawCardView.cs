using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DrawCardView
{
    public DrawCardView(PlayerObjectBehaviour pob, List<Card> cards, int deckN)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject co = CardObjectBehaviour.Create(cards.ElementAt(i), pob);
            pob.Hob.AddCard(co);
            if (!BoardBehaviour.LocalPlayerName.Equals(pob.PlayerName))
            {
                co.GetComponent<DragHandCard>().CanDrag = false;
            }
        }
        pob.Dob.UpdateDeckNumber(deckN);
    }

}
