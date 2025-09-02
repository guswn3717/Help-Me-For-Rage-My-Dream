using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 2f;
    private float currentMoveSpeed;

    private Rigidbody rb;
    private Vector3 moveDirection;

    public Transform targetEnemy;
    private PlayerJab jab;
    private Animator anim;

    private int moveLockCount = 0; // 이동 잠금 카운터

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jab = GetComponent<PlayerJab>();
        anim = GetComponent<Animator>();

        currentMoveSpeed = moveSpeed;

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.useGravity = true;
    }

    void FixedUpdate()
    {
        if (jab != null && (jab.IsJabbing || jab.IsHeavyPunching))
        {
            // 이동은 여전히 MoveLockCount로 제어
        }

        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        if (targetEnemy != null)
        {
            Vector3 enemyDir = (targetEnemy.position - transform.position).normalized;
            enemyDir.y = 0;
            Quaternion enemyRot = Quaternion.LookRotation(enemyDir);
            moveDirection = (enemyRot * inputDir).normalized;
        }
        else
        {
            moveDirection = inputDir;
        }

        Vector3 moveOffset = moveDirection * currentMoveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveOffset);

        // 이동 속도 값으로 애니메이터 파라미터 세팅 (0~1 범위)
        anim.SetFloat("MoveSpeed", moveDirection.magnitude * (currentMoveSpeed / moveSpeed));
    }

    void HandleRotation()
    {
        if (targetEnemy != null)
        {
            Vector3 lookPos = targetEnemy.position - transform.position;
            lookPos.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(lookPos);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 10f));
        }
    }

    // MoveSpeed 제어
    public void LockMove()
    {
        moveLockCount++;
        currentMoveSpeed = 0f;
    }

    public void UnlockMove()
    {
        moveLockCount--;
        if (moveLockCount <= 0)
        {
            moveLockCount = 0;
            currentMoveSpeed = moveSpeed;
        }
    }
}
