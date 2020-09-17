
using System;
using SRC.Model;
		
namespace MapCatalog.mvu
{
	public class Msgs
			{
				Action<Msg> send;
				private Msgs(){} // hide default constructor
				public Msgs(Action<Msg> send)
				{
					this.send = send;
				}
				public void startInit() => send(Msg.startInit());
	public void loadedLocalSave(MapState localSave) => send(Msg.loadedLocalSave(localSave));
	public void completedMinigame(MinigameType game) => send(Msg.completedMinigame(game));
	public void completedCasual(CasualgameType game) => send(Msg.completedCasual(game));
	public void startedNextIteration(IterationConfig ctx) => send(Msg.startedNextIteration(ctx));
			}
	
	/* This code is auto-generated, do not modify manually */
	public class Msg
	{
	public class StartInit : Msg { }
	
	public static StartInit startInit() =>
		new StartInit();
				
	
	public class LoadedLocalSave : Msg
	{
		public readonly MapState localSave;
	
		private LoadedLocalSave() { } // hide default constructor
	
		public LoadedLocalSave(
			MapState localSave
		)
		{
			this.localSave = localSave;
		}
	}
				
	
	public static LoadedLocalSave loadedLocalSave(MapState localSave) =>
		new LoadedLocalSave(localSave);
				
	
	public class CompletedMinigame : Msg
	{
		public readonly MinigameType game;
	
		private CompletedMinigame() { } // hide default constructor
	
		public CompletedMinigame(
			MinigameType game
		)
		{
			this.game = game;
		}
	}
				
	
	public static CompletedMinigame completedMinigame(MinigameType game) =>
		new CompletedMinigame(game);
				
	
	public class CompletedCasual : Msg
	{
		public readonly CasualgameType game;
	
		private CompletedCasual() { } // hide default constructor
	
		public CompletedCasual(
			CasualgameType game
		)
		{
			this.game = game;
		}
	}
				
	
	public static CompletedCasual completedCasual(CasualgameType game) =>
		new CompletedCasual(game);
				
	
	public class StartedNextIteration : Msg
	{
		public readonly IterationConfig ctx;
	
		private StartedNextIteration() { } // hide default constructor
	
		public StartedNextIteration(
			IterationConfig ctx
		)
		{
			this.ctx = ctx;
		}
	}
				
	
	public static StartedNextIteration startedNextIteration(IterationConfig ctx) =>
		new StartedNextIteration(ctx);
				
	}
				
}
