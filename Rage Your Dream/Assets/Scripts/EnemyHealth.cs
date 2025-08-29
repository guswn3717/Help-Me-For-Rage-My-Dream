using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public float maxHP = 500f;
    public float maxHL = 500f;
    public float maxGuardHP = 150f;
    public float guardRecoveryPercent = 0.66f;
    public float guardBreakStunDuration = 2f;

    [Header("현재 값")]
    public float currentHP;
    public float currentHL;
    public float currentGuardHP;

    private bool isGuardBroken = false;

    [Header("UI 이미지 연결")]
    public Image hpBar;
    public Image hlBar;

    [Header("HL 감소 속도")]
    public float hlDecreaseSpeed = 200f; // 1초당 HL 감소량

    [Header("HP 회복 속도")]
    public float hpRecoverSpeed = 50f; // 인스펙터에서 HP 회복 속도 조절

    private bool isHLDecreasing = false;

    void Start()
    {
        currentHP = maxHP;
        currentHL = maxHL;
        currentGuardHP = maxGuardHP;
    }

    void Update()
    {
        // 1️⃣ HL이 점진적으로 HP 위치까지 내려가는 로직
        if (isHLDecreasing)
        {
            currentHL -= hlDecreaseSpeed * Time.deltaTime;
            if (currentHL <= currentHP)
            {
                currentHL = currentHP;
                isHLDecreasing = false;
            }
        }

        // 2️⃣ HP가 HL 위치까지 점진적으로 회복되는 로직
        if (currentHP < currentHL)
        {
            currentHP += hpRecoverSpeed * Time.deltaTime;
            if (currentHP > currentHL)
                currentHP = currentHL;
        }

        // 3️⃣ 이미지 Bar 업데이트 (fillAmount = 0 ~ 1)
        if (hpBar != null) hpBar.fillAmount = Mathf.Clamp01(currentHP / maxHP);
        if (hlBar != null) hlBar.fillAmount = Mathf.Clamp01(currentHL / maxHL);
    }

    public void TakeDamage(float damage, bool isStrongAttack, bool isGuarding = false)
    {
        if (isGuarding)
        {
            currentGuardHP -= damage;
            float guardDamageMultiplier = 0.2f;
            currentHP -= damage * guardDamageMultiplier;

            if (currentGuardHP <= 0 && !isGuardBroken)
                StartCoroutine(GuardBreak());
        }
        else
        {
            currentHP -= damage;

            if (isStrongAttack)
                isHLDecreasing = true;
        }

        if (currentHP <= 0)
        {
            Debug.Log($"{gameObject.name} Down!");
            currentHP = 0;
        }
    }

    IEnumerator GuardBreak()
    {
        isGuardBroken = true;
        Debug.Log($"{gameObject.name} Guard Broken! Stunned!");

        yield return new WaitForSeconds(guardBreakStunDuration);

        currentGuardHP = maxGuardHP * guardRecoveryPercent;
        isGuardBroken = false;

        Debug.Log($"{gameObject.name} Guard Recovered");
    }
}
