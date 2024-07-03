using UnityEngine;

public class Upgrader : MonoBehaviour
{
    [SerializeField] GameObject dropMoneyVfx;
    [SerializeField] AudioClip upgradedSound;
    [SerializeField] AudioClip upgradingSound;

    private bool isPaying;

    private void Start()
    {
        isPaying = false;
    }

    private void StartInteract(Collider other)
    {
        if (isPaying) return;
        isPaying = true;
        var obj = other.GetComponent<AUpgrade>();
        if (obj != null && UserData.CurrentCash > 0)
        {
            obj.StartUpgrade();
            PlayDropMoneyVfx();
            SFX.Instance.PlaySound(upgradingSound, true);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other == null) return;
        if (GetComponent<PlayerInputCtrl>().IsMoving)
        {
            isPaying = false;
            StopDropMoneyVfx();
            SFX.Instance.StopAudio();
            return;
        }
        if (other.CompareTag(AUpgrade.UpgradeableTag) && UserData.CurrentCash > 0)
        {
            var obj = other.GetComponent<AUpgrade>();
            if (!isPaying && obj.MoneyRemaining > 0)
            {
                StartInteract(other);
            }
            if (obj.MoneyRemaining == 0)
            {
                StopDropMoneyVfx();
                isPaying = false;
                return;
            }
            UpdateDropMoneyVFXRotation(other.transform);
            int spend;
            if (UserData.CurrentCash > obj.MoneyRemaining)
            {
                if (obj.MoneyRemaining > (uint)(obj.InitMoney / 3 * Time.deltaTime + 1))
                {
                    spend = (int)(obj.InitMoney / 3 * Time.deltaTime + 1);
                }
                else
                {
                    spend = obj.MoneyRemaining;
                }
            }
            else
            {
                if (UserData.CurrentCash > (uint)(obj.InitMoney / 3 * Time.deltaTime + 1))
                {
                    spend = (int)(obj.InitMoney / 3 * Time.deltaTime + 1);
                }
                else
                {
                    spend = (int)UserData.CurrentCash;
                }
            }
            if (obj != null)
            {
                obj.OnUpgrading(spend);
            }
            var collector = GetComponent<MoneyCollector>();
            if (collector != null)
            {
                collector.SpendMoney(spend);
            }
            if (UserData.CurrentShowingCash % 30 == 0)
            {
                SFX.Instance.Vibrate();
            }
            if (obj.MoneyRemaining <= 0 || UserData.CurrentCash == 0)
            {
                StopDropMoneyVfx();
                isPaying = false;
                SFX.Instance.StopAudio();
                if (obj.MoneyRemaining <= 0)
                {
                    SFX.Instance.PlaySound(upgradedSound);   
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!isPaying) return;
        isPaying = false;
        StopDropMoneyVfx();
        SFX.Instance.StopAudio();
    }
    private void PlayDropMoneyVfx()
    {
        if (dropMoneyVfx != null)
        {
            dropMoneyVfx.SetActive(true);
        }
    }
    public void StopDropMoneyVfx()
    {
        if (dropMoneyVfx != null)
        {
            dropMoneyVfx.SetActive(false);
        }
    }
    private void UpdateDropMoneyVFXRotation(Transform target)
    {
        dropMoneyVfx.transform.LookAt(target);
        float realDis = (transform.position - target.position).magnitude;
        float x = Mathf.Rad2Deg * Mathf.Asin(realDis / Mathf.Sqrt(2));
        dropMoneyVfx.transform.rotation = Quaternion.Euler(x, dropMoneyVfx.transform.rotation.eulerAngles.y, dropMoneyVfx.transform.rotation.eulerAngles.z);
    }
}
