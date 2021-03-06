﻿using System;
using System.Threading.Tasks;

namespace RogueLike
{
	public class InputLoop
	{
		private readonly GameEngine _game;
		private readonly IConsoleInput _consoleInput;

		public InputLoop(GameEngine game, IConsoleInput consoleInput)
		{
			_game = game;
			_consoleInput = consoleInput;
		}

		public async Task InputLoopAsync()
		{
			var commandBuffer = new char[80];
			var extendedCommand = false;
			var i = 0;

			while (_game.IsActive)
			{
				var key = _consoleInput.ReadKey();

				if (!char.IsControl(key.KeyChar))
				{
					if (extendedCommand)
					{
						commandBuffer[i++] = key.KeyChar;
						_game.CommandLine = new string(commandBuffer, 0, i);

						continue;
					}
				}

				switch (key.KeyChar)
				{
					case '`':
						extendedCommand = true;
						continue;
				}

				switch (key.Key)
				{
					case ConsoleKey.Q:
						_game.CommandProcessor.Add("quit");
						break;

					case ConsoleKey.Backspace:
						i = i == 0 ? 0 : i - 1;
						continue;

					case ConsoleKey.Enter:
						_game.CommandProcessor.Add(_game.CommandLine);
						extendedCommand = false;
						_game.CommandLine = "";
						i = 0;
						break;

					case ConsoleKey.Escape:
						extendedCommand = false;
						_game.CommandLine = "";
						i = 0;
						break;

					case ConsoleKey.UpArrow:
						await _game.CommandProcessor.Move(yDelta: -1);
						break;

					case ConsoleKey.DownArrow:
						await _game.CommandProcessor.Move(yDelta: 1);
						break;

					case ConsoleKey.LeftArrow:
						await _game.CommandProcessor.Move(xDelta: -1);
						break;

					case ConsoleKey.RightArrow:
						await _game.CommandProcessor.Move(xDelta: +1);
						break;
				}
			}
		}
	}
}