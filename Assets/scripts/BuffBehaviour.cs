using UnityEngine;
using UnityEngine.UI;

public class BuffBehaviour : MonoBehaviour
{
    public Image Image;

    private Buff buffData;

    public Buff BuffData { get { return buffData; } }

    public static GameObject Create(GameObject card, Buff buff)
    {
        GameObject bo = Instantiate(Resources.Load("prefabs/Buff")) as GameObject;
        BuffBehaviour bb = bo.GetComponent<BuffBehaviour>();
        bb.Image.sprite = Resources.Load<Sprite>("sprites/buffs/" + buff.IconPath);
        bb.buffData = buff;
        buff.BO = bo;
        bo.transform.SetParent(card.transform);
        card.GetComponent<CardObjectBehaviour>().AddBuff(bo);
        return bo;
    }
}
