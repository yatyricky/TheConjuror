public class PlayCard : GameAction
{
    private int player;
    private Card card;

    public PlayCard(int player, Card card) : base()
    {
        this.player = player;
        this.card = card;
    }

    public override void Fire(UpdateUICallBack callBack)
    {
        Player.P(player).PlayCardFromHand(card);
        callBack(player);
    }

}
