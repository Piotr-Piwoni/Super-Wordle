using System;
using Godot;

namespace Mummoth;

public partial class WordFragment : ColorRect
{
	[Signal]
	public delegate void SelectedEventHandler(WordFragment obj);

	[Signal]
	public delegate void TextChangedEventHandler(WordFragment obj);

	public bool HasText => _LineEdit?.Text.Length > 0;
	public WordFragmentState State { get; private set; } = WordFragmentState.None;
	public string Text => _LineEdit?.Text;

	private LineEdit _LineEdit;


	public override void _Ready()
	{
		_LineEdit = GetNode<LineEdit>("LineEdit");
		_LineEdit.TextChanged += OnTextChanged;
		_LineEdit.EditingToggled += OnEditingToggled;
	}


	private void OnEditingToggled(bool toggledOn)
	{
		if (toggledOn)
			EmitSignalSelected(this);
	}

	private void OnTextChanged(string newText)
	{
		if (newText.Length <= 0) return;
		EmitSignalTextChanged(this);
	}

	public void SetEditingMode(bool mode)
	{
		if (mode)
			_LineEdit?.Edit();
		else
			_LineEdit?.Unedit();
	}

	public void ClearText()
	{
		if (_LineEdit == null) return;
		_LineEdit.Text = string.Empty;
	}

	public void UpdateState(WordFragmentState state)
	{
		Color = state switch
		{
				WordFragmentState.None or WordFragmentState.Incorrect => Colors.White,
				WordFragmentState.Correct => Colors.Green,
				WordFragmentState.WrongPosition => Colors.Yellow,
				_ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
		};
	}


	public enum WordFragmentState
	{
		None = 0,
		Incorrect = 1,
		Correct = 2,
		WrongPosition = 3,
	}
}
