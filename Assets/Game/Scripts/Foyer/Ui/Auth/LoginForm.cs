using System;
using Game.Scripts.Foyer.Network.Dto.Auth;
using Game.Scripts.Foyer.Network.Service.Auth;
using Game.Scripts.Shared.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Foyer.Ui.Auth
{
    public class LoginForm : Form
    {
        public event Action EvAuthenticationComplete;
        public event Action EvOpenRegisterForm;
        public event Action EvOpenForgotPasswordForm;

        [SerializeField] private TMP_InputField _emailInputField;
        [SerializeField] private TMP_InputField _passwordInputField;
        [SerializeField] private Button _btnLogin;
        [SerializeField] private Button _btnOpenRegisterForm;
        [SerializeField] private Button _btnOpenForgotPassword;

        protected override void AddListeners()
        {
            _btnLogin.onClick.AddListener(Login);
            _btnOpenRegisterForm.onClick.AddListener(InvokeEvOpenRegisterForm);
            _btnOpenForgotPassword.onClick.AddListener(InvokeEvOpenForgotPasswordForm);
        }

        private async void Start()
        {
            var isLoggedIn = await AuthService.TestAuth();
            if (isLoggedIn)
            {
                EvAuthenticationComplete?.Invoke();
            }
        }

        private async void Login()
        {
            try
            {
                SetInteractable(false);
                var userDto = new SignInUserDto {email = _emailInputField.text, password = _passwordInputField.text};
                await AuthService.SignIn(userDto);
                EvAuthenticationComplete?.Invoke();
            }
            catch (Exception e)
            {
                Log.Err("Authentication", e.Message);
            }
            finally
            {
                SetInteractable(true);
            }

        }

        private void InvokeEvOpenForgotPasswordForm()
        {
            EvOpenForgotPasswordForm?.Invoke();
        }

        private void InvokeEvOpenRegisterForm()
        {
            EvOpenRegisterForm?.Invoke();
        }

        public override void SetInteractable(bool isInteractable)
        {
            _btnLogin.interactable = isInteractable;
            _emailInputField.interactable = isInteractable;
            _passwordInputField.interactable = isInteractable;
            _btnOpenRegisterForm.interactable = isInteractable;
            _btnOpenForgotPassword.interactable = isInteractable;
        }

        protected override void RemoveListeners()
        {
            _btnLogin.onClick.RemoveListener(Login);
            _btnOpenRegisterForm.onClick.RemoveListener(InvokeEvOpenRegisterForm);
            _btnOpenForgotPassword.onClick.RemoveListener(InvokeEvOpenForgotPasswordForm);
        }
    }
}