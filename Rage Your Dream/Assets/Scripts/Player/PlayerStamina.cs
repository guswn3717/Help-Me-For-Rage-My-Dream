using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [Header("스태미나 UI")]
    public Image staminaBar;

    [Header("스태미나 수치")]
    public float maxStamina = 1f;               // 최대 스태미나 (1 = 100%)
    public float currentStamina = 1f;           // 현재 스태미나 (0 ~ 1)

    [Header("회복 설정")]
    public float recoverSpeed = 0.2f;           // 초당 회복량

    void Start()
    {
        currentStamina = maxStamina;
    }

    void Update()
    {
        RecoverStamina();
        UpdateStaminaUI();
    }

    // 스태미나 회복
    void RecoverStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += recoverSpeed * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
    }

    // UI 이미지 업데이트
    void UpdateStaminaUI()
    {
        if (staminaBar != null)
        {
            staminaBar.fillAmount = currentStamina;
        }
    }

    // 외부에서 스태미나 사용 요청
    public bool TryUseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            return true; // 사용 성공
        }

        return false; // 스태미나 부족
    }

    // 외부에서 현재 스태미나 확인할 수 있도록
    public float GetStamina()
    {
        return currentStamina;
    }

    // 외부에서 강제로 스태미나 회복하거나 감소시킬 수도 있음
    public void AddStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina + amount, 0f, maxStamina);
    }

    public void SetStamina(float value)
    {
        currentStamina = Mathf.Clamp(value, 0f, maxStamina);
    }
}
