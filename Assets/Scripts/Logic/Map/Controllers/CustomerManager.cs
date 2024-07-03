using IPS;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public MapCtrl mapCtrl;

    [Header("Spawn")]
    public Transform spawnCustomerPos;
    public GameObject[] customerPrefab;

    [Header("Tutorial")]
    public TutorialCamFollow tutCam;

    private void Start()
    {
        this.AddListener<EventDefine.OnHavingEmptySlot>(SpawnCustomer);
    }
    public void SpawnCustomer(EventDefine.OnHavingEmptySlot param)
    {
        if (param.pos != Vector3.zero)
        {
            SpawnCustomer(param.pos, param.receptionist);
            return;
        }
        int rand = Random.Range(0, customerPrefab.Length);
        GameObject newCustomer = Instantiate(customerPrefab[rand], spawnCustomerPos.position, Quaternion.LookRotation(Vector3.forward), transform);
        var customer = newCustomer.GetComponent<Customer>();
        customer.manager = this;
        this.Dispatch(new EventDefine.OnHavingCustomer { customer = customer, receptionist = param.receptionist });
    }

    public void SpawnCustomer(Vector3 spawnPos, Receptionist receptionist)
    {
        int rand = Random.Range(0, customerPrefab.Length);
        GameObject newCustomer = Instantiate(customerPrefab[rand], spawnPos, Quaternion.LookRotation(Vector3.forward), transform);
        var customer = newCustomer.GetComponent<Customer>();
        customer.manager = this;
        this.Dispatch(new EventDefine.OnHavingCustomer { customer = customer, receptionist = receptionist });
    }

    public void FindNextService(Customer customer)
    {
        Service foundService = mapCtrl.ArrangeCustomerToService(customer);
        if (foundService == Service.None) customer.GoHome();
    }
    public void CallCustomerToRestaurant(Customer customer, Vector3 target, Transform lookAtTarget)
    {
        customer.GoToRestaurant(target, lookAtTarget);
    }
    public void CallCustomerToSauna(Customer customer, Vector3 target, Transform lookAtTarget)
    {
        customer.GoToSauna(target, lookAtTarget);
    }
}
