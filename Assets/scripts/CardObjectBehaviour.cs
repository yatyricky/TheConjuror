using UnityEngine;
using UnityEngine.UI;

public class CardObjectBehaviour : MonoBehaviour
{
    public Image cardClass; // earth/ fire/ air/ water
    public Image cardImage; // the graphics
    public Text cardName; // card name
    public Image cardTypeFrame;
    public Image cardType;
    public Image cardPowerFrame;
    public Text cardPower;
    public Text cardDescription;
    public Image cardCostImage;
    public Text cardCostNumber;

    private Vector3 originPos;
    public Vector3 OriginPos
    {
        get { return originPos; }
        set { originPos = value; }
    }

    private Card cardData;
    public Card CardData
    {
        get { return cardData; }
        set { cardData = value; }
    }

    private int owner;
    public int Owner
    {
        get { return owner; }
        set { owner = value; }
    }

    private float order;
    public float Order
    {
        get { return order; }
        set { order = value; }
    }

    private static float zOffset = 0f;
    public static GameObject Create(Card cardData, int player)
    {
        GameObject co = Instantiate(Resources.Load("prefabs/CardObject")) as GameObject;
        CardObjectBehaviour cob = co.GetComponent<CardObjectBehaviour>();

        // card visual
        cob.cardImage.sprite = IMG2Sprite.LoadNewSprite(Application.dataPath + "/sprites/card_images/" + cardData.id + ".png");
        cob.cardName.text = cardData.name;
        cob.cardClass.sprite = IMG2Sprite.LoadNewSprite(Application.dataPath + "/sprites/card_ui/frame_" + cardData.color + ".png");
        cob.cardType.sprite = IMG2Sprite.LoadNewSprite(Application.dataPath + "/sprites/card_ui/type_" + cardData.type + ".png");
        cob.cardPower.text = cardData.power.ToString();
        cob.cardDescription.text = cardData.description.Replace("|n", "\n");
        cob.cardCostNumber.text = cardData.cost.ToString();
        cob.cardCostImage.sprite = IMG2Sprite.LoadNewSprite(Application.dataPath + "/sprites/card_ui/mana_" + cardData.color + ".png");
        Sprite powerFrame = IMG2Sprite.LoadNewSprite(Application.dataPath + "/sprites/card_ui/power_" + cardData.color + ".png");
        cob.cardTypeFrame.sprite = powerFrame;
        cob.cardPowerFrame.sprite = powerFrame;

        // layout
        cob.Order = zOffset;

        if (!cardData.type.Equals(CardTypes.CREATURE))
        {
            cob.cardPowerFrame.enabled = false;
            cob.cardPower.enabled = false;
        }

        cob.Owner = player;
        cob.CardData = cardData;

        zOffset += -0.01f;
        return co;
    }

    private bool mouseHovering;

    private void Awake()
    {
        mouseHovering = false;
    }

    private void OnMouseOver()
    {
        if (!mouseHovering)
        {
            gameObject.transform.Translate(new Vector3(0, 0, -1f));
            mouseHovering = true;
        }
    }

    private void OnMouseExit()
    {
        if (mouseHovering)
        {
            gameObject.transform.Translate(new Vector3(0, 0, Order - gameObject.transform.position.z));
            mouseHovering = false;
        }
    }
}
