using Game.Scripts.Shared.Logging;
using Game.Scripts.Shared.Ui.Forms;
using UnityEngine;

namespace Game.Scripts.Shared.AppStartup
{
    public class LogStartup : MonoBehaviour
    {
        [SerializeField]
        private LogForm _logForm;
	
        //--------------------------------------------------------
        //--------------------------------------------------------
	
        private void Awake()
        {
            Log.Init(_logForm);
        }
    }
}