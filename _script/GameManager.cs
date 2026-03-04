using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace Mummoth;

public partial class GameManager : Node
{
	public string CurrentWord { get; private set; } = string.Empty;

	[Export]
	private PackedScene _WordleWordComponent;
	[Export]
	private CanvasLayer _Canvas;
	[Export]
	private int _NumberOfRows = 3;
	[Export(PropertyHint.Dir)]
	private string _WordListFolderPath = string.Empty;

	private int _CurrentRow;
	private WordleWord[] _WordRows;


	public override void _Ready()
	{
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

		// Enable the current row.
		_WordRows[_CurrentRow].SetEnable(true);
	}


	private void OnWordSubmitted(string submittedWord)
	{
		List<int> correctLetters = [];
		List<int> incorrectLetters = [];
		List<int> wrongPositionedLetters = [];

		var targetMatched = new bool[CurrentWord.Length];
		// Find the correct letters.
		for (var i = 0; i < CurrentWord.Length; i++)
		{
			if (submittedWord[i] != CurrentWord[i]) continue;
			correctLetters.Add(i);
			targetMatched[i] = true;
		}

		// Find the correct letter but in wrong positions.
		for (var letter = 0; letter < CurrentWord.Length; letter++)
		{
			if (correctLetters.Contains(letter)) continue;

			var found = false;
			for (var targetLetter = 0; targetLetter < CurrentWord.Length; targetLetter++)
			{
				if (submittedWord[letter] != CurrentWord[targetLetter] ||
					targetMatched[targetLetter]) continue;

				wrongPositionedLetters.Add(letter);
				targetMatched[targetLetter] = true;
				found = true;
				break;
			}

			if (!found) incorrectLetters.Add(letter);
		}

		// Update the state of the letters.
		for (var i = 0; i < _WordRows[_CurrentRow].WordFragments.Count; i++)
		{
			WordFragment fragment = _WordRows[_CurrentRow].WordFragments[i];
			if (correctLetters.Contains(i))
				fragment.UpdateState(WordFragment.WordFragmentState.Correct);
			else if (incorrectLetters.Contains(i))
				fragment.UpdateState(WordFragment.WordFragmentState.Incorrect);
			else if (wrongPositionedLetters.Contains(i))
				fragment.UpdateState(WordFragment.WordFragmentState.WrongPosition);
		}

		// Move onto the next row if possible.
		_WordRows[_CurrentRow].SetEnable(false);
		if (_CurrentRow == _WordRows.Length - 1)
		{
			GD.Print("Game Over!");
			return;
		}

		_CurrentRow++;
		_WordRows[_CurrentRow].SetEnable(true);
	}

	private void PickWord()
	{
		// Try and obtain the word list files.
		DirAccess wordListDir = DirAccess.Open(_WordListFolderPath);
		if (wordListDir == null)
		{
			GD.PrintErr("The provided Word List Directory either doesn't exist " +
						"or can't be opened!");
			return;
		}

		List<string> wordLists = wordListDir.GetFiles().ToList();

		// Randomly pick a list.
		int randomIndex = GD.RandRange(0, wordLists.Count - 1);
		string pickedWordListPath = wordLists[randomIndex];

		// Access the lists content.
		var listPath = $"{wordListDir.GetCurrentDir()}/{pickedWordListPath}";
		FileAccess wordListFile = FileAccess.Open(listPath, FileAccess.ModeFlags.Read);

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
