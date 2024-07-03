public partial class UpgradeCtrl
{
    private Area area;
    public void HireCleaner()
    {
        if (area == null) area = GetComponent<Area>();
        area.HireCleaner();
    }
    public void UpgradeCleaner()
    {
        if (area == null) area = GetComponent<Area>();
        area.UpgradeCleaner();
    }
    public Cleaner GetCleaner()
    {
        if (area == null) area = GetComponent<Area>();
        return area.GetCleaner();
    }
    public void ExpandRoom(Room room)
    {
        if (area == null) area = GetComponent<Area>();
        area.ExpandRoom(room);
    }
    public void UpgradeRoom(Room room, uint optionVisual)
    {
        if (area == null) area = GetComponent<Area>();
        area.UpgradeRoom(room, optionVisual);
    }
    public void ExpandToilet()
    {
        if (area == null) area = GetComponent<Area>();
        area.ExpandSauna();
    }
    private Storage storage;
    public Loader GetLoaderToUpgrade()
    {
        if (storage == null) storage = GetComponent<Storage>();
        return storage.GetUpgradeableLoader();
    }
    public void HireMoreLoader()
    {
        if (storage == null) storage = GetComponent<Storage>();
        storage.HireLoader();
    }
    public void UpgradeLoader()
    {
        if (storage == null) storage = GetComponent<Storage>();
        storage.UpgradeLoader();
    }
    private Restaurant restaurant;
    public void HireWaiter()
    {
        if (restaurant == null) restaurant = GetComponent<Restaurant>();
        restaurant.HireWaiter();
    }
    private Reception reception;
    public void HireReceptionist()
    {
        if (reception == null) reception = GetComponent<Reception>();
        reception.HireMoreReceptionist();
    }
}
