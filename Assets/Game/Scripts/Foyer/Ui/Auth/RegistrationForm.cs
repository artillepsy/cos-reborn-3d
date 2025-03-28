using System;
using System.IO;
using Foyer.Network.Dto.Auth;
using Foyer.Network.Service.Auth;
using Shared.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Foyer.Ui.Auth
{
    public class RegistrationForm : Form
    {
        public event Action EvOpenActivateAccountForm;
        public event Action EvCancelRegister;
        
        [SerializeField] private Button _btnRegister;
        [SerializeField] private Button _btnRegisterActivateAccount;
        [SerializeField] private Button _btnRegisterCancel;

        [SerializeField] private TMP_InputField _registerEmailInputField;
        [SerializeField] private TMP_InputField _registerPlayerNameInputField;
        [SerializeField] private TMP_InputField _registerPasswordInputField;
        [SerializeField] private TMP_InputField _registerConfirmPasswordInputField;

        private void InvokeEvOpenActivateAccountForm()
        {
            EvOpenActivateAccountForm?.Invoke();
        }
        private void InvokeEvCancelRegister()
        {
            EvCancelRegister?.Invoke();
        }

        private async void Register()
        {
            try
            {
                SetInteractable(false);
                if (_registerConfirmPasswordInputField.text != _registerPasswordInputField.text)
                {
                    throw new InvalidDataException("Password doesn't match");
                }

                var userDto = new SignUpUserDto
                {
                    email = _registerEmailInputField.text,
                    password = _registerPasswordInputField.text,
                    playerName = _registerPlayerNameInputField.text
                };
                await AuthService.SignUp(userDto);
                EvOpenActivateAccountForm?.Invoke();
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
        protected override void AddListeners()
        {
            _btnRegister.onClick.AddListener(Register);
            _btnRegisterActivateAccount.onClick.AddListener(InvokeEvOpenActivateAccountForm);
            _btnRegisterCancel.onClick.AddListener(InvokeEvCancelRegister);
        }

        protected override void RemoveListeners()
        {
            _btnRegister.onClick.RemoveListener(Register);
            _btnRegisterActivateAccount.onClick.RemoveListener(InvokeEvOpenActivateAccountForm);
            _btnRegisterCancel.onClick.RemoveListener(InvokeEvCancelRegister);
        }

        public override void SetInteractable(bool isInteractable)
        {
            _btnRegister.interactable = isInteractable;
            _btnRegisterActivateAccount.interactable = isInteractable;
            _btnRegisterCancel.interactable = isInteractable;
            _registerEmailInputField.interactable = isInteractable;
            _registerPlayerNameInputField.interactable = isInteractable;
            _registerPasswordInputField.interactable = isInteractable;
            _registerConfirmPasswordInputField.interactable = isInteractable;
        }
    }
}