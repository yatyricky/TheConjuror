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
        if (cardData.Id == 1000028)
        {
            new DrawCard(player, 1).Fire(callBack);
        }
        else
        {
            callBack(null);
        }
    }
}
