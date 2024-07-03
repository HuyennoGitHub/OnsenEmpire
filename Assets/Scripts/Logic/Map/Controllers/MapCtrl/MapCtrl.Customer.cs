using UnityEngine;

public partial class MapCtrl
{
    [SerializeField] CustomerManager customerManager;
    private Service FindService(bool isVIP)
    {
        if (isVIP)
        {
           return Service.None;
        }
        if (servableServices.Count == 0)
        {
            return Service.None;
        }
        int randomService = Random.Range(0, servableServices.Count);
        return servableServices[randomService];
    }
    public Service ArrangeCustomerToService(Customer customer)
    {
        Service chosenService = FindService(false);
        Area area = customer.usingRoom.area;
        switch (chosenService)
        {
            case Service.Sauna:
                if (area.CallCustomerToSauna(customer))
                {
                    OrderCustomerToSauna(area, customer);
                    return chosenService;
                }
                else return Service.None;
            case Service.Restaurant:
                if (restaurant.ProcessCustomerHaveMealRequest(customer))
                {
                    Vector3 target = restaurant.GetWaitingSlot(customer);
                    Transform lookAtTarget = restaurant.lookAtTarget;
                    customerManager.CallCustomerToRestaurant(customer, target, lookAtTarget);
                    return chosenService;
                }
                else return Service.None;
            default:
                return Service.None;
        }
    }
    private void OrderCustomerToSauna(Area area, Customer customer)
    {
        Vector3 target = area.GetWaitingSauanSlot(customer);
        Transform lookAtTarget = area.GetLookAtSaunaTarget();
        customerManager.CallCustomerToSauna(customer, target, lookAtTarget);
    }
}
