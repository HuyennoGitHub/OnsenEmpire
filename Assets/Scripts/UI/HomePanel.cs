using UnityEngine.UI;
using UnityEngine;
using IPS;

public class HomePanel : Panel
{
    [SerializeField] Button startButton;
    protected override void OnEnable()
    {
        startButton.onClick.AddListener(OnClickStartButton);
    }
    private void OnClickStartButton()
    {
        Hide();
        this.Dispatch<EventDefine.OnStartGame>();
    }
}
