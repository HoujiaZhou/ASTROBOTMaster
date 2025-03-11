
    using System;
    using TMPro;
    using UnityEngine;

    public class Message_UI: MonoBehaviour
    {
        [SerializeField] private    TextMeshProUGUI text;
        private float time;
        public void Set_message(string message, float time)
        {
            this.time = time;
            this.text.text = message;
        }

        private void OnGUI()
        {
            if (time > 0)
            {
                time -= Time.deltaTime;
            }
            else
            {
                time = 0;
                text.text = "";
            }
        }
    }
