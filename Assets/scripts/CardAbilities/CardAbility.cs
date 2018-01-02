public class CardAbility
{
    public Card Card;

    internal virtual int GetAttackModifier(bool attacker)
    {
        return 0;
    }
}
