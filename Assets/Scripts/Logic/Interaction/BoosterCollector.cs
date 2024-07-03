using UnityEngine;

public class BoosterCollector : ACollector
{
    private bool collected;
    [SerializeField] private PlayerInputCtrl player;

    private void Start()
    {
        collected = false;
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(BoosterCollectable.BoosterCollectableTag))
        {
            if (player.IsMoving) return;
            base.OnTriggerEnter(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(BoosterCollectable.BoosterCollectableTag) && collected) collected = false;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(BoosterCollectable.BoosterCollectableTag))
        {
            if (!collected && !GetComponent<PlayerInputCtrl>().IsMoving)
            {
                collected = true;
                base.OnTriggerEnter(other);
            }
        }
    }
    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(BoosterCollectable.BoosterCollectableTag))
        {
            if (player.IsMoving) return;
            base.OnCollisionEnter(collision);
        }
    }
    public void SpendGold(int value)
    {
        //minus gold
    }
    public void CollectMoney(int value)
    {
        UserData.SetCash(UserData.CurrentCash + value);
        UserData.SetFakeCash(UserData.CurrentShowingCash + value);
    }
    public void SpeedUp(int value, float time)
    {
        player.SpeedUp(value / 100f, time);
    }
}
