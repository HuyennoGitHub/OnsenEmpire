using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cleaner : Bot, IInteractable
{
    public Transform visualGraphic;
    public GameObject myTool;
    public List<Room> missions = new();
    [SerializeField] StaffData cleanerDataSO;
    [SerializeField] Refiller myRefiller;
    [SerializeField] Transform lookAtTransform;
    public ObjectData Info;
    public SkinnedMeshRenderer[] skinnedMeshRenderers;
    private BoxCollider boxCollider;
    private BoxCollider BoxCollider
    {
        get
        {
            if (boxCollider != null) return boxCollider;
            boxCollider = GetComponent<BoxCollider>();
            return boxCollider;
        }
    }

    public float doTaskTime;
    public Transform relaxPos;
    public GameObject upgradeVFX;
    public uint region;
    private Room mission;

    protected override void Start()
    {
        base.Start();
        ShowVFX();
        Invoke(nameof(HideVFX), 2);
        myTool.SetActive(true);
        myTool.transform.parent = transform.parent;
        doTaskTime = cleanerDataSO.doTaskTime[Info.Level];
        destination = relaxPos.position;
        //OnMissionCompleted();
        StartCoroutine(StartMission());
    }
    private void ShowVFX()
    {
        upgradeVFX.SetActive(true);
    }
    private void HideVFX()
    {
        upgradeVFX.SetActive(false);
    }
    private void LateUpdate()
    {
        if (CalculateDistance(destination, relaxPos.position) < 0.1f)
        {
            if ((destination - transform.position).sqrMagnitude < 0.1f)
            {
                animator.Play("Idle");
                transform.LookAt(lookAtTransform);
            }
        }
    }
    public void AddMission(Room mission)
    {
        Room found = missions.Find(r => r.Info.Id.Equals(mission.Info.Id));
        if (found == null)
        {
            missions.Add(mission);
        }
    }
    IEnumerator StartMission()
    {
        while (true)
        {
            while (missions.Count == 0)
            {
                destination = relaxPos.position;
                yield return new WaitForEndOfFrame();
            }
            mission = missions[0];
            missions.Remove(mission);
            StartCoroutine(DoClean());
            while (mission != null)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
    IEnumerator DoClean()
    {
        while (!CheckMissionComplete())
        {
            var dirtyObj = mission.RefillTarget;
            animator.Play("Walking");
            Vector3 des = dirtyObj.RefillPos;
            des.y = 0;
            MoveTo(des);
            while (CalculateDistance(destination, transform.position) > 0.01f)
            {
                if (!dirtyObj.NeedRefill) break;
                yield return new WaitForEndOfFrame();
            }
            if (dirtyObj.NeedRefill)
            {
                animator.Play("Clean");
                Invoke(nameof(EnableInteract), 1f);
            }
            while (dirtyObj.NeedRefill)
            {
                yield return new WaitForEndOfFrame();
            }
            animator.Play("Walking");
            DisableInteract();
        }
        MoveTo(relaxPos.position);
        animator.Play("Walking");
        mission = null;
        //Invoke(nameof(OnMissionCompleted), 1);
    }
    public bool CheckMissionComplete()
    {
        if (mission.RefillTarget != null) return false;
        mission = null;
        return true;
    }
    //private void OnMissionCompleted() {
    //    this.Dispatch(new AssignedRoomCleanEvent { cleaner = this });
    //}

    public bool IsInteractEnable => throw new System.NotImplementedException();

    public void DisableInteract()
    {
        BoxCollider.enabled = false;
    }

    public void EnableInteract()
    {
        BoxCollider.enabled = true;
    }

    public void OnInteract(IInteractable other)
    {
    }
    public void Upgrade()
    {
        ShowVFX();
        Invoke(nameof(HideVFX), 2);
        Info.SetLevel(Info.Level + 1);
        doTaskTime = cleanerDataSO.doTaskTime[Info.Level];
        LoadVisual();
        GameData.Instance.GetObject(Info.Id, Info.Level).Unlock();
        UserData.Inventory.SaveItem(new ObjectData(Info.Id, ObjectType.Cleaner, Info.Level), true);
    }
    public bool IsUnlocked => !Info.IsNull && Info.Unlocked;

    public void UnlockLevel(int lv = 1, uint visualOption = 1)
    {
        gameObject.SetActive(true);
        for (int i = 1; i <= lv; i++)
        {
            GameData.Instance.GetObject(Info.Id, i).Unlock();
        }
        //if (lv == Info.Level) return;
        Info.SetLevel(lv);
        LoadVisual();
    }
    private void LoadVisual()
    {
        Material visual = cleanerDataSO.skin[Info.Level];
        foreach (var skin in skinnedMeshRenderers)
        {
            skin.material = visual;
        }

    }
}
