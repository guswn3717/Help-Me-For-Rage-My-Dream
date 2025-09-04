using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStamina : MonoBehaviour
{
    [Header("스태미나 UI")]
    public Image staminaBar;

    [Header("스태미나 수치")]
    public float maxStamina = 1f;
    public float currentStamina = 1f;

    [Header("회복 설정")]
    public float recoverSpeed = 0.2f;
    public float recoverDelay = 0.5f;

    [Header("SB 설정")]
    public bool isSB = false;
    public float sbDuration = 1.5f;

    [Header("닷지 SP 차단 시간")]
    public float dodgeSPBlockDuration = 0.5f;

    private bool canRecover = true;
    private bool dodgeSPBlocked = false;

    void Start()
    {
        currentStamina = maxStamina;
    }

    void Update()
    {
        if (!isSB && canRecover && !dodgeSPBlocked)
            RecoverStamina();

        UpdateStaminaUI();
    }

    void RecoverStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += recoverSpeed * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
    }

    void UpdateStaminaUI()
    {
        if (staminaBar != null)
            staminaBar.fillAmount = Mathf.Clamp01(currentStamina);
    }

    // 펀치용: SP 음수 허용, 음수 시 SB
    public bool TryUseStaminaPunch(float amount)
    {
        currentStamina -= amount;
        if (currentStamina < 0f && !isSB)
        {
            StartCoroutine(StartSB());
        }
        StartCoroutine(RecoveryDelay());
        return true; // 항상 행동은 나감
    }

    // 닷지용: SP가 0 이상이어야 함, SP가 0이면 0.5초 회복 차단
    public bool TryUseStaminaDodge(float amount)
    {
        if (currentStamina < amount)
        {
            currentStamina = Mathf.Max(currentStamina - amount, 0f);
            StartCoroutine(DodgeSPBlock());
            return true;
        }
        else
        {
            currentStamina -= amount;
            StartCoroutine(RecoveryDelay());
            return true;
        }
    }

    IEnumerator RecoveryDelay()
    {
        canRecover = false;
        yield return new WaitForSeconds(recoverDelay);
        canRecover = true;
    }

    IEnumerator StartSB()
    {
        isSB = true;
        canRecover = false;
        Debug.Log("SB 상태 시작!");
        yield return new WaitForSeconds(sbDuration);
        isSB = false;
        canRecover = true;
        Debug.Log("SB 상태 종료!");
    }

    IEnumerator DodgeSPBlock()
    {
        dodgeSPBlocked = true;
        yield return new WaitForSeconds(dodgeSPBlockDuration);
        dodgeSPBlocked = false;
    }

    public float GetStamina() => currentStamina;

    public void AddStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina + amount, 0f, maxStamina);
    }

    public void SetStamina(float value)
    {
        currentStamina = Mathf.Clamp(value, 0f, maxStamina);
    }
}
