using Foyer.Ui.Auth;
using Foyer.Ui.Lobby;
using UnityEngine;

namespace Foyer.Ui
{
    public class StartMenu : MonoBehaviour
    {
        [SerializeField] private LoginForm _authenticationForm;
        [SerializeField] private LobbiesForm _lobbiesForm;

        private void Awake()
        {
            _authenticationForm.EvAuthenticationComplete += OnAuthenticationComplete;
        }

        private void OnAuthenticationComplete()
        {
            _authenticationForm.gameObject.SetActive(false);
            _lobbiesForm.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            _authenticationForm.EvAuthenticationComplete -= OnAuthenticationComplete;
        }
    }
}