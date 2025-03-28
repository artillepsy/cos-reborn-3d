using System;
using Foyer.Network.Dto.Auth;
using Foyer.Network.Service.Auth;
using Shared.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Foyer.Ui.Auth
{
    public class ChangePasswordForm : Form
    {
        public event Action EvCancelClicked;
        private const string Tag = "Change password";
        
        [SerializeField] private TMP_InputField _oldPasswordInputField;
        [SerializeField] private TMP_InputField _newPasswordInputField;
        [SerializeField] private TMP_InputField _confirmNewPasswordInputField;
        [SerializeField] private Button _btnSubmit;
        [SerializeField] private Button _btnCancel;
        
        protected override void AddListeners()
        {
            _btnSubmit.onClick.AddListener(SendChangePasswordRequest);
            _btnCancel.onClick.AddListener(InvokeEvCancelClicked);
        }

        protected override void RemoveListeners()
        {
            _btnSubmit.onClick.RemoveListener(SendChangePasswordRequest);
            _btnCancel.onClick.RemoveListener(InvokeEvCancelClicked);
        }

        public override void SetInteractable(bool isInteractable)
        {
            _oldPasswordInputField.interactable = isInteractable;
            _newPasswordInputField.interactable = isInteractable;
            _confirmNewPasswordInputField.interactable = isInteractable;
            _btnSubmit.interactable = isInteractable;
            _btnCancel.interactable = isInteractable;
        }

        private async void SendChangePasswordRequest()
        {
            var newPassword = _newPasswordInputField.text;
            var newPasswordRepeat = _confirmNewPasswordInputField.text;
            if (newPassword != newPasswordRepeat)
            {
                Log.Err(Tag, "Invalid repeat password");
                return;
            }
            try
            {
                SetInteractable(false);
                await AuthService.ChangePassword(new ChangePasswordDto
                {
                    newPassword = newPassword,
                    oldPassword = _oldPasswordInputField.text
                });
            }
            catch (Exception e)
            {
                Log.Err(Tag, e.Message);
            }
            finally
            {
                SetInteractable(true);
            }
        }

        private void InvokeEvCancelClicked()
        {
            EvCancelClicked?.Invoke();
        }
    }
}