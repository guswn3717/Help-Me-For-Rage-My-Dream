using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [Header("공격 설정")]
    public float jabDamage = 3f;
    public float heavyDamage = 6f;
    public float attackRange = 2f;
    public LayerMask enemyLayer; // Enemy 레이어만 체크

    // === AnimationEvent에서 호출 ===
    public void JabHit()
    {
        DealDamage(jabDamage);
    }

    public void HeavyHit()
    {
        DealDamage(heavyDamage);
    }

    private void DealDamage(float damage)
    {
        // 플레이어 위치 기준 일정 거리 내 적 탐색
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);

        foreach (Collider hit in hits)
        {
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                // 거리 확인 (정확한 판정)
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance <= attackRange)
                {
                    // 데미지 적용 (두 번째 인자는 isHeavy)
                    enemy.TakeDamage(damage, damage == heavyDamage);
                }
            }
        }
    }

    // 씬에서 시각화용
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
