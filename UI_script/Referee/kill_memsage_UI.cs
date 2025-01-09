using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class kill_memsage_UI : MonoBehaviour
{
    [SerializeField] private RectTransform killer, killed;
    [SerializeField] private TextMeshProUGUI killedText;
    private float tickTime;
    private string killerName, killedName;
    [SerializeField] private GameObject red1, red2, blue1, blue2;
    private GameObject killerObject, killedObject;

    public void Set_Kill_Memsage(bool update, string killer_name, string killed_name)
    {
        this.killerName = killer_name;
        this.killedName = killed_name;
        if (update)
        {
            RectTransform rectTransform;
            gameObject.SetActive(true);
            tickTime = 10.0f;

            if (killerName == "RED1")
            {
                killerObject = GameObject.Instantiate(red1, killer.position, Quaternion.identity);
            }
            else if (killerName == "RED2")
            {
                killerObject = GameObject.Instantiate(red2, killer.position, Quaternion.identity);
            }
            else if (killerName == "BLUE1")
            {
                killerObject = GameObject.Instantiate(blue1, killer.position, Quaternion.identity);
            }
            else if (killerName == "BLUE2")
            {
                killerObject = GameObject.Instantiate(blue2, killer.position, Quaternion.identity);
            }
            else
            {
                killedText.text = "已阵亡";
                if (killedName == "RED1")
                {
                    killedObject = GameObject.Instantiate(red1, killed.position, Quaternion.identity);
                }
                else if (killedName == "RED2")
                {
                    killedObject = GameObject.Instantiate(red2, killed.position, Quaternion.identity);
                }
                else if (killedName == "BLUE1")
                {
                    killedObject = GameObject.Instantiate(blue1, killed.position, Quaternion.identity);
                }
                else if (killedName == "BLUE2")
                {
                    killedObject = GameObject.Instantiate(blue2, killed.position, Quaternion.identity);
                }

                rectTransform = killedObject.GetComponent<RectTransform>();
                rectTransform.SetParent(GetComponent<RectTransform>());
                return;
            }


            if (killedName == "RED1")
            {
                killedObject = GameObject.Instantiate(red1, killed.position, Quaternion.identity);
            }
            else if (killedName == "RED2")
            {
                killedObject = GameObject.Instantiate(red2, killed.position, Quaternion.identity);
            }
            else if (killedName == "BLUE1")
            {
                killedObject = GameObject.Instantiate(blue1, killed.position, Quaternion.identity);
            }
            else if (killedName == "BLUE2")
            {
                killedObject = GameObject.Instantiate(blue2, killed.position, Quaternion.identity);
            }

            killedText.text = "击毁";
            rectTransform = killerObject.GetComponent<RectTransform>();
            rectTransform.SetParent(GetComponent<RectTransform>());
            rectTransform = killedObject.GetComponent<RectTransform>();
            rectTransform.SetParent(GetComponent<RectTransform>());
        }
    }

    void OnGUI()
    {
        if (tickTime > 0.0f)
        {
            tickTime -= Time.deltaTime;
        }
        else
        {
            Destroy(killerObject);
            Destroy(killedObject);
            gameObject.SetActive(false);
        }
    }
}