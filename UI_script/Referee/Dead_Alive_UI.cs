using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dead_Alive_UI : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image aliveTimeImage;
    [SerializeField] private TextMeshProUGUI aliveTime;
    [SerializeField] private Button aliveButton;

    public void Set_Alive_Time(float totalTime, float nowTime)
    {
        if (totalTime == 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }

        if (nowTime > 0)
        {
            aliveButton.interactable = false;
        }
        else aliveButton.interactable = true;
        
        aliveTimeImage.fillAmount = (totalTime - nowTime) / totalTime;
        int time = (int)nowTime;
        aliveTime.text = time.ToString() + "S";
    }
}