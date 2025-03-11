using Photon.Pun;
using UnityEngine;

public class Base_control : MonoBehaviour
{
    [SerializeField] Animator animator1, animator2, animator3;
    [SerializeField] private Referee_control referee;
    private int hp;
    private bool isOpen = true;

    public void OpenArmor()
    {
        animator1.SetBool("BaseIsOpen", true);
        animator2.SetBool("BaseIsOpen", true);
        animator3.SetBool("BaseIsOpen", true);
    }

    void Update()
    {
        // 按空格键触发动画
        // if (!PhotonNetwork.IsMasterClient) return;
        hp = referee.Get_robotHP();
        if (hp <= 2000 && isOpen)
        {
            isOpen = false;
            if (referee.Get_Robot_color() == Robot_color.RED)
                referee.rule.Send_Memsage("红方基地已展开", 5);
            else referee.rule.Send_Memsage("蓝方基地已展开", 5);

            OpenArmor();
        }
    }
}