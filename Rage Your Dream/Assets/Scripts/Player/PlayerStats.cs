using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float maxHP = 100f;
    public float maxHL = 100f;
    public float maxSP = 100f;

    public float currentHP;
    public float currentHL;
    public float currentSP;

    void Start()
    {
        currentHP = maxHP;
        currentHL = maxHL;
        currentSP = maxSP;
    }

    public void TakeDamage(float amount, bool isStrong)
    {
        currentHP -= amount;
        if (isStrong)
        {
            currentHL = Mathf.Max(currentHP, currentHL - amount); // HL도 같이 감소
        }
    }

    public void UseStamina(float amount)
    {
        currentSP = Mathf.Max(0, currentSP - amount);
    }

    public void RecoverStamina(float amount)
    {
        currentSP = Mathf.Min(maxSP, currentSP + amount);
    }

    public bool IsSBroken() => currentSP <= 0;
}
