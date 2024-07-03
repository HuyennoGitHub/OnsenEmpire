using IPS;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private void Awake()
    {
        this.AddListener<EventDefine.OnEnterRoom>(ChangeCamInRoom);
        this.AddListener<EventDefine.OnExitRoom>(ChangeCamOutRoom);
    }
    private void OnEnable()
    {
        var player = FindObjectOfType<PlayerInputCtrl>();
        if (player != null)
        {
            if (player.inRoom)
            {
                ChangeCam(true, player.viewSide == PlayerInputCtrl.Side.Left);
            }
            else
            {
                ChangeCam(false);
            }
        }
    }
    private void ChangeCamInRoom(EventDefine.OnEnterRoom param)
    {
        ChangeCam(true, param.leftSideRoom);
    }
    private void ChangeCamOutRoom()
    {
        ChangeCam(false);
    }
    public void ChangeCam(bool inRoom, bool leftside = false)
    {
        if (inRoom)
        {
            if (leftside)
            {
                transform.rotation = Quaternion.Euler(50, -45, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(50, 45, 0);
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(50, 0, 0);
        }
    }
}
