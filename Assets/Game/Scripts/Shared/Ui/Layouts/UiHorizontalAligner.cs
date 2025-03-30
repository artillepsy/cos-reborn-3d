using System.Collections.Generic;
using BlahEditor.Attributes;
using Shared.Util;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Shared.Ui.Layouts
{
public class UiHorizontalAligner : MonoBehaviour
{
	[Header("Params")]
	[SerializeField]
	private EMode _mode;
	[SerializeField]
	private float _space;
	[SerializeField]
	private float _offsetLeft;
	[SerializeField]
	private float _offsetRight;
	[SerializeField]
	private List<Transform> _ignoreList;

	//-----------------------------------------------------------
	//-----------------------------------------------------------
	private List<RectTransform> _items = new();
	
	public void Align()
	{
		_items.Clear();
		for (var i = 0; i < Container.childCount; i++)
		{
			var child = Container.GetChild(i);
			if (!child.gameObject.activeSelf)
				continue;
			if (_ignoreList?.Contains(child) == true)
				continue;
			_items.Add((RectTransform)child);
		}
		
		if (_mode == EMode.Left)
		{
			Align(_offsetLeft, _space, 1, 0f);
		}
		else if (_mode == EMode.Middle)
		{
			float width     = Container.rect.width;
			float reqWidth  = GetRequiredWidth() + (_items.Count - 1) * _space;
			float startPosX = (width - reqWidth) / 2f + _offsetLeft - _offsetRight;
			Align(startPosX, _space, 1, 0);
		}
		else if (_mode == EMode.Right)
		{
			Align(-_offsetRight, _space, -1, 1f);
		}
		else if (_mode == EMode.Resize)
		{
			float width = Align(_offsetLeft, _space, 1, 0f);
			width               += _offsetRight;
			Container.sizeDelta =  Container.sizeDelta.WithX(width);
		}
		else if (_mode == EMode.Fill)
		{
			float width     = Container.rect.width;
			float reqWidth  = GetRequiredWidth() + _offsetLeft + _offsetRight;
			float space     = _items.Count > 1 ? (width - reqWidth) / (_items.Count-1) : 0;
			float startPosX = _offsetLeft - _offsetRight;
			Align(startPosX, space, 1, 0f);
		}
	}

	private float Align(float startPosX, float space, int stepSign, float anchor)
	{
		float posX = startPosX;
		foreach (var item in _items)
		{
			SetAnchor(item, anchor);
			posX                  += item.pivot.x * item.rect.width * stepSign;
			item.anchoredPosition =  item.anchoredPosition.WithX(posX);
			posX += ((1f - item.pivot.x) * item.rect.width + space) * stepSign;
		}
		return (posX - space * stepSign) * stepSign;
	}

	private float GetRequiredWidth()
	{
		float width = 0;
		foreach (var item in _items)
			width += item.rect.width;
		return width;
	}


	private void SetAnchor(RectTransform rt, float anchor)
	{
		rt.anchorMin = new Vector2(anchor, rt.anchorMin.y);
		rt.anchorMax = new Vector2(anchor, rt.anchorMax.y);
		//rt.pivot     = new Vector2(anchor, rt.pivot.y);
	}
	
	
	private RectTransform _backingContainer;

	private RectTransform Container
	{
		get
		{
			if (_backingContainer == null)
				_backingContainer = GetComponent<RectTransform>();
			return _backingContainer;
		}
	}

	private enum EMode
	{
		Left,
		Middle,
		Right,
		Fill,
		Resize
	}

	//-----------------------------------------------------------
	//-----------------------------------------------------------
#if UNITY_EDITOR
	[SerializeField, Button("Align", nameof(EditorAlign))]
	private bool _editorButtonAlign;

	private void EditorAlign()
	{
		var objs = new List<Object>();
		for (var i = 0; i < Container.childCount; i++)
			objs.Add(Container.GetChild(i));
		Undo.RecordObjects(objs.ToArray(), $"{nameof(UiHorizontalAligner)}. Align");
		Align();
		EditorUtility.SetDirty(this);
	}
#endif
}
}