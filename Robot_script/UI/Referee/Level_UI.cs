using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Level_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Leveltext;
    public void set_level(int level, int now_exp, int next_exp)
    {
        Leveltext.text ="Level "+ (level + 1).ToString() + " : " + now_exp.ToString() + " / " + next_exp.ToString();
    }
}