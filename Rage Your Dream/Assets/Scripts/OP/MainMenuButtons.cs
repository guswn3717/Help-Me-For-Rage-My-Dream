using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    [Header("UI 오브젝트 연결")]
    public GameObject nan2doPanel;      // Start 누르면 보여줄 패널
    public GameObject creditPanel;      // Credit 누르면 보여줄 패널

    public void OnStartClicked()
    {
        if (nan2doPanel != null)
            nan2doPanel.SetActive(true);
    }

    public void OnCreditClicked()
    {
        if (creditPanel != null)
            creditPanel.SetActive(true);
    }

    public void OnExitClicked()
    {
        Application.Quit();
        Debug.Log("게임 종료 요청");
    }
}
