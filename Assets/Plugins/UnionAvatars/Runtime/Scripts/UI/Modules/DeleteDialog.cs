using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

namespace UnionAvatars.UI
{
    public class DeleteDialog : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dialogText;
        [SerializeField] private Button noButton;
        [SerializeField] private Button yesButton;

        public void SetupDeleteDialog(string avatarName, Action noAction, Action yesAction)
        {
            dialogText.text += "<b>" + avatarName + "</b>";
            noButton.onClick.AddListener(new UnityAction(noAction));
            yesButton.onClick.AddListener(new UnityAction(yesAction));
        }

        public void Close()
        {
            Destroy(gameObject);
        }
    }
}