using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MessageBehaviour : MonoBehaviour
{
    public Text Connecting;
    public Text Waiting;

    public const int CONNECTING = 100;
    public const int WAITING = 200;

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show(int whichMessage)
    {
        gameObject.SetActive(true);
        Connecting.enabled = false;
        Waiting.enabled = false;

        switch (whichMessage)
        {
            case CONNECTING:
                Connecting.enabled = true;
                break;
            case WAITING:
                Waiting.enabled = true;
                break;
            default:
                break;
        }
    }

}
