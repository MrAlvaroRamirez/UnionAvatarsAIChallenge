using System.Threading.Tasks;
using TMPro;
using UnionAvatars.API;
using UnityEngine;
using UnityEngine.UI;

namespace UnionAvatars.UI
{
    public class RegisterUI : UIModule
    {
        [SerializeField] private GameObject userInput;
        [SerializeField] private GameObject loadingUI;
        [SerializeField] private GameObject verificationUI;
        [SerializeField] private TextMeshProUGUI verificationEmailText;
        [Header ("Input Fields")]
        [SerializeField] private TMP_InputField usernameField;
        [SerializeField] private TMP_InputField emailField;
        [SerializeField] private TMP_InputField passwordField;
        [SerializeField] private TMP_InputField confirmPasswordField;
        [SerializeField] private Toggle termsToggle;
        [Header ("Input Warnings")]
        [SerializeField] private TextMeshProUGUI usernameWarning;
        [SerializeField] private TextMeshProUGUI emailWarning;
        [SerializeField] private TextMeshProUGUI passwordWarning;
        [SerializeField] private TextMeshProUGUI confirmPasswordWarning;
        [Header ("Buttons")]
        [SerializeField] private Button submitButton;
        [SerializeField] private Button backButton;

        private void Start()
        {  
            usernameField.onEndEdit.AddListener((s) => _ = CheckUsername());
            emailField.onEndEdit.AddListener((s) => _ = CheckEmail());
            passwordField.onEndEdit.AddListener((s) => _ = CheckPassword());
            confirmPasswordField.onEndEdit.AddListener((s) => _ = CheckConfirmationPassword());

            InputNavigator inputNavigator = GetComponent<InputNavigator>();
            usernameField.onSubmit.AddListener(inputNavigator.SelectNextElement);
            emailField.onSubmit.AddListener(inputNavigator.SelectNextElement);
            passwordField.onSubmit.AddListener(inputNavigator.SelectNextElement);
        }

        public async Task<bool> CheckUsername()
        {
            if(usernameField.text == "")
            {
                usernameWarning.text = "This field is required";
                usernameWarning.gameObject.SetActive(true);
                return false;
            }

            string response = await WebRequests.GetRaw(uiManager.session.UnionAvatarsSession.Url + "users/exists/username/" + usernameField.text,
                                                       default,
                                                       uiManager.session.UnionAvatarsSession);

            if (response == "true")
            {
                usernameWarning.text = "This username is already taken";
                usernameWarning.gameObject.SetActive(true);
                return false;
            }

            usernameWarning.gameObject.SetActive(false);
            return true;
        }

        public async Task<bool> CheckEmail()
        {
            if(emailField.text == "")
            {
                emailWarning.text = "This field is required";
                emailWarning.gameObject.SetActive(true);
                return false;
            }

            string response = await WebRequests.GetRaw(uiManager.session.UnionAvatarsSession.Url + "users/exists/email/" + emailField.text,
                                                       default,
                                                       uiManager.session.UnionAvatarsSession);

            if (response == "true")
            {
                emailWarning.text = "This email is already in use";
                emailWarning.gameObject.SetActive(true);
                return false;
            }
            
            emailWarning.gameObject.SetActive(false);
            return true;
        }

        public bool CheckPassword()
        {
            if(passwordField.text == "")
            {
                passwordWarning.text = "This field is required";
                passwordWarning.gameObject.SetActive(true);
                return false;
            }
           
            passwordWarning.gameObject.SetActive(false);
            return true;

        }

        public bool CheckConfirmationPassword()
        {
            if(confirmPasswordField.text != passwordField.text)
            {
                confirmPasswordWarning.text = "Passwords do not match";
                confirmPasswordWarning.gameObject.SetActive(true);
                return false;
            }
      
            confirmPasswordWarning.gameObject.SetActive(false);
            return true;
        }

        public void ShowTermsOfUse()
        {
            Application.OpenURL("https://app.unionavatars.com/general-terms-of-use");
        }

        public async void Register()
        {
            if(!await CheckUsername()
               || !await CheckEmail()
               || !CheckPassword()
               || !CheckConfirmationPassword())
               return;

            userInput.SetActive(false);
            loadingUI.SetActive(true);
            submitButton.interactable = false;

            bool registered = await uiManager.session.Register(usernameField.text, emailField.text, passwordField.text);

            if(registered)
            {
                verificationEmailText.text += emailField.text;
                verificationUI.SetActive(true);
                loadingUI.SetActive(false);
            }
            else
            {
                userInput.SetActive(true);
                loadingUI.SetActive(false);
                submitButton.interactable = true;
                return;
            }
        }
    }
}