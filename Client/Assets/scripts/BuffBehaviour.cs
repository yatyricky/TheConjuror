using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffBehaviour : MonoBehaviour
{
    public Image Image;

    [HideInInspector]public int Guid;

    private static Dictionary<int, BuffBehaviour> AllBuffs = new Dictionary<int, BuffBehaviour>();

    public static GameObject Create(CardObjectBehaviour cob, int guid, string path)
    {
        GameObject bo = Instantiate(Resources.Load("prefabs/Buff")) as GameObject;
        BuffBehaviour bb = bo.GetComponent<BuffBehaviour>();
        bb.Image.sprite = Resources.Load<Sprite>("sprites/buffs/" + path);
        bb.Guid = guid;
        bo.transform.SetParent(cob.gameObject.transform);
        cob.AddBuff(bo);
        AddBuff(bb);
        return bo;
    }

    private static void AddBuff(BuffBehaviour bob)
    {
        try
        {
            AllBuffs.Add(bob.Guid, bob);

        }
        catch (ArgumentException e)
        {
            throw new Exception(" --- Catched --- : " + e.Message);
        }
    }

    internal static BuffBehaviour GetBOB(int guid)
    {
        BuffBehaviour ret = null;
        if (AllBuffs.TryGetValue(guid, out ret))
        {
            return ret;
        }
        else
        {
            Debug.LogError(Environment.StackTrace);
            throw new Exception("No such buff in the game, id: " + guid);
        }
    }
}
