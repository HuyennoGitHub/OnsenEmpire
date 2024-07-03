using UnityEngine;

public class CashObject : ACollectable
{
    [SerializeField] int value;

    public int Value => value;
    public void SetValue(int value)
    {
        this.value = value;
    }

    public override bool OnCollected(ICollector collector)
    {
        if (!(collector is IMoneyCollector)) return false;
        if (base.OnCollected(collector))
        {
            if (collector is IMoneyCollector)
            {
                (collector as IMoneyCollector).CollectMoney(value);
                if (SFX.Instance.SoundEnable) transform.GetComponent<AudioSource>().Play();
                SFX.Instance.Vibrate();
            }
        }
        return true;
    }
}
