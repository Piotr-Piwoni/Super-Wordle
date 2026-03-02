using System;
using System.Linq;
using Godot;
using Godot.Collections;

namespace Mummoth;

public partial class GameManager : Node
{
	public string CurrentWord { get; private set; } = "";

	[Export]
	private PackedScene _WordleWordComponent;
	[Export]
	private CanvasLayer _Canvas;
	[Export(PropertyHint.ArrayType, "Path to the word lists.")]
	private Array<string> _WordLists = [];
	[Export]
	private int _NumberOfRows = 3;

	private WordleWord _CurrentRow;
	private WordleWord[] _WordRows;


	public override void _Ready()
	{
		if (_WordLists.Count == 0)
		{
			GD.PrintErr("At least one word list needs to be assigned!");
			return;
		}

		GD.Randomize();
		PickWord();

		// Created word rows based on the number of rows allowed.
		_WordRows = new WordleWord[_NumberOfRows];
		for (var i = 0; i < _NumberOfRows; i++)
		{
			var wordRow = _WordleWordComponent.Instantiate<WordleWord>();
			_Canvas.GetChild(0).AddChild(wordRow); //< Add it to the Vertical Container.

			wordRow.WordSubmitted += OnWordSubmitted;
			wordRow.Generate(CurrentWord.Length);

			_WordRows[i] = wordRow;
		}

		_CurrentRow = _WordRows.First();
	}


	private void OnWordSubmitted(string submittedWord)
	{
		GD.Print(submittedWord);
	}

	private void PickWord()
	{
		string pickedWordListPath = _WordLists.PickRandom();
		FileAccess wordListFile = FileAccess.Open(pickedWordListPath,
												  FileAccess.ModeFlags.Read);
		// Read file.
		var wordList = string.Empty;
		if (wordListFile.IsOpen())
		{
			wordList = wordListFile.GetAsText();
			wordListFile.Close();
		}

		// Split the file into words.
		string[] words = wordList.Split(["\r\n", "\n", "\r",],
										StringSplitOptions.RemoveEmptyEntries)
								 .Select(w => w.Trim())
								 .ToArray();

		CurrentWord = words[GD.RandRange(0, words.Length - 1)];
		GD.Print(CurrentWord);
	}
}
