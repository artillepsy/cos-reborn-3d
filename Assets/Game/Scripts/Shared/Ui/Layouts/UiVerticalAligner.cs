using System.Collections.Generic;
using BlahEditor.Attributes;
using Shared.Util;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Shared.Ui.Layouts
{
public class UiVerticalAligner : MonoBehaviour
{
	[Header("Params")]
	[SerializeField]
	private EMode _mode;
	[SerializeField]
	private float _space;
	[SerializeField]
	private float _offsetTop;
	[SerializeField]
	private float _offsetBot;
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
		
		if (_mode == EMode.Top)
		{
			Align(-_offsetTop, _space, -1, 1);
		}
		else if (_mode == EMode.Middle)
		{
			float height     = Container.rect.height;
			float reqHeight  = GetRequiredHeight() + (_items.Count - 1) * _space;
			float startPosY = (height - reqHeight) / 2f + _offsetTop - _offsetBot;
			Align(-startPosY, _space, -1, 1);
		}
		else if (_mode == EMode.Bot)
		{
			Align(_offsetBot, _space, 1, 0);
		}
		else if (_mode == EMode.Resize)
		{
			float height = Align(-_offsetTop, _space, -1, 1);
			height               += _offsetBot;
			Container.sizeDelta =  Container.sizeDelta.WithY(height);
		}
		else if (_mode == EMode.Fill)
		{
			float height     = Container.rect.width;
			float reqHeight  = GetRequiredHeight() + _offsetTop + _offsetBot;
			float space     = _items.Count > 1 ? (height - reqHeight) / (_items.Count-1) : 0;
			float startPosY = _offsetTop - _offsetBot;
			Align(-startPosY, space, -1, 1);
		}
	}

	private float Align(float startPosY, float space, int stepSign, float anchor)
	{
		float posY = startPosY;
		foreach (var item in _items)
		{
			SetAnchor(item, anchor);
			float bias = Mathf.Abs(anchor - item.pivot.y);
			posY                  += bias * item.rect.height * stepSign;
			item.anchoredPosition =  item.anchoredPosition.WithY(posY);
			posY                  += ((1f - bias) * item.rect.height + space) * stepSign;
		}
		return (posY - space * stepSign) * stepSign;
	}

	private float GetRequiredHeight()
	{
		float height = 0;
		foreach (var item in _items)
			height += item.rect.height;
		return height;
	}


	private void SetAnchor(RectTransform rt, float anchor)
	{
		rt.anchorMin = new Vector2(rt.anchorMin.x, anchor);
		rt.anchorMax = new Vector2(rt.anchorMax.x, anchor);
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
		Top,
		Middle,
		Bot,
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
		objs.Add(Container);
		Undo.RecordObjects(objs.ToArray(), $"{nameof(UiHorizontalAligner)}. Align");
		Align();
		EditorUtility.SetDirty(this);
	}
#endif
}
}