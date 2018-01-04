public class NoBattleBuff : Buff
{
    public NoBattleBuff() : base()
    {
        iconPath = "no_battle";
        type = BuffType.NO_BATTLE;
    }

    internal override bool ShouldRemoveEndTurn()
    {
        return true;
    }
}
