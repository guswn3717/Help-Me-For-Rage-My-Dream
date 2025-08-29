using UnityEngine;

public class RingBoundaryCheck : MonoBehaviour
{
    public Vector3 resetPosition = new Vector3(0f, 1f, 0f);

    private float minX = -3.75f;
    private float maxX = 3.75f;
    private float minZ = -3.75f;
    private float maxZ = 3.75f;

    void Update()
{
    // 기존 링 범위 체크 코드도 여기 있을 거고...

    // y값 안전빵 체크
    if (transform.position.y < -2f)
    {
        ResetPosition();
    }
}

void ResetPosition()
{
    transform.position = new Vector3(0f, 1f, 0f);
    Rigidbody rb = GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.velocity = Vector3.zero; // 속도 초기화
    }
}

}
