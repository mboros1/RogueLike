using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using Nito.AsyncEx;
using static LanguageExt.Prelude;

namespace RogueLike
{
	public class GameEngine : IGameEngine
	{
		private readonly AsyncCollection<IPlayerAction> _actionQueue;

		public GameEngine()
		{
			_actionQueue = new AsyncCollection<IPlayerAction>(new ConcurrentQueue<IPlayerAction>(), 10);
			CommandLine = string.Empty;
			Player = new Player();
		}

		public string CommandLine { get; set; }
		public string StatusLine { get; private set; }
		public int StatusTtl { get; private set; }
		public Player Player { get; set; }
		public CommandProcessor CommandProcessor { get; set; }
		public IObjectLoader ObjectLoader { get; set; }
		public ISaveGameStore SaveGameStore { get; set; }
      public Map Map { get; set; }
		public bool IsActive { get; set; }

		public Task<IPlayerAction> TakeNextActionAsync()
		{
			return _actionQueue.TakeAsync();
		}

		public void Save()
		{
			var player = Player.Save();
			var map = Map.Save();

			SaveGameStore.Save(player);
			SaveGameStore.Save(map);
		}

		public void Load()
		{
			SaveGameStore.LoadPlayer().IfSome(Player.Load);
			SaveGameStore.LoadMap().IfSome(Map.Load);
		}

		public async Task<IPlayerAction> EnqueueActionAsync(IPlayerAction action)
		{
			await _actionQueue.AddAsync(action);
			return action;
		}

		public void EndGame()
		{
			IsActive = false;
			_actionQueue.CompleteAdding();
		}

		public void SetStatus(string format, params object[] args)
		{
			StatusLine = string.Format(format, args);
			StatusTtl = 60;
		}

		public Option<string> GetStatusLine()
		{
			if (StatusTtl > 0)
			{
				StatusTtl--;
				return Some(StatusLine);
			}

			return None;
		}
	}
}