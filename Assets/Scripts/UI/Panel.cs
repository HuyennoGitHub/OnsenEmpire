using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Panel : MonoBehaviour
{
    public Image background;
    public Transform mainPanel;
    protected virtual void OnEnable()
    {
        Show();
    }
    protected void Hide()
    {
        if (background != null)
        {
            background.DOFade(0, .3f).SetUpdate(true).OnComplete(() =>
            {
                background.gameObject.SetActive(false);
            });
        }
        if (mainPanel != null)
        {
            mainPanel.DOKill();
            mainPanel.DOScale(.5f, .3f).SetUpdate(true).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        } else gameObject.SetActive(false);
    }
    protected void Show()
    {
        if (background != null)
        {
            background.gameObject.SetActive(true);
            background.DOFade(0.5f, .3f).From(0).SetUpdate(true);
        }
        if (mainPanel != null)
        {
            mainPanel.DOKill();
            mainPanel.DOScale(1, .3f).From(.5f).SetUpdate(true);
        }
    }
}
