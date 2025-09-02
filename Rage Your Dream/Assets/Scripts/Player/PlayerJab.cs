using UnityEngine;
using System.Collections;

public class PlayerJab : MonoBehaviour
{
    private Animator anim;

    private bool isJabbing = false;
    private bool isHeavyPunching = false;

    private float firstJabTime = -1f;
    private float jabCooldown = 0.35f;
    private float jabInputLockedUntil = 0f;

    [SerializeField] private float secondJabThreshold = 0.4f;

    [Header("스태미나")]
    public PlayerStamina stamina;
    public float jabStaminaCost = 0.1f;
    public float heavyStaminaCost = 0.2f;

    [Header("입력 키")]
    public KeyCode jabKey = KeyCode.K;
    public KeyCode heavyPunchKey = KeyCode.L;

    [Header("이동")]
    public PlayerMovement movement;

    public bool IsJabbing => isJabbing;
    public bool IsHeavyPunching => isHeavyPunching;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (stamina == null)
            stamina = GetComponent<PlayerStamina>();
        if (movement == null)
            movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // 강펀치 우선 처리
        if (Input.GetKeyDown(heavyPunchKey))
        {
            if (!isHeavyPunching)
                StartCoroutine(PerformHeavyPunch());
            return;
        }

        // 잽 입력
        if (Time.time >= jabInputLockedUntil && Input.GetKeyDown(jabKey))
        {
            if (stamina != null && !stamina.TryUseStamina(jabStaminaCost))
            {
                Debug.Log("스태미나 부족으로 잽 불가");
                return;
            }

            StartCoroutine(PerformJab());
        }
    }

    IEnumerator PerformJab()
    {
        jabInputLockedUntil = Time.time + jabCooldown;

        if (!isJabbing)
        {
            isJabbing = true;
            firstJabTime = Time.time;

            anim.SetBool("IsJabbing", true);
            anim.SetTrigger("Jab");

            if (movement != null)
                movement.LockMove();

            yield return WaitForAnimation("Jab");

            if (movement != null)
                movement.UnlockMove();

            isJabbing = false;
            anim.SetBool("IsJabbing", false);
        }
        else if (Time.time - firstJabTime <= secondJabThreshold)
        {
            firstJabTime = Time.time;
            anim.Play("Jab", 0, 0.2f);

            if (movement != null)
                movement.LockMove();

            yield return WaitForAnimation("Jab");

            if (movement != null)
                movement.UnlockMove();

            isJabbing = false;
            anim.SetBool("IsJabbing", false);
        }
    }

    IEnumerator PerformHeavyPunch()
    {
        if (stamina != null && !stamina.TryUseStamina(heavyStaminaCost))
        {
            Debug.Log("스태미나 부족으로 강펀치 불가");
            yield break;
        }

        // 잽 상태 초기화
        isJabbing = false;
        anim.SetBool("IsJabbing", false);
        anim.ResetTrigger("Jab");

        if (movement != null)
            movement.LockMove();

        // 트리거 설정 후 0.1초만 유지
        anim.SetTrigger("Strong_Punch");
        StartCoroutine(ResetTriggerShortly("Strong_Punch", 0.1f));

        // Strong_Punch 애니메이션 상태 진입 시점까지 대기
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Strong_Punch"));

        isHeavyPunching = true;

        // 애니메이션 끝날 때까지 대기
        yield return WaitForAnimation("Strong_Punch");

        if (movement != null)
            movement.UnlockMove();

        isHeavyPunching = false;
    }

    // 트리거를 짧게 켜고 끄는 코루틴
    private IEnumerator ResetTriggerShortly(string triggerName, float delay)
    {
        yield return new WaitForSeconds(delay);
        anim.ResetTrigger(triggerName);
    }

    private IEnumerator WaitForAnimation(string animName)
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        // 애니메이션 시작될 때까지 대기
        while (!state.IsName(animName))
        {
            yield return null;
            state = anim.GetCurrentAnimatorStateInfo(0);
        }

        // 애니메이션 끝날 때까지 대기
        while (state.normalizedTime < 1.0f)
        {
            yield return null;
            state = anim.GetCurrentAnimatorStateInfo(0);
        }
    }
}
