using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DeckObjectBehaviour : MonoBehaviour
{
    public Text CardsRemaining;

    private PlayerObjectBehaviour pob;

    private void Start()
    {
        pob = GameLoop.FindParentWithTag(gameObject, "Player").GetComponent<PlayerObjectBehaviour>();
        
    }

    public void UpdateDeckNumber()
    {
        CardsRemaining.text = pob.Player.Deck.Size.ToString();
    }

    public void DrawNCards(int num)
    {
        new DrawCard(pob.Player, num).Fire(UpdateUI);
    }

    /// <summary>
    /// Called when draw card is done
    /// </summary>
    /// <param name="payload">Returned list of drawn cards</param>
    public void UpdateUI(object payload)
    {
        List<Card> cardList = (List<Card>)payload;
        for (int i = 0; i < cardList.Count; i ++)
        {
            GameObject co = CardObjectBehaviour.Create(cardList.ElementAt(i), pob);
            pob.HandArea.GetComponent<HandObjectBehaviour>().AddCard(co);
        }
        UpdateDeckNumber();
    }
}
