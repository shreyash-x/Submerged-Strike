using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UI
{
    public class FeedbackForm : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI emailField;
        [SerializeField] private TextMeshProUGUI textField;
        [SerializeField] private Toggle isBugToggle;
        [SerializeField] private Button submitBtn;
        [SerializeField] private TextMeshProUGUI formSubmitStatus;
        [SerializeField] private Color errorColor;
        [SerializeField] private Color successColor;

        private const string URL = "https://api.web3forms.com/submit";

        private void Awake()
        {
            submitBtn.onClick.AddListener(() =>
            {
                textField.text = textField.text.Trim();
                if (textField.text.Length == 0) return;
                StartCoroutine(SubmitForm());
            });
        }

        private void Update()
        {
            if (emailField.havePropertiesChanged || textField.havePropertiesChanged) formSubmitStatus.text = "";
        }

        // form should be validated before calling this
        private IEnumerator SubmitForm()
        {
            textField.text = textField.text.Trim();
            var form = new WWWForm();
            form.AddField("access_key", "01b8392c-4a7e-4099-bf6b-e71b629add76");
            
            form.AddField("build", Application.version);
            form.AddField("screen-width", $"{Screen.width}");
            form.AddField("screen-height", $"{Screen.height}");
            
            form.AddField("email", emailField.text);
            form.AddField("description", textField.text);
            form.AddField("type", isBugToggle.isOn ? "Bug" : "Feedback");
            using(var w = UnityWebRequest.Post(URL, form))
            {
                yield return w.SendWebRequest();
                if (w.result != UnityWebRequest.Result.Success)
                {
                    formSubmitStatus.color = errorColor;
                    formSubmitStatus.text = $"Unable to submit. Error Code: {w.responseCode}.";
                    Debug.Log(w.error);
                }
                else
                {
                    formSubmitStatus.color = successColor;
                    formSubmitStatus.text = $"Form successfully submitted.";
                }
            }
        }
    }
}