using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerJab : MonoBehaviour
{
    private Animator anim;
    private Rigidbody rb;

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

    [Header("잽 밀기")]
    public float jabPushForce = 3f;      
    public float jabPushDuration = 0.05f;

    [Header("강펀치 밀기")]
    public float heavyPunchPushForce = 8f;  
    public float heavyPunchPushDuration = 0.07f;

    public bool IsJabbing => isJabbing;
    public bool IsHeavyPunching => isHeavyPunching;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        if (stamina == null) stamina = GetComponent<PlayerStamina>();
        if (movement == null) movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // 강펀치 우선 처리
        if (Input.GetKeyDown(heavyPunchKey))
        {
            if (!isHeavyPunching)
            {
                if (stamina != null)
                {
                    // 펀치용 SP 처리
                    stamina.TryUseStaminaPunch(heavyStaminaCost);
                }
                StartCoroutine(PerformHeavyPunch());
            }
            return;
        }

        // 잽 입력
        if (Time.time >= jabInputLockedUntil && Input.GetKeyDown(jabKey))
        {
            if (stamina != null)
            {
                stamina.TryUseStaminaPunch(jabStaminaCost);
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

            movement?.LockMove();
            StartCoroutine(JabPush());

            yield return WaitForAnimation("Jab");

            movement?.UnlockMove();
            isJabbing = false;
            anim.SetBool("IsJabbing", false);
        }
        else if (Time.time - firstJabTime <= secondJabThreshold)
        {
            firstJabTime = Time.time;
            anim.Play("Jab", 0, 0.2f);

            movement?.LockMove();
            StartCoroutine(JabPush());

            yield return WaitForAnimation("Jab");

            movement?.UnlockMove();
            isJabbing = false;
            anim.SetBool("IsJabbing", false);
        }
    }

    private IEnumerator JabPush()
    {
        if (rb != null)
        {
            rb.AddForce(transform.forward * jabPushForce, ForceMode.Impulse);
            yield return new WaitForSeconds(jabPushDuration);
            rb.velocity = Vector3.zero;
        }
    }

    IEnumerator PerformHeavyPunch()
    {
        isJabbing = false;
        anim.SetBool("IsJabbing", false);
        anim.ResetTrigger("Jab");

        movement?.LockMove();

        anim.SetTrigger("Strong_Punch");
        StartCoroutine(ResetTriggerShortly("Strong_Punch", 0.1f));

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Strong_Punch"));
        isHeavyPunching = true;

        StartCoroutine(HeavyPunchPush());
        yield return WaitForAnimation("Strong_Punch");

        movement?.UnlockMove();
        isHeavyPunching = false;
    }

    private IEnumerator HeavyPunchPush()
    {
        if (rb != null)
        {
            rb.AddForce(transform.forward * heavyPunchPushForce, ForceMode.Impulse);
            yield return new WaitForSeconds(heavyPunchPushDuration);
            rb.velocity = Vector3.zero;
        }
    }

    private IEnumerator ResetTriggerShortly(string triggerName, float delay)
    {
        yield return new WaitForSeconds(delay);
        anim.ResetTrigger(triggerName);
    }

    private IEnumerator WaitForAnimation(string animName)
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        while (!state.IsName(animName))
        {
            yield return null;
            state = anim.GetCurrentAnimatorStateInfo(0);
        }

        while (state.normalizedTime < 1.0f)
        {
            yield return null;
            state = anim.GetCurrentAnimatorStateInfo(0);
        }
    }
}
