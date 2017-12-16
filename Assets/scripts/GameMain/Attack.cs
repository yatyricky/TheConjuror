using System.Collections.Generic;

public class Attack : GameAction
{
    private Player attacker;
    private Player defender;
    private int cardSlot;

    public Attack(Player attacker, Player defender, int cardSlot)
    {
        this.attacker = attacker;
        this.defender = defender;
        this.cardSlot = cardSlot;
    }

    public override void Fire(UpdateUICallBack callBack)
    {
        callBack(attacker.Attack(defender, cardSlot));
    }

    public class AttackResult
    {
        public List<Card> Attacker;
        public List<Card> Defender;
    }
}
