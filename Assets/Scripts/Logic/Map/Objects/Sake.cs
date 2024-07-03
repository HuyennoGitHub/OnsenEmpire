using IPS;
using UnityEngine;

public class Sake : MonoBehaviour
{
    [SerializeField] BoxCollider interactCollider;
    public GameObject interactArea;

    private void Start()
    {
        this.AddListener<EventDefine.OrderingSake>(EnableInteract);
        this.AddListener<EventDefine.DoneSakeOrder>(DisableInteract);
    }
    private void OnEnable()
    {
        Reception reception = FindObjectOfType<Reception>();
        if (reception.specialOrderCustomer != null && !reception.specialOrderCustomer.IsServed)
        {
            EnableInteract();
        }
        else DisableInteract();
    }
    private void EnableInteract()
    {
        interactCollider.enabled = true;
        interactArea.SetActive(true);
    }
    public void DisableInteract()
    {
        interactCollider.enabled = false;
        interactArea.SetActive(false);
    }
}
