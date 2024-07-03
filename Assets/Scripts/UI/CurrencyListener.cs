using IPS;
using System.Collections;
using TMPro;
using UnityEngine;

public class CurrencyListener : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private float animDuration;
    private int currencyCache;

    private void Start()
    {
        this.AddListener<EventDefine.OnCashChanged>(OnCurrencyChanged);
        currencyCache = UserData.CurrentCash;
        UserData.InitShowingCash();
        UpdateVisual();
    }

    private void OnCurrencyChanged()
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (currencyCache >= UserData.CurrentShowingCash)
        {
            currencyCache = UserData.CurrentShowingCash;
            currencyText.text = UserData.CurrentShowingCash.ToString();
            return;
        }
        StopAllCoroutines();
        StartCoroutine(IECounting(currencyText, currencyCache, UserData.CurrentShowingCash));
    }

    private IEnumerator IECounting(TextMeshProUGUI text, int from, int to)
    {
        if (from == to) yield break;

        float duration = animDuration;
        float elapse = 0;

        while (elapse < duration && currencyCache < to)
        {
            elapse += Time.deltaTime;
            currencyCache = (int)(Mathf.Lerp(from, to, elapse));
            text.text = currencyCache.ToString();
            yield return null;
        }

        currencyCache = to;
        text.text = to.ToString();
    }
}
