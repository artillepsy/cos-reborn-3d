using System;
using Foyer.Network.Dto.Auth;
using Foyer.Network.Service.Auth;
using Shared.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Foyer.Ui.Auth
{
    public class ForgotPasswordForm : Form
    {
        public event Action EvRequestSentSuccessfully;
        public event Action EvConfirmPasswordForgottenClicked;
        public event Action EvCancelClicked;
        [SerializeField] private TMP_InputField _emailInputField;
        [SerializeField] private Button _btnSubmit;
        [SerializeField] private Button _btmConfirmPasswordForgotten;
        [SerializeField] private Button _btnCancel;

        protected override void AddListeners()
        {
            _btnSubmit.onClick.AddListener(SendResetPasswordRequest);
            _btnCancel.onClick.AddListener(InvokeEvCancelClicked);
            _btmConfirmPasswordForgotten.onClick.AddListener(InvokeEvConfirmPasswordForgottenClicked);
        }

        protected override void RemoveListeners()
        {
            _btnSubmit.onClick.RemoveListener(SendResetPasswordRequest);
            _btnCancel.onClick.RemoveListener(InvokeEvCancelClicked);
            _btmConfirmPasswordForgotten.onClick.RemoveListener(InvokeEvConfirmPasswordForgottenClicked);
        }

        private void InvokeEvCancelClicked()
        {
            EvCancelClicked?.Invoke();
        }

        private void InvokeEvConfirmPasswordForgottenClicked()
        {
            EvConfirmPasswordForgottenClicked?.Invoke();
        }

        private async void SendResetPasswordRequest()
        {
            try
            {
                SetInteractable(false);
                var email = _emailInputField.text;
                var dto = new ForgotPasswordDto {email = email};
                await AuthService.ForgotPassword(dto);
                Log.Inf("Forgot password", $"Message send to email {email}");
                EvRequestSentSuccessfully?.Invoke();
            }
            catch (Exception e)
            {
                Log.Err("Forgot password", e.Message);
            }
            finally
            {
                SetInteractable(true);
            }
        }

        public override void SetInteractable(bool isInteractable)
        {
            _emailInputField.interactable = isInteractable;
            _btnSubmit.interactable = isInteractable;
            _btnCancel.interactable = isInteractable;
            _btmConfirmPasswordForgotten.interactable = isInteractable;
        }
    }
}