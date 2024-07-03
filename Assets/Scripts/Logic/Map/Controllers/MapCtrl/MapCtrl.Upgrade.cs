public partial class MapCtrl
{
    public void ExpandRestaurant()
    {
        restaurant = FindObjectOfType<Restaurant>();
        AddServableService(Service.Restaurant);
    }
    public void ExpandArea(Area newArea)
    {
        newArea.Unlock();
        newArea.Info.SetLevel(1);
    }
}
