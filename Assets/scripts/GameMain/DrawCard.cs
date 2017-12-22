using System.Collections.Generic;

public class DrawCard : GameAction
{
    private Player player;
    private int num;

    public DrawCard(Player player, int num) : base()
    {
        this.player = player;
        this.num = num;
    }

    public override void Fire(UpdateUICallBack callBack)
    {
        List<Card> drawn = new List<Card>();
        for (int i = 0; i < num; i ++)
        {
            drawn.Add(player.DrawRandom());
        }
        Payload payload = new Payload
        {
            ActionName = GetType().Name,
            payload = drawn
        };

        callBack(payload);
    }
}
