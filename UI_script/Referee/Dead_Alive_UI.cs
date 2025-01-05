using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dead_Alive_UI : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image aliveTimeImage;
    [SerializeField] private TextMeshProUGUI aliveTime;
    [SerializeField] private Button aliveButton;
    [SerializeField] private TextMeshProUGUI Damage42mm, Damage17mm, DamageUnknow, DamageHeat,nickname;
    Damage_Log damageLog;
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

    public void Set_Damage_Log(Damage_Log damageLog_)
    {
        int total;
        damageLog = damageLog_;
        total = damageLog.ExcessheatDamage + damageLog.BigBulletDamage + damageLog.SmallBulletDamage +
                damageLog.UnknowDemage;
        Damage42mm.text = damageLog.BigBulletDamage.ToString()+'|'+ (damageLog.BigBulletDamage / total * 100).ToString()+"%"; 
        Damage17mm.text = damageLog.SmallBulletDamage.ToString()+'|'+ (damageLog.SmallBulletDamage / total * 100).ToString()+"%"; 
        DamageUnknow.text = damageLog.UnknowDemage.ToString()+'|'+ (damageLog.UnknowDemage / total * 100).ToString()+"%";
        DamageHeat.text = damageLog.ExcessheatDamage.ToString()+'|'+ (damageLog.ExcessheatDamage / total * 100).ToString()+"%";
        nickname.text = "被"+damageLog.LastDamagenickname+"杀死";
    }
}