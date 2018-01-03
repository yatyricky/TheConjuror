using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class PlaySpellCard : GameAction
{
    private Player player;
    private Card cardData;

    public PlaySpellCard(Player player, Card cardData)
    {
        this.player = player;
        this.cardData = cardData;
    }

    public override void Fire(UpdateUICallBack callBack)
    {
        player.PlaySpellCardFromHand(cardData);

        Assembly assembly = Assembly.GetExecutingAssembly();
        Type type = null;
        try
        {
            type = assembly.GetTypes().First(t => t.Name == "Card" + cardData.Id);
        }
        catch (InvalidOperationException ioe)
        {
            Debug.LogWarning(ioe);
        }
        if (type != null)
        {
            CardAbility obj = (CardAbility)Activator.CreateInstance(type);
            obj.Card = cardData;
            obj.DoAction(player, callBack);
        }
        else
        {
            callBack(null);
        }
    }
}
