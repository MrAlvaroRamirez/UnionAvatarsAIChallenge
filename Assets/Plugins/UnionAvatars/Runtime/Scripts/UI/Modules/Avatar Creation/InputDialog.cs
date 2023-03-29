using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

namespace UnionAvatars.UI
{
    public class InputDialog : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dialogText;
        [SerializeField] private TMP_InputField textField;
        [SerializeField] private Button backButton;
        [SerializeField] private Button yesButton;

        public void SetupInputDialog(string message, Action backAction, Action<string> yesAction)
        {
            dialogText.text = message;
            backButton.onClick.AddListener(new UnityAction(backAction));
            yesButton.onClick.AddListener(new UnityAction(() => yesAction(textField.text)));
        }

        public void Close()
        {
            Destroy(gameObject);
        }
    }
}