using DG.Tweening;
using IPS;
using UnityEngine;

public class SakuraRewardManager : MonoBehaviour
{
    [SerializeField] GameObject starPoolingObject;
    [SerializeField] uint maxSakura;
    [SerializeField] Vector3[] initPos;
    [SerializeField] Quaternion[] initRot;
    [SerializeField] RectTransform destination;

    private void Start()
    {
        this.AddListener<EventDefine.RewardSakura>(RewardSakura);
        initPos = new Vector3[maxSakura];
        initRot = new Quaternion[maxSakura];

        for (int i = 0; i < starPoolingObject.transform.childCount; i++)
        {
            initPos[i] = starPoolingObject.transform.GetChild(i).position;
            initRot[i] = starPoolingObject.transform.GetChild(i).rotation;
        }
    }
    private void ResetSakura()
    {
        for (int i = 0; i < starPoolingObject.transform.childCount; i++)
        {
            starPoolingObject.transform.GetChild(i).SetPositionAndRotation(initPos[i], initRot[i]);
        }
    }
    public void RewardSakura(EventDefine.RewardSakura param)
    {
        int sakura = param.sakura;
        ResetSakura();
        int receive = (int)Mathf.Clamp(sakura, 0, maxSakura);
        float delay = 0;
        for (int i = 0; i < receive; i++)
        {
            Transform showObj = starPoolingObject.transform.GetChild(i);
            showObj.gameObject.SetActive(true);
            showObj.DOScale(1f, 0.1f).SetDelay(delay).SetEase(Ease.OutBack);
            showObj.GetComponent<RectTransform>().DOAnchorPos(destination.anchoredPosition, 0.8f).SetDelay(delay + 0.2f)
                .SetEase(Ease.InBack).OnComplete(() => {
                    UserData.AddSakura(1);
                });
            showObj.DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f);
            showObj.DOScale(0f, 0.1f).SetDelay(delay + 1.2f).SetEase(Ease.OutBack);
            delay += 0.1f;
        }
    }
}
