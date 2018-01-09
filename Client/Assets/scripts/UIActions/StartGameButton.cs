using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StartGameButton : MonoBehaviour
{
    public Text InputText;

    // Use this for initialization
    void Start()
    {
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        NetworkController.instance.LoginPlayer(InputText.text);
    }

}
