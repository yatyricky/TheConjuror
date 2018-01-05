public class Card1000028 : CardAbility
{
    internal override void DoAction(Player player, GameAction.UpdateUICallBack callBack)
    {
        new DrawCard(player, 1).Fire(callBack);
    }
}
