using UnityEngine;

public abstract class DraggingActions : MonoBehaviour
{

    public abstract void OnStartDrag();

    public abstract void OnEndDrag();

    public abstract void OnDraggingInUpdate();

    protected bool dragabble;

    public virtual bool CanDrag
    {
        get
        {
            return dragabble;
        }
    }

    protected abstract bool DragSuccessful();
}
