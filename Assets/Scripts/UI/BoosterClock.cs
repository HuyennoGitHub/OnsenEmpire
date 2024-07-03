using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BoosterClock : MonoBehaviour
{
    [SerializeField] Image timeImage;
    [SerializeField] float timeS;

    private float remainTime;
    public void AddTime(float time)
    {
        remainTime += time;
    }
    public void StartCounting()
    {
        StartCoroutine(OnCounting());
    }
    public void StopCounting()
    {
        StopCoroutine(OnCounting());
    }
    IEnumerator OnCounting()
    {
        while (remainTime > 0)
        {
            yield return new WaitForEndOfFrame();
            remainTime -= Time.deltaTime;
            timeImage.fillAmount = remainTime / timeS;
        }
        EndCounting();
    }
    private void EndCounting()
    {
        gameObject.SetActive(false);
    }
}
