using UnityEngine;

public class Sanctuarize : GameAction
{
    private Player player;
    private Card target;

    public Sanctuarize(Player player, Card target)
    {
        this.player = player;
        this.target = target;
    }

    public override void Fire(UpdateUICallBack callBack)
    {
        Buff newBuff = new NoBattleBuff
        {
            Caster = player
        };
        target.AddBuff(newBuff);
        SanctuarizeData sd = new SanctuarizeData
        {
            buff = newBuff,
            card = target
        };
        Payload payload = new Payload
        {
            ActionName = GetType().Name,
            payload = sd
        };
        callBack(payload);
    }

    public class SanctuarizeData
    {
        public Buff buff;
        public Card card;
    }
}
