using IPS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : Bot, IInteractable
{
    public List<EventDefine.NeedRefillEvent> missions = new();
    public Transform visualGraphic;

    [SerializeField] Refiller myRefiller;
    [SerializeField] RefillCollector myRefillCollector;
    [SerializeField] Transform lookAtTransform;
    [SerializeField] StaffData loaderDataSO;
    [SerializeField] SkinnedMeshRenderer skin;

    public Transform towelStore;
    public Transform relaxPos;
    public ObjectData Info;
    public GameObject upgradeVFX;
    public Recycle recycle;

    public int MaxTarget => myRefillCollector.MaxTarget;

    private List<Vector3> collectPoss = new();
    public bool IsInteractEnable => throw new System.NotImplementedException();
    private int index = -1;
    private BoxCollider boxCollider;
    public BoxCollider BoxCollider
    {
        get
        {
            if (boxCollider != null) { return boxCollider; }
            else
            {
                boxCollider = GetComponent<BoxCollider>();
                return boxCollider;
            }
        }
    }
    protected override void Start()
    {
        base.Start();
        ShowVFX(true);
        myRefillCollector.MaxTarget = loaderDataSO.maxBringAmount[Info.Level];
        Vector3 tmp = relaxPos.position;
        tmp.y = transform.position.y;
        relaxPos.position = tmp;
        tmp = towelStore.position;
        tmp.y = transform.position.y;
        towelStore.position = tmp;
        DisableInteract();
        destination = relaxPos.position;
    }
    private void ShowVFX(bool show = false)
    {
        upgradeVFX.SetActive(show);
    }
    private void LateUpdate()
    {
        if (CalculateDistance(destination, transform.position) < 0.01f)
        {
            EnableInteract();
            if (myRefillCollector.CollectedTotal == 0)
            {
                animator.Play("Idle");
            }
            else
            {
                animator.Play("Grab");
            }
            if (CalculateDistance(destination, relaxPos.position) < 0.01f)
            {
                transform.LookAt(lookAtTransform);
            }
        }
        else
        {
            DisableInteract();
            if (myRefillCollector.CollectedTotal == 0)
            {
                animator.Play("Walking");
            }
            else
            {
                animator.Play("GoGrab");
            }
        }
    }
    public void StartMission()
    {
        collectPoss = new();
        foreach (var mission in missions)
        {
            if (mission.needRefill.NeedType == RefillObjectType.Towel)
            {
                collectPoss.Add(towelStore.position);
            }
        }
        index = 0;
        StartCoroutine(nameof(RunMission));
    }

    IEnumerator RunMission()
    {
        while (myRefillCollector.CollectedTotal < collectPoss.Count && myRefillCollector.CollectedTotal < myRefillCollector.MaxTarget)
        {
            MoveTo(collectPoss[index]);
            if (myRefillCollector.CollectedTotal > index)
            {
                index++;
            }
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(RunMission2());
    }
    IEnumerator RunMission2()
    {
        index = -1;
        animator.Play("GoGrab");
        while (missions.Count > 0 && myRefillCollector.CollectedTotal > 0)
        {
            MoveTo(missions[0].pos);
            if (!missions[0].needRefill.NeedRefill)
            {
                CompleteMission();
            }
            while (missions.Count > 0 && myRefillCollector.GetObject(missions[0].type) == null)
            {
                if (myRefillCollector.CollectedTotal < myRefillCollector.MaxTarget)
                {
                    StartMission();
                    //CollectMore(missions[0]);
                }
                else
                {
                    ThrowAll();
                }
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
        }
        if (myRefillCollector.CollectedTotal == 0 && missions.Count > 0)
        {
            destination = relaxPos.position;
            animator.Play("Walking");
            foreach (var mission in missions)
            {
                this.Dispatch(mission);
            }
            missions.Clear();
            Invoke(nameof(OnMissionCompleted), 3);
        }
    }
    public void CompleteMission()
    {
        missions.RemoveAt(0);
        if (missions.Count == 0)
        {
            destination = relaxPos.position;
            if (myRefillCollector.CollectedTotal == 0)
            {
                animator.Play("Walking");
            }
            else
            {
                animator.Play("GoGrab");
            }
            Invoke(nameof(OnMissionCompleted), 3);
        }
    }
    public void ThrowAll()
    {
        MoveTo(recycle.RefillPos);
        StartCoroutine(WaitToThrowAll());
    }
    IEnumerator WaitToThrowAll()
    {
        while (myRefillCollector.CollectedTotal > 0)
        {
            yield return new WaitForEndOfFrame();
        }
        StartMission();
    }
    public void CollectMore(EventDefine.NeedRefillEvent mission)
    {
        RefillObjectType objType = mission.type;
        if (objType == RefillObjectType.Towel)
        {
            MoveTo(towelStore.position);
        }
    }

    private void OnMissionCompleted()
    {
        this.Dispatch(new EventDefine.LoadMissionCompleteEvent { loader = this });
    }

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
        ShowVFX(true);
        if (Info.Level + 1 > 3)
        {
            Info.SetLevel(3);
            return;
        }
        else Info.SetLevel(Info.Level + 1);
        myRefillCollector.MaxTarget = loaderDataSO.maxBringAmount[Info.Level];
        LoadVisual();
        GameData.Instance.GetObject(Info.Id, Info.Level).Unlock();
        UserData.Inventory.SaveItem(new ObjectData(Info.Id, ObjectType.Loader, Info.Level), true);
    }
    public bool IsUnlocked => !Info.IsNull && Info.Unlocked;

    public void UnlockLevel(int lv)
    {
        gameObject.SetActive(true);
        for (int i = 1; i <= lv; i++)
        {
            GameData.Instance.GetObject(Info.Id, i).Unlock();
        }
        Info.SetLevel(lv);
        LoadVisual();
    }
    private void LoadVisual()
    {
        Material visual = loaderDataSO.skin[Info.Level];
        skin.material = visual;
    }
}
