using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Rain : MonoBehaviour
{
    [Header("Particle Prefab")]
    public GameObject confettiPrefab; // Confetti Particle Prefab

    [Header("색상 배열")]
    public Color[] colors = new Color[]
    {
        Color.white, Color.red, Color.blue, Color.green, Color.yellow, Color.magenta
    };

    [Header("생성 위치, 회전, 스케일")]
    public Vector3 spawnPosition = new Vector3(4.89f, 16.57f, 16.69f);
    public Vector3 spawnRotation = new Vector3(90f, 0f, 0f);
    public Vector3 spawnScale = Vector3.one;

    [Header("Particle 설정")]
    public int particleCount = 30; // 한 번에 생성할 입자 수
    public float spawnInterval = 1f; // 반복 간격

    [Header("Back 이미지")]
    public Image Back;

    private ParticleSystem ps;
    private GameObject confettiInstance;
    private bool isSpawning = false;

    void Start()
    {
        if (Back != null)
            StartCoroutine(CheckBackForSpawn());
        else
            Debug.LogWarning("Back 이미지가 Rain에 연결되지 않았습니다!");
    }

    IEnumerator CheckBackForSpawn()
    {
        while (true)
        {
            // Back.fillAmount가 1 이하이면 폭죽 시작
            if (Back.fillAmount <= 1f && !isSpawning)
            {
                isSpawning = true;
                CreateConfettiInstance();
                StartCoroutine(SpawnRainLoop());
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    void CreateConfettiInstance()
    {
        if (confettiPrefab == null) return;

        // Instantiate 후 Parent 제거 → 월드 좌표 기준
        confettiInstance = Instantiate(confettiPrefab);
        confettiInstance.transform.SetParent(null); 
        confettiInstance.transform.position = spawnPosition;
        confettiInstance.transform.rotation = Quaternion.Euler(spawnRotation);
        confettiInstance.transform.localScale = spawnScale;

        ps = confettiInstance.GetComponent<ParticleSystem>();
        if (ps != null)
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    IEnumerator SpawnRainLoop()
    {
        while (true)
        {
            SpawnRain();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnRain()
    {
        if (ps == null) return;

        // 개별 파티클 랜덤 색상
        for (int i = 0; i < particleCount; i++)
        {
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
            emitParams.startColor = colors[Random.Range(0, colors.Length)];
            ps.Emit(emitParams, 1);
        }
    }
}
