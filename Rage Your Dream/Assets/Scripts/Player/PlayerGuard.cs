using UnityEngine;

public class PlayerGuard : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            anim.SetBool("IsGuarding", true);
            
        if (Input.GetKeyUp(KeyCode.F))
            anim.SetBool("IsGuarding", false);
    }
}
