public class PlayCard : GameAction
{
    private Player player;
    private Card card;
    private int slotId;

    public PlayCard(Player player, Card card, int slotId) : base()
    {
        this.player = player;
        this.card = card;
        this.slotId = slotId;
    }

    public override void Fire(UpdateUICallBack callBack)
    {
        player.PlayCardFromHand(card, slotId);
        callBack(slotId);
    }

}
