using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Shared.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace Game.Scripts.Shared.Ui.Forms
{
//todo: add sort by inf/err etc
/// <summary> In-game logs UI. Displays logs in builds </summary>
public class LogForm : MonoBehaviour
{
	[Header("Components")]
	[SerializeField]
	private ScrollRect _scrollRect;
	
	[Header("Toggle button")]
	[SerializeField]
	private KeyCode _toggleKey = KeyCode.Tab;
	[SerializeField]
	private Button _toggleButton;
	[SerializeField]
	private TextMeshProUGUI _toggleText;
	
	[Header("Clear logs button")]
	[SerializeField]
	private KeyCode _clearKey = KeyCode.Delete;
	[SerializeField]
	private Button _clearButton;
	[SerializeField]
	private TextMeshProUGUI _clearText;
	
	[Header("Logs")]
	[SerializeField]
	private GameObject _logsGroup;
	[SerializeField]
	private TextMeshProUGUI _logPrefab;
	[SerializeField]
	private Transform _logsParentTf;
	[SerializeField]
	private Image _LogsBg;
	
	[Header("Colors")]
	[SerializeField]
	private Color _infoColor = Color.white;
	[SerializeField]
	private Color _errorColor = Color.red;
	
	[Header("Last log")]
	[SerializeField]
	private CanvasGroup _lastLogCanvasGroup;
	[SerializeField]
	private TextMeshProUGUI _lastLogText;
	[SerializeField]
	private Image _lastLogBg;
	
	[Header("Params")]
	[SerializeField]
	private bool _isInfiniteLogsCount = false;
	[SerializeField]
	private int _maxLogsCount = 100;
	[SerializeField]
	private float _lastLogHideDelaySec = 5f;
	[SerializeField, Range(0f, 1f)]
	private float _bgOpacity = 0.7f;
	
	//--------------------------------------------------------
	//--------------------------------------------------------
	private List<TextMeshProUGUI> _logs = new List<TextMeshProUGUI>();

	private Coroutine _hideLastLogDelayCo;
	private Coroutine _scrollCo;
	
	private void Start()
	{
		_logPrefab.gameObject.SetActive(false);
		_toggleButton.onClick.AddListener(ToggleShowHide);
		_clearButton.onClick.AddListener(ClearAllLogs);
		
		_lastLogCanvasGroup.gameObject.SetActive(!_logsGroup.activeSelf);
		_lastLogCanvasGroup.alpha = 0f;

		_toggleText.text = $"Logs [{_toggleKey}]";
		_clearText.text  = $"Clear [{_clearKey}]";
	}

	private void Update()
	{
		if (Input.GetKeyDown(_toggleKey))
		{
			ToggleShowHide();
		}
		if (Input.GetKeyDown(_clearKey))
		{
			ClearAllLogs();
		}
	}
	
	public void Inf(string message)
	{
		SpawnLog(message, _infoColor);
		SetLastLog(message, _infoColor);
		TryClearOldLogs();
	}
	
	public void Err(string message)
	{
		SpawnLog(message, _errorColor);
		SetLastLog(message, _errorColor);
		TryClearOldLogs();
	}

	private void ToggleShowHide()
	{
		_logsGroup.SetActive(!_logsGroup.activeSelf);
		_lastLogCanvasGroup.gameObject.SetActive(!_logsGroup.activeSelf);

		if (_logsGroup.activeSelf)
		{
			ScrollToBottom();
		}
	}

	private void ClearAllLogs()
	{
		foreach (var log in _logs)
		{
			Destroy(log.gameObject);
		}
		_logs.Clear();
		_lastLogCanvasGroup.alpha = 0f;
	}

	private void SpawnLog(string message, Color color)
	{
		var logInst = Instantiate(_logPrefab, _logsParentTf);
		logInst.text  = message;
		logInst.color = color;
		logInst.gameObject.SetActive(true);
		
		_logs.Add(logInst);
		
		ScrollToBottom();
	}

	private void ScrollToBottom()
	{
		if (_scrollCo != null)
		{
			StopCoroutine(_scrollCo);
		}

		_scrollCo = StartCoroutine(ScrollToBottomCo());
	}

	private void SetLastLog(string message, Color color)
	{
		_lastLogText.text         = message;
		_lastLogText.color        = color;
		_lastLogCanvasGroup.alpha = 1f;

		if (_hideLastLogDelayCo != null)
		{
			StopCoroutine(_hideLastLogDelayCo);
		}
		_hideLastLogDelayCo = StartCoroutine(HideLastLogDelayCo());
	}

	private void TryClearOldLogs()
	{
		if (_isInfiniteLogsCount || _logs.Count <= _maxLogsCount)
		{
			return;
		}
		int toDeleteCount = _logs.Count - _maxLogsCount;

		for (int i = 0; i < toDeleteCount; i++)
		{
			Destroy(_logs[0].gameObject);
			_logs.RemoveAt(0);
		}
		ScrollToBottom();
	}

	private IEnumerator HideLastLogDelayCo()
	{
		yield return new WaitForSecondsRealtime(_lastLogHideDelaySec);
		_lastLogCanvasGroup.alpha = 0f;
		_hideLastLogDelayCo       = null;
	}
	
	private IEnumerator ScrollToBottomCo()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		_scrollRect.ScrollToBottom();
		_scrollCo = null;
	}
	
	//--------------------------------------------------------
	//--------------------------------------------------------

	private void OnValidate()
	{
		if (!_lastLogBg || !_LogsBg)
		{
			return;
		}
		_LogsBg.color    = new Color(_LogsBg.color.r, _LogsBg.color.g, _LogsBg.color.b, _bgOpacity);
		_lastLogBg.color = new Color(_lastLogBg.color.r, _lastLogBg.color.g, _lastLogBg.color.b, _bgOpacity);
	}

	private void OnDestroy()
	{
		_toggleButton.onClick.RemoveListener(ToggleShowHide);
		_clearButton.onClick.RemoveListener(ClearAllLogs);
	}
}
}