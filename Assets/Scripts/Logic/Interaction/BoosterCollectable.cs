using IPS;
using System.Collections;
using UnityEngine;

public enum BoosterType { Money, Transport, Both }
public class BoosterCollectable : ACollectable
{
    public const string BoosterCollectableTag = "BoosterCollectable";
    public Transform model;
    [SerializeField] private BoosterType type;
    [Header("Text")]
    [SerializeField] string titlePopup;
    [SerializeField] string content;
    [SerializeField] float useTimeM;

    [Header("Image")]
    [SerializeField] Sprite avatar;

    private float showTimeS;
    private float showingTime;
    private BoosterCollector collector;
    private int value;

    public BoosterType Type => type;
    private void Start()
    {
        gameObject.tag = BoosterCollectableTag;
    }
    private void OnEnable()
    {
        StartCountDown();
    }
    private void OnDisable()
    {
        StartCooldown();
    }
    public override bool OnCollected(ICollector collector)
    {
        ShowPopup();
        this.collector = collector as BoosterCollector;
        return true;
    }
    private void ShowPopup()
    {
        OfferPanel panel = UICtrl.Instance.Get<OfferPanel>();
        panel.SetPanelInfo(titlePopup, content, value, useTimeM, avatar, OnRemove, this);
        UICtrl.Instance.Show<OfferPanel>();
    }
    private void OnRemove(BoosterCollectable target)
    {
        if (Type == BoosterType.Money) collector.CollectMoney(value);
        else collector.SpeedUp(value, useTimeM);
        if (target != null) gameObject.SetActive(false);
    }
    public void UseGold()
    {
        collector.SpendGold(value);
    }
    private void StartCooldown()
    {
        this.Dispatch(new EventDefine.OrderToCallShowBoosterOffer { type = Type });
    }
    private void StartCountDown()
    {
        showingTime = showTimeS;
        StartCoroutine(CountDown());
    }
    IEnumerator CountDown()
    {
        while (showingTime > 0)
        {
            showingTime -= Time.deltaTime;
            if (showingTime <= 0)
            {
                gameObject.SetActive(false);
            }
            yield return new WaitForEndOfFrame();
        }
    }
    public void LoadVisual(GameObject visual)
    {
        Destroy(model.GetChild(0).gameObject);
        Instantiate(visual, model);
    }
    public void SetValue(int valueBonus, float showTime)
    {
        value = valueBonus;
        showTimeS = showTime;
    }
}
