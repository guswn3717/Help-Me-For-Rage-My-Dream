using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerDodge : MonoBehaviour
{
    private Rigidbody rb;
    private Animator anim;

    private bool isDodging = false;
    private float originalDrag;

    public float dodgeForce = 12f;
    public float dodgeDrag = 6f;

    public Transform targetEnemy;

    [Header("스태미나 관련")]
    public PlayerStamina stamina;      // 연결할 스태미나 스크립트
    public float dodgeStaminaCost = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        originalDrag = rb.drag;

        if (stamina == null)
            stamina = GetComponent<PlayerStamina>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isDodging)
        {
            if (stamina != null)
            {
                // 닷지용 SP 처리
                stamina.TryUseStaminaDodge(dodgeStaminaCost);
            }

            Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            Vector3 dodgeDirection;

            if (targetEnemy != null)
            {
                Vector3 enemyDir = (targetEnemy.position - transform.position).normalized;
                enemyDir.y = 0;
                Quaternion enemyRot = Quaternion.LookRotation(enemyDir);
                dodgeDirection = enemyRot * inputDir;
            }
            else
            {
                dodgeDirection = inputDir;
            }

            if (dodgeDirection == Vector3.zero)
                dodgeDirection = transform.forward;

            StartCoroutine(DodgeRoutine(dodgeDirection));
        }
    }

    IEnumerator DodgeRoutine(Vector3 dir)
    {
        isDodging = true;
        anim.SetBool("isDodging", true);

        yield return new WaitForSeconds(0.15f);

        rb.velocity = Vector3.zero;
        rb.drag = dodgeDrag;
        rb.AddForce(dir.normalized * dodgeForce, ForceMode.Impulse);

        yield return new WaitForSeconds(0.15f);

        rb.drag = originalDrag;
        isDodging = false;
        anim.SetBool("isDodging", false);
    }

    public bool IsDodging() => isDodging;
}
