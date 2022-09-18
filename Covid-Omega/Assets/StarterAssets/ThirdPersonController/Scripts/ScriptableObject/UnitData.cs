using UnityEngine;

[CreateAssetMenu(menuName = "Data/Unit")]
public class UnitData : ScriptableObject
{
    public float MaxHealth;
    public float CurrentHealth;
    public float MovingSpeed;
    public float RotatingSpeed;
    public float DetectingRange;
}