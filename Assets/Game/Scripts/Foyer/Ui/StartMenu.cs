using Game.Scripts.Foyer.Ui.Auth;
using Game.Scripts.Foyer.Ui.Lobby;
using UnityEngine;

namespace Game.Scripts.Foyer.Ui
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