using UnityEngine;
using UnityEngine.UI;

public class CardObjectBehaviour : MonoBehaviour {

    public GameObject cardRef;

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

    private Card cardData;
    public Card CardData
    {
        get
        {
            return cardData;
        }
        set
        {
            cardData = value;
        }
    }

    private int owner;
    public int Owner
    {
        get
        {
            return owner;
        }
        set
        {
            owner = value;
        }
    }

    private float order;
    public float Order
    {
        get
        {
            return order;
        }
        set
        {
            order = value;
        }
    }
    private bool mouseHovering;

    private void Awake()
    {
        mouseHovering = false;
    }

    // Use this for initialization
    void Start ()
    {
		
	}

    private void OnMouseOver()
    {
        if (!mouseHovering)
        {
            cardRef.transform.Translate(new Vector3(0, 0, -1f));
            mouseHovering = true;
        }
    }

    private void OnMouseExit()
    {
        if (mouseHovering)
        {
            cardRef.transform.Translate(new Vector3(0, 0, Order - cardRef.transform.position.z));
            mouseHovering = false;
        }
    }

    void OnMouseDown()
    {
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
