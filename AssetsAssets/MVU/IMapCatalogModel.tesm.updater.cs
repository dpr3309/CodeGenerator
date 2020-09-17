
using System;
using System.Linq;
using System.Collections.Generic;
using MapCatalog.mvu;
		
namespace MapCatalog.mvu
{
	
	public struct Handler<TModel> where TModel : Model
	{
		public Func<Msg.StartInit, TModel, (Model, IEnumerable<Cmd>)> StartInit;
	public Func<Msg.LoadedLocalSave, TModel, (Model, IEnumerable<Cmd>)> LoadedLocalSave;
	public Func<Msg.CompletedMinigame, TModel, (Model, IEnumerable<Cmd>)> CompletedMinigame;
	public Func<Msg.CompletedCasual, TModel, (Model, IEnumerable<Cmd>)> CompletedCasual;
	public Func<Msg.StartedNextIteration, TModel, (Model, IEnumerable<Cmd>)> StartedNextIteration;
	
		public (Model, IEnumerable<Cmd>)? handle(Msg msg, TModel model)
		{
			switch (msg)
			{
				
				case Msg.StartInit m:
					return StartInit?.Invoke(m, model);
	
	
				case Msg.LoadedLocalSave m:
					return LoadedLocalSave?.Invoke(m, model);
	
	
				case Msg.CompletedMinigame m:
					return CompletedMinigame?.Invoke(m, model);
	
	
				case Msg.CompletedCasual m:
					return CompletedCasual?.Invoke(m, model);
	
	
				case Msg.StartedNextIteration m:
					return StartedNextIteration?.Invoke(m, model);
	
			}
			return null;
		}
	}
	
	
	public struct Updater
	{
		public Handler<Model.NotStarted> NotStarted;
	
		public (Model, IEnumerable<Cmd>)? handle(Msg msg, Model model)
		{
			switch (model)
			{
				
				case Model.NotStarted m:
					return NotStarted.handle(msg, m);
	
			}
			return null;
		}
	}
	
}
