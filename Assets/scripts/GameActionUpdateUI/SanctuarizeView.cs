public class SanctuarizeView : GameActionUpdateUIView
{

    public override void DoAction()
    {
        Sanctuarize.SanctuarizeData payload = (Sanctuarize.SanctuarizeData)Payload;
        BuffBehaviour.Create(payload.card.COB.gameObject, payload.buff);
    }
}
