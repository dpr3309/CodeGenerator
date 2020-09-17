
using SRC.Model;
using System.Collections.Generic;
		
namespace MapCatalog.mvu
{
	
	/* This code is auto-generated, do not modify manually */
	public class Cmd
	{
	public class ShowWinScreen : Cmd { }
	
	public static ShowWinScreen showWinScreen() =>
		new ShowWinScreen();
				
	
	public class CollectCoin : Cmd
	{
		public readonly MinigameType island;
	
		private CollectCoin() { } // hide default constructor
	
		public CollectCoin(
			MinigameType island
		)
		{
			this.island = island;
		}
	}
				
	
	public static CollectCoin collectCoin(MinigameType island) =>
		new CollectCoin(island);
				
	public class SaveLocalState : Cmd { }
	
	public static SaveLocalState saveLocalState() =>
		new SaveLocalState();
				
	
	public class ShowCasualIslands : Cmd
	{
		public readonly IEnumerable<CasualgameType> casuals;
	
		private ShowCasualIslands() { } // hide default constructor
	
		public ShowCasualIslands(
			IEnumerable<CasualgameType> casuals
		)
		{
			this.casuals = casuals;
		}
	}
				
	
	public static ShowCasualIslands showCasualIslands(IEnumerable<CasualgameType> casuals) =>
		new ShowCasualIslands(casuals);
				
	
	public class LaunchMinigame : Cmd
	{
		public readonly MinigameType game;
	
		private LaunchMinigame() { } // hide default constructor
	
		public LaunchMinigame(
			MinigameType game
		)
		{
			this.game = game;
		}
	}
				
	
	public static LaunchMinigame launchMinigame(MinigameType game) =>
		new LaunchMinigame(game);
				
	
	public class LaunchCasual : Cmd
	{
		public readonly CasualgameType game;
	
		private LaunchCasual() { } // hide default constructor
	
		public LaunchCasual(
			CasualgameType game
		)
		{
			this.game = game;
		}
	}
				
	
	public static LaunchCasual launchCasual(CasualgameType game) =>
		new LaunchCasual(game);
				
	}
				
}
