using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;

namespace Mummoth;

public partial class WordleWord : Control
{
	[Signal]
	public delegate void WordSubmittedEventHandler(string submittedWord);

	public List<WordFragment> WordFragments { get; } = [];

	[Export]
	private PackedScene _WordFragmentComponent;

	private int _SelectedFragment;


	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("ui_text_submit"))
			SubmitWord();

		if (!Input.IsKeyPressed(Key.Backspace) ||
			WordFragments[_SelectedFragment].HasText) return;

		if (_SelectedFragment - 1 < 0) return;
		WordFragments[_SelectedFragment].SetEditingMode(false);

		_SelectedFragment--;
		WordFragments[_SelectedFragment].SetEditingMode(true);
	}

	private void SubmitWord()
	{
		if (_SelectedFragment != WordFragments.Count - 1 ||
			!WordFragments.Last().HasText) return;

		// Construct the full word from the fragments.
		var finalWord = new StringBuilder();
		foreach (WordFragment fragment in WordFragments)
			finalWord.Append(fragment.Text);

		EmitSignalWordSubmitted(finalWord.ToString());
	}


	private void OnTextChanged(WordFragment obj)
	{
		if (obj == WordFragments.Last()) return;
		WordFragments[_SelectedFragment].SetEditingMode(false);

		_SelectedFragment++;
		WordFragments[_SelectedFragment].SetEditingMode(true);
	}

	private void OnSelected(WordFragment obj)
	{
		int index = WordFragments.IndexOf(obj);
		if (index == _SelectedFragment) return;

		foreach (WordFragment piece in WordFragments)
			piece.SetEditingMode(false);

		for (var i = 0; i < WordFragments.Count; i++)
		{
			WordFragment piece = WordFragments[i];
			if (piece.HasText) continue;

			piece.SetEditingMode(true);
			_SelectedFragment = i;
			return;
		}
	}

	public void Generate(int letterAmount)
	{
		var container = GetChild<HBoxContainer>(0);
		for (var i = 0; i < letterAmount; i++)
		{
			var piece = _WordFragmentComponent.Instantiate<WordFragment>();
			container.AddChild(piece);
			WordFragments.Add(piece);
		}

		foreach (WordFragment piece in WordFragments)
		{
			piece.Selected += OnSelected;
			piece.TextChanged += OnTextChanged;
		}
	}
}
