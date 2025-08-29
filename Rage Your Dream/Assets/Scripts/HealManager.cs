using UnityEngine;

public class HealManager : MonoBehaviour
{
    public PlayerStats stats;
    public float healSpeed = 5f; // 초당 회복량

    void Update()
    {
        if (stats.currentHP < stats.currentHL)
        {
            stats.currentHP += healSpeed * Time.deltaTime;
            stats.currentHP = Mathf.Min(stats.currentHP, stats.currentHL); // HL보다 못 넘음
        }
    }
}
