using System.Collections;

public class Card1000023 : CardAbility
{
    internal override int GetAttackModifier(bool attacker)
    {
        if (Card == null)
        {
            throw new System.Exception("No card initialized in CardAbility");
        }
        if (attacker)
        {
            return 0 - Card.Power;
        }
        else
        {
            return 0;
        }
    }
}
