using UnityEngine;

namespace Game.Scripts.Foyer.Ui.Auth
{
    public class AuthenticationForm : Form
    {
        [SerializeField] private LoginForm _loginForm;
        
        [SerializeField] private RegistrationForm _registrationForm;
        
        [SerializeField] private RegistrationConfirmationForm _registrationConfirmationForm;
        
        [SerializeField] private ForgotPasswordForm _forgotPasswordForm;
        
        [SerializeField] private ForgotPasswordConfirmationForm _forgotPasswordConfirmationForm;

        protected override void AddListeners()
        {
            _loginForm.EvOpenRegisterForm += OpenRegisterForm;
            _loginForm.EvOpenForgotPasswordForm += OpenForgotPasswordForm;
            _registrationForm.EvCancelRegister += OpenLoginForm;
            _registrationForm.EvOpenActivateAccountForm += OpenActivateAccountForm;
            _registrationConfirmationForm.EvCloseForm += OpenRegisterForm;
            _forgotPasswordForm.EvCancelClicked += CloseForgotPasswordForm;
            _forgotPasswordForm.EvRequestSentSuccessfully += OpenForgotPasswordConfirmationForm;
            _forgotPasswordForm.EvConfirmPasswordForgottenClicked += OpenForgotPasswordConfirmationForm;
            _forgotPasswordConfirmationForm.EvCancelClicked += OpenForgotPasswordForm;
            _forgotPasswordConfirmationForm.EvRequestSentSuccessfully += OpenLoginForm;
        }

        protected override void RemoveListeners()
        {
            _loginForm.EvOpenRegisterForm -= OpenRegisterForm;
            _loginForm.EvOpenForgotPasswordForm -= OpenForgotPasswordForm;
            _registrationForm.EvCancelRegister -= OpenLoginForm;
            _registrationForm.EvOpenActivateAccountForm -= OpenActivateAccountForm;
            _registrationConfirmationForm.EvCloseForm -= OpenRegisterForm;
            _forgotPasswordForm.EvCancelClicked -= CloseForgotPasswordForm;
            _forgotPasswordForm.EvRequestSentSuccessfully -= OpenForgotPasswordConfirmationForm;
            _forgotPasswordForm.EvConfirmPasswordForgottenClicked -= OpenForgotPasswordConfirmationForm;
            _forgotPasswordConfirmationForm.EvCancelClicked -= OpenForgotPasswordForm;
            _forgotPasswordConfirmationForm.EvRequestSentSuccessfully -= OpenLoginForm;
        }

        private void OpenActivateAccountForm()
        {
            SwitchRegistrationConfirmForm(true);
        }

        private void OpenRegisterForm()
        {
            SwitchRegistrationForm(true);
        }

        private void OpenLoginForm()
        {
            SwitchRegistrationForm(false);
        }

        private void OpenForgotPasswordForm()
        {
            SwitchForgotPasswordForm(true);
        }

        private void CloseForgotPasswordForm()
        {
            SwitchForgotPasswordForm(false);
        }

        private void OpenForgotPasswordConfirmationForm()
        {
            SwitchPasswordConfirmationForm(true);
        }

        private void SwitchRegistrationForm(bool isEnabled)
        {
            _loginForm.gameObject.SetActive(!isEnabled);
            _registrationForm.gameObject.SetActive(isEnabled);
            _registrationConfirmationForm.gameObject.SetActive(false);
            _forgotPasswordForm.gameObject.SetActive(false);
            _forgotPasswordConfirmationForm.gameObject.SetActive(false);
        }

        private void SwitchRegistrationConfirmForm(bool isEnabled)
        {
            _loginForm.gameObject.SetActive(false);
            _registrationForm.gameObject.SetActive(!isEnabled);
            _registrationConfirmationForm.gameObject.SetActive(isEnabled);
            _forgotPasswordForm.gameObject.SetActive(false);
            _forgotPasswordConfirmationForm.gameObject.SetActive(false);
        }

        private void SwitchForgotPasswordForm(bool isEnabled)
        {
            _loginForm.gameObject.SetActive(!isEnabled);
            _registrationForm.gameObject.SetActive(false);
            _registrationConfirmationForm.gameObject.SetActive(false);
            _forgotPasswordForm.gameObject.SetActive(isEnabled);
            _forgotPasswordConfirmationForm.gameObject.SetActive(false);
        }

        private void SwitchPasswordConfirmationForm(bool isEnabled)
        {
            _loginForm.gameObject.SetActive(false);
            _registrationForm.gameObject.SetActive(false);
            _registrationConfirmationForm.gameObject.SetActive(false);
            _forgotPasswordForm.gameObject.SetActive(!isEnabled);
            _forgotPasswordConfirmationForm.gameObject.SetActive(isEnabled);
        }

        public override void SetInteractable(bool isInteractable)
        {
            _loginForm.SetInteractable(isInteractable);
            _registrationForm.SetInteractable(isInteractable);
            _registrationConfirmationForm.SetInteractable(isInteractable);
            _forgotPasswordForm.SetInteractable(isInteractable);
            _forgotPasswordConfirmationForm.SetInteractable(isInteractable);
        }
    }
}