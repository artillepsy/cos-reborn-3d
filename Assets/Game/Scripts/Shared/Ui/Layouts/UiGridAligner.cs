using System;
using System.Collections.Generic;
using BlahEditor.Attributes;
using UnityEditor;
using UnityEngine;

namespace Shared.Ui.Layouts
{
public class UiGridAligner : MonoBehaviour
{
	[Header("Params")]
	[SerializeField]
	private EMode _mode;
	[SerializeField, EnabledIf(nameof(EditorRowsEnabled))]
	private int _rowsCount;
	[SerializeField, EnabledIf(nameof(EditorColumnsEnabled))]
	private int _columnsCount;
	[SerializeField]
	private Vector2 _cellSize;
	[SerializeField]
	private float _offsetTop;
	[SerializeField]
	private float _offsetBot;
	[SerializeField]
	private float _offsetLeft;
	[SerializeField]
	private float _offsetRight;
	[SerializeField]
	private Vector2 _space;
	[SerializeField]
	private List<RectTransform> _ignoreList;

	[Header("Components")]
	[SerializeField, Disabled]
	private RectTransform _holderRT;

	//-----------------------------------------------------------
	//-----------------------------------------------------------
	public void Align()
	{
		var rt = transform;

		int row        = 0;
		int column     = 0;
		int itemsCount = 0;

		for (var i = 0; i < rt.childCount; i++)
		{
			var child = (RectTransform)rt.GetChild(i);
			if (_ignoreList.Contains(child))
				continue;
			if (!child.gameObject.activeSelf)
				continue;

			itemsCount += 1;
			
			child.anchorMin = new Vector2(0, 1);
			child.anchorMax = new Vector2(0, 1);
			var childPivot = child.pivot;
			var childSize  = child.rect.size;
			child.anchoredPosition = new Vector2(
				_offsetLeft + _cellSize.x * column + _space.x * column + childSize.x * childPivot.x,
				-(_offsetTop + _cellSize.y * row + _space.y * row + childSize.y * (1f - childPivot.y))
			);

			if (_mode == EMode.FixedColumns)
			{
				column += 1;
				if (column == _columnsCount)
				{
					column =  0;
					row    += 1;
				}
			}
			else if (_mode == EMode.FixedRows)
			{
				row += 1;
				if (row == _rowsCount)
				{
					row    =  0;
					column += 1;
				}
			}
			else if (_mode == EMode.BlocksHorizontal)
			{
				if (itemsCount < _columnsCount * _rowsCount)
				{
					column += 1;
					if (column == _columnsCount)
					{
						row    = 1;
						column = 0;
					}
				}
				else 
				{
					row += 1;
					if (row == _rowsCount)
					{
						row    =  0;
						column += 1;
					}
				}
			}
		}

		Vector2 holderSize;
		if (_mode == EMode.FixedColumns)
		{
			holderSize = GetSize(
				_columnsCount,
				itemsCount == 0 ? 0 : (itemsCount % _columnsCount == 0 ? row : row+1)
			);
		}
		else if (_mode == EMode.FixedRows)
		{
			holderSize = GetSize(column + 1, _rowsCount);
		}
		else if (_mode == EMode.BlocksHorizontal)
		{
			if (itemsCount < _columnsCount)
				holderSize = GetSize(column, 1);
			else if (itemsCount < _columnsCount * _rowsCount)
				holderSize = GetSize(_columnsCount, _rowsCount);
			else if (row == 0)
				holderSize = GetSize(column, _rowsCount);
			else
				holderSize = GetSize(column + 1, _rowsCount);
		}
		else
			throw new NotImplementedException();
		_holderRT.sizeDelta = holderSize;
	}
    
	private Vector2 GetSize(int column, int row)
		=> new(
			_offsetLeft + _offsetRight + column * _cellSize.x + Math.Max(0, column - 1) * _space.x,
			_offsetTop + _offsetBot + row * _cellSize.y + Math.Max(0, row - 1) * _space.y
		);

	private enum EMode
	{
		FixedColumns,
		FixedRows,
		BlocksHorizontal
	}
	
	//-----------------------------------------------------------
	//-----------------------------------------------------------
	private bool EditorColumnsEnabled => _mode is EMode.FixedColumns or EMode.BlocksHorizontal;
	private bool EditorRowsEnabled    => _mode is EMode.FixedRows or EMode.BlocksHorizontal;
	
	private void OnValidate()
	{
		_holderRT = GetComponent<RectTransform>();
	}

#if UNITY_EDITOR
	[SerializeField, Button("Align", nameof(EditorAlign))]
	private bool _editorAlignButton;
	private void EditorAlign()
	{
		var items = new List<RectTransform>();
		for (var i = 0; i < _holderRT.childCount; i++)
		{
			var child = _holderRT.GetChild(i);
			items.Add(child.GetComponent<RectTransform>());
		}
		items.Add(_holderRT);

		Undo.RecordObjects(items.ToArray(), $"{nameof(UiGridAligner)}. Align");
		Align();
		EditorUtility.SetDirty(this);
	}
#endif
}
}