using System;
using UnityEngine;
using IPS;

public class EventDefine
{
    public struct OnStartGame : IEventParam { }
    public struct OnEndGame : IEventParam { }
    public struct OnUpLevel : IEventParam { }
    public struct OnSakuraChanged : IEventParam { }
    public struct NeedRefillEvent : IEventParam
    {
        public RefillByAmount needRefill;
        public Vector3 pos;
        public RefillObjectType type;
    }
    public struct NeedCleanEvent : IEventParam
    {
        public Room needCleanRoom;
        public string areaId;
    }
    public struct OrderToCallShowBoosterOffer : IEventParam { public BoosterType type; }
    public struct OpenBoosterOffer : IEventParam { }
    public struct OnHavingCustomer : IEventParam { public Customer customer; public Receptionist receptionist; }
    public struct OnHavingEmptySlot : IEventParam { public Vector3 pos; public Receptionist receptionist; }
    public struct ServedOrder : IEventParam
    {
        public ShokuTable table;
        public RefillObjectType foodType;
        public Waiter waiter;
    }
    public struct HaveOrder : IEventParam
    {
        public ShokuTable table;
        public int vegetable;
        public int meat;
    }
    public struct OnChosenVisualOption : IEventParam { public uint chosenOption; }
    public struct RewardSakura : IEventParam { public int sakura; }
    public struct OnExpandedArea : IEventParam { public string areaId; }
    public struct CanCallOrder : IEventParam { }
    public struct StartUpgradeRoom : IEventParam { }
    public struct OnUpgradeRoomDone : IEventParam { }
    public struct OnUpgradeDone : IEventParam { }
    public struct LoadMissionCompleteEvent : IEventParam { public Loader loader; }
    public struct BringingEvent : IEventParam { }
    public struct NotBringEvent : IEventParam { }
    public struct CleanEvent : IEventParam { public Room inRoom; }
    public struct CanUseEvent : IEventParam { public Room room; }
    public struct OrderingSake : IEventParam { }
    public struct DoneSakeOrder : IEventParam { }
    public struct OnEnterRoom : IEventParam { public bool leftSideRoom; }
    public struct OnExitRoom : IEventParam { }
    public struct OnCashChanged : IEventParam { }
}
