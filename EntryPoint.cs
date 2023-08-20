using OpenTK.Windowing.Desktop;
using Sokoban;

Game game = new Game(
	NativeWindowSettings.Default,
	GameWindowSettings.Default
);

game.Run();