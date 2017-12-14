using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DeckObjectBehaviour : MonoBehaviour
{
    public Text cardsRemaining;
    public GameObject cardPrefab;
    public GameObject handArea;
    public int whichPlayer;

    private float zOffset;

    private void Awake()
    {
        zOffset = 0f;
    }

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
        Vector3 prefabOrigin = cardPrefab.transform.position;
        List<Card> cardList = (List<Card>)payload;

        for (int i = 0; i < cardList.Count; i ++)
        {
            GameObject co = Instantiate(cardPrefab, new Vector3(prefabOrigin.x, prefabOrigin.y, prefabOrigin.z + zOffset), Quaternion.identity);
            CardObjectBehaviour cob = co.GetComponent<CardObjectBehaviour>();

            // card visual
            cob.cardImage.sprite = IMG2Sprite.LoadNewSprite(Application.dataPath + "/sprites/card_images/" + cardList.ElementAt(i).id + ".png");
            cob.cardName.text = cardList.ElementAt(i).name;
            cob.cardClass.sprite = IMG2Sprite.LoadNewSprite(Application.dataPath + "/sprites/card_ui/frame_" + cardList.ElementAt(i).color + ".png");
            cob.cardType.sprite = IMG2Sprite.LoadNewSprite(Application.dataPath + "/sprites/card_ui/type_" + cardList.ElementAt(i).type + ".png");
            cob.cardPower.text = cardList.ElementAt(i).power.ToString();
            cob.cardDescription.text = cardList.ElementAt(i).description.Replace("|n", "\n");
            cob.cardCostNumber.text = cardList.ElementAt(i).cost.ToString();
            cob.cardCostImage.sprite = IMG2Sprite.LoadNewSprite(Application.dataPath + "/sprites/card_ui/mana_" + cardList.ElementAt(i).color + ".png");
            Sprite powerFrame = IMG2Sprite.LoadNewSprite(Application.dataPath + "/sprites/card_ui/power_" + cardList.ElementAt(i).color + ".png");
            cob.cardTypeFrame.sprite = powerFrame;
            cob.cardPowerFrame.sprite = powerFrame;

            // layout
            cob.Order = zOffset;

            if (!cardList.ElementAt(i).type.Equals("creature"))
            {
                cob.cardPowerFrame.enabled = false;
                cob.cardPower.enabled = false;
            }

            cob.Owner = whichPlayer;
            cob.CardData = cardList.ElementAt(i);

            handArea.GetComponent<HandObjectBehaviour>().AddCard(co);
        }

        cardsRemaining.text = Player.P(whichPlayer).Deck.Size.ToString();

        zOffset += -0.01f;
    }
}
