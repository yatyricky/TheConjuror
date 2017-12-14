using System.Collections.Generic;

public class DrawCard : GameAction
{
    private int player;
    private int num;
    private List<Card> drawn;

    public DrawCard(int player, int num) : base()
    {
        this.player = player;
        this.num = num;
        drawn = new List<Card>();
    }

    public override void Fire(UpdateUICallBack callBack)
    {
        for (int i = 0; i < num; i ++)
        {
            drawn.Add(Player.P(player).DrawRandom());
        }
        callBack(drawn);
    }
}
