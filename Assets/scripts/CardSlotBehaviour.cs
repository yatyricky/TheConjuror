using UnityEngine;
using UnityEngine.UI;

public class CardSlotBehaviour : MonoBehaviour
{
    public int OwnerPlayerId;
    public Sprite ImageAsset;
    public Image TheImage;

    private void Awake()
    {
        TheImage.sprite = ImageAsset;
        if (OwnerPlayerId == 1)
        {
            TheImage.GetComponent<RectTransform>().Rotate(new Vector3(-180f, 180f, 0f));
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseEnter()
    {
    }
}
