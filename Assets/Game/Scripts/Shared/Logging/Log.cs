using System;
using Game.Scripts.Shared.Ui.Forms;
using UnityEngine;

namespace Game.Scripts.Shared.Logging
{
/// <summary> Responsible for sending logs to in-game UI. </summary>
public static class Log
{
	private static LogForm _form;

	//--------------------------------------------------------
	//--------------------------------------------------------
	
	public static void Init(LogForm form)
	{
		_form = form;
	}

	public static void Inf(string tag, string message, bool withDebugLog = true)
	{
		string log = $"[{DateTime.Now:hh:mm:ss}] [{tag}] {message}";

		if (withDebugLog)
		{
			Debug.Log(log);
		}
		_form.Inf(log);
	}
	
	/*
	public static void Inf(object classContext, string message, bool withDebugLog = true)
	{
		string log = $"[{DateTime.Now:hh:mm:ss}] [{classContext.GetType().Name}] {message}";

		if (withDebugLog)
		{
			Debug.Log(log);
		}
		_form.Inf(log);
	}*/
	
	public static void Err(string tag, string message, bool withDebugLog = true)
	{
		string log = $"[{DateTime.Now:hh:mm:ss}] [{tag}] {message}";

		if (withDebugLog)
		{
			Debug.LogError(log);
		}
		_form.Err(log);
	}
	
	public static void Err(object classContext, string message, bool withDebugLog = true)
	{
		string log = $"[{DateTime.Now:hh:mm:ss}] [{classContext.GetType().Name}] {message}";

		if (withDebugLog)
		{
			Debug.LogError(log);
		}
		_form.Err(log);
	}
	
	//todo: Warn
}
}