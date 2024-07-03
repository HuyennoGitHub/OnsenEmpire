using UnityEngine;

[CreateAssetMenu(fileName = "OnSen Data", menuName = "Scriptable Object/ Economy Data", order = 0)]
public class RoomData : ScriptableObject
{
    public int checkin;
    public int tip;
    public int orderFee;
}
