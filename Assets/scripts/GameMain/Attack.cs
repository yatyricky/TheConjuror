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
        Payload data = new Payload
        {
            ActionName = GetType().Name,
            payload = attacker.Attack(defender, cardSlot)
        };
        callBack(data);
    }

    public class AttackResult
    {
        public List<Card> Attacker;
        public List<Card> Defender;
        public int AttackerSlotId;
        public int DefenderSlotId;
        public List<Tuple<int, int>> AttackerModifiers;
        public List<Tuple<int, int>> DefenderModifiers;

        public int SumModifiers(List<Tuple<int, int>> list)
        {
            int sum = 0;
            foreach(Tuple<int, int> entry in list)
            {
                sum += entry.Value1;
            }
            return sum;
        }
    }
}
