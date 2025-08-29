using UnityEngine;
using UnityEngine.UI;

public class StaminaSlider : MonoBehaviour
{
    [Header("슬라이더 연결")]
    public Slider spSlider;        // SP 시각화용 슬라이더

    [Header("스테미나 설정")]
    public float maxSP = 100f;     // 최대 스테미나
    public float recoverRate = 10f; // 초당 회복량

    [HideInInspector]
    public float currentSP;         // 현재 스테미나

    [Header("SB 설정")]
    public bool isSB = false;       // 스테미나 브로큰 상태
    public float sbDuration = 1.5f; // SB 지속 시간
    private float sbTimer = 0f;

    void Start()
    {
        currentSP = maxSP;
        spSlider.maxValue = maxSP;
        spSlider.value = currentSP;
    }

    void Update()
    {
        // SB 타이머
        if (isSB)
        {
            sbTimer -= Time.deltaTime;
            if (sbTimer <= 0f)
                isSB = false;
        }

        // SP 회복 (SB가 아닐 때만)
        if (!isSB && currentSP < maxSP)
        {
            currentSP += recoverRate * Time.deltaTime;
            currentSP = Mathf.Min(currentSP, maxSP);
            spSlider.value = currentSP;
        }
    }

    // 스테미나 소모
    public void ConsumeSP(float amount, bool triggerSB = true)
    {
        currentSP -= amount;
        currentSP = Mathf.Max(currentSP, 0f);
        spSlider.value = currentSP;

        if (triggerSB && currentSP <= 0f)
        {
            TriggerSB(sbDuration);
        }
    }

    // SB 발동
    public void TriggerSB(float duration)
    {
        isSB = true;
        sbTimer = duration;
        // 이동/공격 제한은 외부에서 CanAct 체크
    }

    // 행동 가능 여부 확인
    public bool CanAct()
    {
        return !isSB;
    }
}
