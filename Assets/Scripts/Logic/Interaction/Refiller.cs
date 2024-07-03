using UnityEngine;

public class Refiller : ARefiller, ICollector
{
    [SerializeField] RefillCollector myCollector;

    public override void StartRefill(IRefillable other)
    {
        base.StartRefill(other);
        if (other is RefillByAmount && myCollector != null)
        {
            var refillByAmount = other as RefillByAmount;
            var obj = myCollector.GetObject(refillByAmount.NeedType);
            if (obj == null) return;
            obj.BeFill(refillByAmount.FillTarget);
            refillByAmount.Fill();
            myCollector.ReOrganize();
            refillByAmount.OnCompleted();
            base.EndRefill(other);
        }
        else if (other is Recycle)
        {
            var recycle = other as Recycle;
            recycle.character = this;
        }
        else if (other is ShokuTable && myCollector != null)
        {
            var table = other as ShokuTable;
            var obj = myCollector.GetObject(table.NeedObjectType);
            if (obj == null)
            {
                Waiter waiter = GetComponent<Waiter>();
                if (waiter != null)
                {
                    waiter.ThrowFood();
                }
                base.EndRefill(other);
                return;
            }
            obj.BeFill(table.FillTarget);
            table.Fill((obj as RefillCollectable).Type, GetComponent<Waiter>());
            myCollector.ReOrganize();
            table.OnCompleted();
            base.EndRefill(other);
        }
    }
    public void Throw(IRefillable other)
    {
        if (other is Recycle)
        {
            var recycle = other as Recycle;
            var obj = myCollector.GetLastObject();
            if (obj == null) return;
            obj.BeFill(recycle.FillTarget);
            SFX.Instance.Haptic();
        }
    }
}
