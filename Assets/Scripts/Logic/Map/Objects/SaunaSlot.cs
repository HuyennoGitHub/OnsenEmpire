using UnityEngine;

public class SaunaSlot : MonoBehaviour, IRoom
{
    [SerializeField] RefillByAmount refillByAmount;
    public Transform usePos;

    public bool CanUse => refillByAmount.RemainAmount > 0 && !Using;

    public Vector3 UsePos => usePos != null ? usePos.position : transform.position;

    public ServiceType ServiceOfType => ServiceType.Sauna;

    public bool Using { get; set; } = false;
    public void FreeSlot()
    {
        Using = false;
    }
}
