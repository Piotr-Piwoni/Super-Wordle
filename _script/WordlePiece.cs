using Godot;

namespace Mummoth;

public partial class WordlePiece : ColorRect
{
	[Signal]
	public delegate void SelectedEventHandler(WordlePiece obj);

	[Signal]
	public delegate void TextChangedEventHandler(WordlePiece obj);

	public bool HasText => _LineEdit?.Text.Length > 0;

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
}
