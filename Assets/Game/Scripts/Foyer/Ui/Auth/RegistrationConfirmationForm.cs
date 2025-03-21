using System;
using Game.Scripts.Foyer.Network.Service.Auth;
using Game.Scripts.Shared.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Foyer.Ui.Auth
{
    public class RegistrationConfirmationForm : Form
    {
        public event Action EvCloseForm;
        
        [SerializeField] private Button _btnVerificationSubmit;
        [SerializeField] private Button _btnVerificationCancel;
        [SerializeField] private TMP_InputField _verificationTokenInputField;
        protected override void AddListeners()
        {
            _btnVerificationSubmit.onClick.AddListener(SubmitConfirmation);
            _btnVerificationCancel.onClick.AddListener(InvokeEvCloseForm);
        }

        private void InvokeEvCloseForm()
        {
            EvCloseForm?.Invoke();
        }

        private async void SubmitConfirmation()
        {
            try
            {
                SetInteractable(false);
                await AuthService.ConfirmRegistration(_verificationTokenInputField.text);
                EvCloseForm?.Invoke();
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

        protected override void RemoveListeners()
        {
            _btnVerificationSubmit.onClick.RemoveListener(SubmitConfirmation);
            _btnVerificationCancel.onClick.RemoveListener(InvokeEvCloseForm);
        }

        public override void SetInteractable(bool isInteractable)
        {
            _btnVerificationSubmit.interactable = isInteractable;
            _btnVerificationCancel.interactable = isInteractable;
            _verificationTokenInputField.interactable = isInteractable;
        }
    }
}