using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OP : MonoBehaviour
{
    public GameObject gloves;
    public GameObject OPLogo;
    public Image Back;

    private CanvasGroup gloveGroup;
    private CanvasGroup logoGroup;
    private RectTransform logoRect;

    public float gloveDelay = 1f;
    public float logoDelay = 2f;
    public float logoMoveDuration = 1f;
    public float backStartDelay = 0.5f;
    public float backDisappearDuration = 1f;

    public Vector2 startSize = new Vector2(100, 100);
    public Vector2 endSize = new Vector2(200, 200);

    public Vector3 startPos = new Vector3(130f, -173f, 0f);
    public Vector3 endPos = Vector3.zero;

    public AudioSource OPBGM;

    void Start()
    {
        gloveGroup = gloves.GetComponent<CanvasGroup>() ?? gloves.AddComponent<CanvasGroup>();
        logoGroup = OPLogo.GetComponent<CanvasGroup>() ?? OPLogo.AddComponent<CanvasGroup>();
        logoRect = OPLogo.GetComponent<RectTransform>();

        gloves.SetActive(false);
        OPLogo.SetActive(false);
        Back.fillAmount = 1f;
        logoRect.sizeDelta = startSize;
        logoRect.localPosition = startPos;

        StartCoroutine(PlayOpeningSequence());
    }

    IEnumerator PlayOpeningSequence()
    {
        yield return new WaitForSeconds(gloveDelay);
        gloves.SetActive(true);
        gloveGroup.alpha = 0f;

        if (OPBGM != null)
        {
            OPBGM.loop = true;
            OPBGM.volume = 0.3f;
            OPBGM.Play();
        }

        float t = 0f;
        float gloveDuration = 1.5f;
        while (t < gloveDuration)
        {
            t += Time.deltaTime;
            gloveGroup.alpha = Mathf.Lerp(0f, 1f, t / gloveDuration);
            yield return null;
        }
        gloveGroup.alpha = 1f;

        yield return new WaitForSeconds(logoDelay);
        OPLogo.SetActive(true);
        logoGroup.alpha = 0f;

        t = 0f;
        float transitionDuration = 1f;
        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            float ratio = t / transitionDuration;
            logoGroup.alpha = Mathf.Lerp(0f, 1f, ratio);
            gloveGroup.alpha = Mathf.Lerp(1f, 0f, ratio);
            yield return null;
        }
        logoGroup.alpha = 1f;
        gloveGroup.alpha = 0f;
        gloves.SetActive(false);

        t = 0f;
        Vector3 initialPos = startPos;
        Vector3 targetPos = endPos;
        Vector2 initialSize = startSize;
        Vector2 targetSize = endSize;

        while (t < logoMoveDuration)
        {
            t += Time.deltaTime;
            float ratio = t / logoMoveDuration;
            logoRect.localPosition = Vector3.Lerp(initialPos, targetPos, ratio);
            logoRect.sizeDelta = Vector2.Lerp(initialSize, targetSize, ratio);
            yield return null;
        }
        logoRect.localPosition = targetPos;
        logoRect.sizeDelta = targetSize;

        yield return new WaitForSeconds(backStartDelay);

        OPLogo.SetActive(false);
        t = 0f;
        float startFill = Back.fillAmount;
        float endFill = 0f;
        while (t < backDisappearDuration)
        {
            t += Time.deltaTime;
            float ratio = t / backDisappearDuration;
            Back.fillAmount = Mathf.Lerp(startFill, endFill, ratio);
            yield return null;
        }
        Back.fillAmount = 0f;
    }
}
