﻿using System;
using System.Threading.Tasks;

namespace RogueLike
{
	public class GameLoop
	{
		private IGameEngine Game { get; set; }

		public GameLoop(IGameEngine game)
		{
			Game = game;
		}

		public async Task GameLoopAsync()
		{
			Game.Load();

			while (Game.IsActive)
			{
				var context = new GameActionContext(Game);
				try
				{
					var action = await Game.TakeNextActionAsync();
					action.Act(context);

					foreach (var mobile in Game.Map.Mobiles)
						mobile.Act(context);
					
					Game.Save();
				}
				catch (InvalidOperationException)
				{
					continue;
				}
			}
		}
	}
}