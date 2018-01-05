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

    private void UpdateUI(GameAction.Payload payload)
    {
        new DrawCardView
        {
            Payload = payload.payload,
            POB = pob
        }.DoAction();
    }
}
