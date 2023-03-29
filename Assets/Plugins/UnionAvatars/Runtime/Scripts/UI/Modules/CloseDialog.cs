using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

namespace UnionAvatars.UI
{
    public class CloseDialog : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dialogText;
        [SerializeField] private Button noButton;
        [SerializeField] private Button yesButton;

        public void SetupCloseDialog(string message, Action noAction, Action yesAction)
        {
            dialogText.text = message;
            noButton.onClick.AddListener(new UnityAction(noAction));
            yesButton.onClick.AddListener(new UnityAction(yesAction));
        }

        public void Close()
        {
            Destroy(gameObject);
        }
    }
}