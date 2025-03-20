using System;
using Game.Scripts.Foyer.Network.Service.Auth;
using Game.Scripts.Shared.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Foyer.Ui.Auth
{
    public class ForgotPasswordConfirmationForm : Form
    {
        public event Action EvCancelClicked;
        public event Action EvRequestSentSuccessfully;
        
        [SerializeField] private TMP_InputField _tokenInputField;
        [SerializeField] private Button _btnSubmit;
        [SerializeField] private Button _btnCancel;
        
        protected override void AddListeners()
        {
            _btnSubmit.onClick.AddListener(SendConfirmationRequest);
            _btnCancel.onClick.AddListener(InvokeEvCancelClicked);
        }

        protected override void RemoveListeners()
        {
            _btnSubmit.onClick.RemoveListener(SendConfirmationRequest);
            _btnCancel.onClick.RemoveListener(InvokeEvCancelClicked);
        }

        public override void SetInteractable(bool isInteractable)
        {
            _btnSubmit.interactable = isInteractable;
            _btnCancel.interactable = isInteractable;
            _tokenInputField.interactable = isInteractable;
        }

        private async void SendConfirmationRequest()
        {
            try
            {
                SetInteractable(false);
                var token = _tokenInputField.text;
                await AuthService.ForgotPasswordConfirm(token);
                Log.Inf("Reset password confirmation", "New password was sent through the email");
                EvRequestSentSuccessfully?.Invoke();
            }
            catch (Exception e)
            {
                Log.Err("Reset password confirmation", e.Message);
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