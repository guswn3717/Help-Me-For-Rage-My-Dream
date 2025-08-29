using UnityEngine;
using UnityEngine.UI;

public class SPTest : MonoBehaviour
{
    public StaminaSlider playerSP;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            playerSP.ConsumeSP(20f);
        }
    }
}
