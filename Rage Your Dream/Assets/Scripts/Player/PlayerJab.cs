using UnityEngine;
using System.Collections;

public class PlayerJab : MonoBehaviour
{
    private Animator anim;

    private bool isJabbing = false;
    private bool isHeavyPunching = false;

    private float firstJabTime = -1f;
    private float jabCooldown = 0.35f; // 입력 무시 시간
    private float jabInputLockedUntil = 0f;

    [SerializeField] private float secondJabThreshold = 0.4f;

    public bool IsJabbing => isJabbing;
    public bool IsHeavyPunching => isHeavyPunching;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isHeavyPunching)
            return;

        // 잽 입력
        if (Time.time >= jabInputLockedUntil && Input.GetKeyDown(KeyCode.K))
        {
            TryJab();
        }

        // 잽 애니메이션 끝났는지 확인
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        if ((state.IsName("Jab") || state.IsName("Jab_Combo")) && state.normalizedTime >= 1.0f)
        {
            isJabbing = false;
            anim.SetBool("IsJabbing", false);
        }
    }

    void TryJab()
    {
        jabInputLockedUntil = Time.time + jabCooldown;

        if (!isJabbing)
        {
            StartFirstJab();
            return;
        }

        // 두 번째 이상 잽
        if (Time.time - firstJabTime <= secondJabThreshold)
        {
            StartSecondJab();
        }
    }

    void StartFirstJab()
    {
        isJabbing = true;
        firstJabTime = Time.time;

        anim.SetBool("IsJabbing", true);
        anim.SetTrigger("Jab");
    }

    void StartSecondJab()
    {
        firstJabTime = Time.time; // 연속 잽 초기화
        anim.Play("Jab", 0, 0.2f); // 애니메이션 앞부분 0.2초 건너뛰기
    }

    public bool CanHeavyPunch()
    {
        return !isHeavyPunching &&
               (!isJabbing || anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"));
    }

    public void HeavyPunchStart()
    {
        if (!CanHeavyPunch()) return;

        // 잽 상태 초기화
        isJabbing = false;
        anim.SetBool("IsJabbing", false);
        anim.ResetTrigger("Jab");

        isHeavyPunching = true;

        // 강펀치 애니메이션으로 즉시 전환
        anim.Play("HeavyPunch", 0, 0f);
    }

    // 애니메이션 이벤트에서 호출
    public void HeavyPunchEnd()
    {
        // 1초 뒤 상태 초기화
        StartCoroutine(ResetAfterDelay(1f));
    }

    private IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 코드 변수 초기화
        isHeavyPunching = false;
        isJabbing = false;

        // Animator 초기화
        anim.SetBool("IsJabbing", false);
        anim.ResetTrigger("Jab");
        anim.ResetTrigger("HeavyPunch");

        // 강제 Idle 상태로 전환
        anim.Play("Idle", 0, 0f);
    }
}
