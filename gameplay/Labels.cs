using System.Data;

namespace Sokoban.gameplay
{
	/// <summary>
	/// A class that holds all of the text that appears in the game
	/// </summary>
	public static class Labels
	{
		public const string TITLE =					"SOKOBAN";
		public const string BACK =					"Back";
		public const string NEXT =					"Next";
		public const string SAVE_PREFIX =			"Save";
		public const string NEW_GAME =				"New game";
		public const string CONTINUE =				"Continue";
		public const string LEADERBOARD =			"Leaderboard";

		public const string LEVEL_EASY =			"Easy";
		public const string LEVEL_MEDIUM =			"Intermediate";
		public const string LEVEL_HARD =			"Advanced";

		public const string EDITOR_LEVEL_PREFIX =	"Level";
		public const string EDITOR_NEW =			"New";
		public const string EDITOR_EDIT =			"Edit level";
		public const string EDITOR_DELETE =			"Delete level";

		public const string EDITOR_TITLE =			"LEVEL EDITOR";
		public const string EDITOR_CONTROLS =		"Controls";
		public const string EDITOR_MOVEMENT =		"Arrows/WASD - move cursor";
		public const string EDITOR_REMOVE =			"1 - Remove item";
		public const string EDITOR_PLAYER =			"2 - Place player";
		public const string EDITOR_CHEST =			"3 - Place chest";
		public const string EDITOR_OBSTACLE =		"4 - Place obstacle";
		public const string EDITOR_PLATE =			"5 - Place plate";
		public const string EDITOR_MINUS =			"minus - shrink level";
		public const string EDITOR_PLUS =			"plus - expand level";

		public const string MODULE_SELECTOR =		"Casual play";
		public const string MODULE_CHALLENGE =		"Challenge mode";
		public const string MODULE_EDITOR =			"Custom levels";

		public const string SAVE =					"Save";
		public const string SETTINGS_YES =			"Yes";
		public const string SETTINGS_NO =			"No";

		public const string PLAY =					"Play";
		public const string SETTINGS =				"Settings";
		public const string QUIT =					"Quit";

		public const string PAGE_URL =				"https://abcight.com";

		public const string PAUSE_TITLE =			"PAUSE";
		public const string PAUSE_TEST =			"Test level";
		public const string PAUSE_RESET =			"Reset level";
		public const string PAUSE_EDITOR_RESET =	"Go to edit mode";
		public const string PAUSE_MENU_RESET =		"Exit to menu";
		public const string PAUSE_FINALIZE =		"End run";

		public const string CHALLENGE_STEPS =		"Step count";
		public const string CHALLENGE_SCORE =		"Score";

		public const string GAME_END =				"FINISH";
		public const string GAME_SCORE =			"Score";
		public const string GAME_RETURN =			"Press RETURN to go back to the menu.";
	}
}
