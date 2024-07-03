using UnityEngine;

[CreateAssetMenu(fileName = "Staff Data", menuName = "Scriptable Object/ NPC Data", order = 0)]
public class StaffData : ScriptableObject
{
    public float[] moveSpeed = new float[4];
    public float[] doTaskTime = new float[4];
    public int[] maxBringAmount = new int[4];
    public Material[] skin = new Material[4];
}
