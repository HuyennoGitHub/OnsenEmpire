using System.Collections;
using UnityEngine;
using TMPro;
using IPS;

public class UpgradeRoom : AUpgrade
{
    public Room room;
    public int nextLv;
    [SerializeField] TextMeshProUGUI lvText;
    private bool isChoosing;
    private uint chosenOption;
    private int sakuraRewardFree;


    protected override void Start()
    {
        base.Start();
        string id = room.Info.Id;
        int cost = GameData.Instance.GetObject(id, nextLv).UnlockCost;
        lvText.text = $"Lv.{nextLv}";
        SetUpgradeInfo(type, cost);
    }
    public override void OnCompleted()
    {
        this.AddListener<EventDefine.OnChosenVisualOption>(CloseOptionPanel);
        isChoosing = true;
        StartCoroutine(ChooseVisualRoom());
    }
    IEnumerator ChooseVisualRoom()
    {
        //mapCtrl.tutCam.MoveToShowRoom(room);
        UICtrl.Instance.Get<UpgradeRoomPanel>().SetLevelTitleText(room.Info.Level + 1);
        // Set tung option cho panel
        LevelInfo info = GameData.Instance.GetObject(room.Info.Id, room.Info.Level + 1);
        UpgradeRoomPanel panel = UICtrl.Instance.Get<UpgradeRoomPanel>();
        sakuraRewardFree = (room.Info.Level + 1) * 2 - 1;
        panel.SetOption1(info.GetAvatar2D(0), sakuraRewardFree, 1);
        panel.SetOption2(info.GetAvatar2D(1), sakuraRewardFree, 1);
        panel.SetOptionVIP(info.GetAvatar2D(2), sakuraRewardFree + 2, 1, 5);
        UICtrl.Instance.Show<UpgradeRoomPanel>();
        while (isChoosing)
        {
            yield return new WaitForEndOfFrame();
        }
        UserData.SetCash(UserData.CurrentCash - InitMoney);
        //room.Upgrade(chosenOption);
        upgradeCtrl.UpgradeRoom(room, chosenOption);
        this.Dispatch<EventDefine.OnUpgradeDone>();
        gameObject.SetActive(false);
        upgradeCtrl.ShowNextUpgradeRoomBox();
        //base.OnCompleted();
        Invoke(nameof(ReturnPlayerView), 0.75f);
    }
    private void CloseOptionPanel(EventDefine.OnChosenVisualOption param)
    {
        isChoosing = false;
        int rewardSakura;
        chosenOption = param.chosenOption;
        if (param.chosenOption == 1)
        {
            rewardSakura = sakuraRewardFree;

        }
        else if (param.chosenOption == 2)
        {
            rewardSakura = sakuraRewardFree;
        }
        else
        {
            rewardSakura = sakuraRewardFree + 2;
        }
        this.Dispatch(new EventDefine.RewardSakura { sakura = rewardSakura });
        // xu li visual theo option
    }
    private void ReturnPlayerView()
    {
        //mapCtrl.tutCam.ChangeView();
    }

    public override bool CheckToShow()
    {
        throw new System.NotImplementedException();
    }
}
