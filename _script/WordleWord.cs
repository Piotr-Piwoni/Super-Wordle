using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Mummoth;

public partial class WordleWord : Control
{
	[Export]
	private PackedScene _WordlePieceScene;

	private readonly List<WordlePiece> _WordlePieces = [];
	private int _SelectedPiece;


	public override void _Input(InputEvent @event)
	{
		if (!Input.IsKeyPressed(Key.Backspace) ||
			_WordlePieces[_SelectedPiece].HasText) return;

		if (_SelectedPiece - 1 < 0) return;
		_WordlePieces[_SelectedPiece].SetEditingMode(false);

		_SelectedPiece--;
		_WordlePieces[_SelectedPiece].SetEditingMode(true);
	}


	private void OnTextChanged(WordlePiece obj)
	{
		if (obj == _WordlePieces.Last()) return;
		_WordlePieces[_SelectedPiece].SetEditingMode(false);

		_SelectedPiece++;
		_WordlePieces[_SelectedPiece].SetEditingMode(true);
	}

	private void OnSelected(WordlePiece obj)
	{
		int pieceIndex = _WordlePieces.IndexOf(obj);
		if (pieceIndex == _SelectedPiece) return;

		foreach (WordlePiece piece in _WordlePieces)
			piece.SetEditingMode(false);

		for (var i = 0; i < _WordlePieces.Count; i++)
		{
			WordlePiece piece = _WordlePieces[i];
			if (piece.HasText) continue;

			piece.SetEditingMode(true);
			_SelectedPiece = i;
			return;
		}
	}

	public void Generate(int letterAmount)
	{
		var container = GetChild<HBoxContainer>(0);
		for (var i = 0; i < letterAmount; i++)
		{
			var piece = _WordlePieceScene.Instantiate<WordlePiece>();
			container.AddChild(piece);
			_WordlePieces.Add(piece);
		}

		foreach (WordlePiece piece in _WordlePieces)
		{
			piece.Selected += OnSelected;
			piece.TextChanged += OnTextChanged;
		}
	}
}
