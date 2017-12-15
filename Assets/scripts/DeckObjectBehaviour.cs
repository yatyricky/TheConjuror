using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DeckObjectBehaviour : MonoBehaviour
{
    public Text cardsRemaining;
    public GameObject handArea;
    public int whichPlayer;

    private void Start()
    {
        cardsRemaining.text = Player.P(whichPlayer).Deck.Size.ToString();
    }

    private void OnMouseUp()
    {
        new DrawCard(whichPlayer, 1).Fire(UpdateUI);
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
            GameObject co = CardObjectBehaviour.Create(cardList.ElementAt(i), whichPlayer);
            handArea.GetComponent<HandObjectBehaviour>().AddCard(co);
        }
        cardsRemaining.text = Player.P(whichPlayer).Deck.Size.ToString();
    }
}
