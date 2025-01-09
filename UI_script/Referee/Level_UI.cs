using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Level_UI : MonoBehaviour
{
    [SerializeField] private Image level_fill,level_image;
    [SerializeField] private GameObject red1,red3,blue1,blue3;
    public Sprite level1, level2, level3, level4, level5, level6, level7, level8, level9, level10;
    private Sprite[] level = new Sprite[10];
    private void Start()
    {
        level[0] = level1;
        level[1] = level2;
        level[2] = level3;
        level[3] = level4;
        level[4] = level5;
        level[5] = level6;
        level[6] = level7;
        level[7] = level8;
        level[8] = level9;
        level[9] = level10;
    }
    public void set_level(int nowlevel, int now_exp, int next_exp,string nickname)
    {
        if(nowlevel < 9)
            level_fill.fillAmount = (float)now_exp / (float)next_exp / 2.0f; 
        else level_fill.fillAmount = 1;
        level_image.sprite = level[nowlevel];
        if (nickname == "RED1")
        {
            if (!red1.activeSelf)
            {
                red1.SetActive(true);
            }
        }else if (nickname == "RED2")
        {
            if (!red3.activeSelf)
            {
                red3.SetActive(true);
            }
        }else if (nickname == "BLUE1")
        {
            if (!blue1.activeSelf)
            {
                blue1.SetActive(true);
            }
        }else if (nickname == "BLUE2")
        {
            if (!blue3.activeSelf)
            {
                blue3.SetActive(true);
            }
        }
    }
}