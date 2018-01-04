using UnityEngine;

public class Card1000123 : CardAbility
{
    internal override void DoAction(Player player, GameAction.UpdateUICallBack callBack)
    {
        GameObject co = Object.Instantiate(Resources.Load("prefabs/Target")) as GameObject;
        SelectTargetCard sel = co.GetComponent<SelectTargetCard>();
        sel.SetData(player, Card, (target) =>
        {
            Object.Destroy(co);
            new Sanctuarize(player, target.GetComponent<CardObjectBehaviour>().CardData).Fire(callBack);
        });
    }
}
