using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public float maxHP = 1000f;
    public float currentHP;

    public float maxHL = 1000f;
    public float currentHL;

    public float maxGuardHP = 300f;
    public float currentGuardHP;

    public float guardRecoveryPercent = 0.66f;
    public float guardBreakStunDuration = 2f;

    private bool isGuardBroken = false;

    [Header("UI 이미지 연결")]
    public Image hpBar;
    public Image hlBar;

    [Header("HL 감소 속도")]
    public float hlDecreaseSpeed = 200f; // 1초당 HL 감소량

    private bool isHLDecreasing = false;

    void Start()
    {
        currentHP = maxHP;
        currentHL = maxHL;
        currentGuardHP = maxGuardHP;
    }

    void Update()
    {
        // HL이 점진적으로 HP 위치까지 내려가는 로직
        if (isHLDecreasing)
        {
            currentHL -= hlDecreaseSpeed * Time.deltaTime;
            if (currentHL <= currentHP)
            {
                currentHL = currentHP;
                isHLDecreasing = false;
            }
        }

        // 이미지 UI 채우기 (fillAmount = 0 ~ 1)
        if (hpBar != null) hpBar.fillAmount = Mathf.Clamp01(currentHP / maxHP);
        if (hlBar != null) hlBar.fillAmount = Mathf.Clamp01(currentHL / maxHL);
    }

    public void TakeDamage(float damage, bool isStrongAttack, bool isGuarding)
    {
        if (isGuarding)
        {
            currentGuardHP -= damage;
            float guardDamageMultiplier = 0.2f;
            currentHP -= damage * guardDamageMultiplier;

            if (currentGuardHP <= 0 && !isGuardBroken)
            {
                StartCoroutine(GuardBreak());
            }
        }
        else
        {
            // 일반 공격: HP만 감소
            currentHP -= damage;

            // 강공격: HL도 점진적으로 감소 시작
            if (isStrongAttack)
            {
                isHLDecreasing = true;
            }
        }

        // 다운 판정
        if (currentHP <= 0)
        {
            currentHP = 0;
            Debug.Log("Player Down!");
        }
    }

    IEnumerator GuardBreak()
    {
        isGuardBroken = true;
        Debug.Log("Guard Broken! Stunned!");

        yield return new WaitForSeconds(guardBreakStunDuration);

        currentGuardHP = maxGuardHP * guardRecoveryPercent;
        isGuardBroken = false;
        Debug.Log("Guard Recovered");
    }
}
