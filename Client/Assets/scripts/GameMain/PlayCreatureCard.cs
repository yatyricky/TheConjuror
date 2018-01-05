public class PlayCreatureCard : GameAction
{
    private Player player;
    private Card card;
    private int slotId;

    public PlayCreatureCard(Player player, Card card, int slotId) : base()
    {
        this.player = player;
        this.card = card;
        this.slotId = slotId;
    }

    public override void Fire(UpdateUICallBack callBack)
    {
        player.PlayCardFromHand(card, slotId);
        Payload data = new Payload
        {
            ActionName = GetType().Name,
            payload = slotId
        };
        callBack(data);
    }

}
