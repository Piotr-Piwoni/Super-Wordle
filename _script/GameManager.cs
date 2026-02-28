using System;
using System.Linq;
using Godot;

namespace Mummoth;

public partial class GameManager : Node
{
	public string CurrentWord { get; private set; } = "";

	[Export]
	private PackedScene _WordleWordScene;
	[Export]
	private CanvasLayer _Canvas;
	[Export(PropertyHint.ArrayType, "Path to the word lists.")]
	private Godot.Collections.Array<string> _WordLists = [];

	private WordleWord _CurrentRow;


	public override void _Ready()
	{
		if (_WordLists.Count == 0)
		{
			GD.PrintErr("At least one word list needs to be assigned!");
			return;
		}

		GD.Randomize();
		PickWord();

		var wordRow = _WordleWordScene.Instantiate<WordleWord>();
		_Canvas.AddChild(wordRow);

		wordRow.Generate(CurrentWord.Length);

		_CurrentRow = wordRow;
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
