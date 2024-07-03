using DG.Tweening;
using UnityEngine;

public class SortMoneyInContainer : MonoBehaviour
{
    private int currentCol, currentRow, currentHigh;

    [SerializeField] int colMax, rowMax, highMax;
    [SerializeField] float colDistance;
    [SerializeField] float rowDistance;
    [SerializeField] float highDistance;

    private int cashTotal;
    private void Start()
    {
        Init();
        cashTotal = 0;
    }
    public void Sort(GameObject cashObj, CashObject money)
    {
        Vector3 putPos = new Vector3(currentCol * colDistance, currentHigh * highDistance, currentRow * rowDistance);
        cashObj.transform.DOLocalMove(putPos, .5f);
        currentRow++;
        if (currentRow == rowMax)
        {
            currentRow = 0;
            currentCol++;
            if (currentCol == colMax)
            {
                currentCol = 0;
                currentHigh++;
                if (currentHigh == highMax)
                {
                    currentHigh = 0;
                }
            }
        }
        AddCash(money.Value);
        money.SetValue(0);
    }
    private void AddCash(int value)
    {
        cashTotal += value;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Init();
            CollectAllMoney(other.transform.position);
        }
    }
    public void CollectAllMoney(Vector3 player)
    {
        Init();
        Transform[] cashs = GetComponentsInChildren<Transform>();
        for (int i = 1; i < cashs.Length; i++)
        {
            cashs[i].DOMove(player, .5f);
        }
        UserData.SetCash(UserData.CurrentCash + cashTotal);
        UserData.SetFakeCash(UserData.CurrentShowingCash + cashTotal);
        cashTotal = 0;
    }

    public void Init()
    {
        currentCol = 0;
        currentRow = 0;
        currentHigh = 0;
    }
}
