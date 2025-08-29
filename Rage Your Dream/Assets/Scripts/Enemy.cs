using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Current HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        // 네트워크 동기화, 리스폰 등 처리 여기에 추가 가능
        Destroy(gameObject);
    }
}
