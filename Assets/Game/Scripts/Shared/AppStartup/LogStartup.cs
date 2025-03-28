using Shared.Logging;
using Shared.Ui.Forms;
using UnityEngine;

namespace Shared.AppStartup
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