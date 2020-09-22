using System.Collections.Generic;
using SRC.Model;

namespace HomeIsland
{
    [Model]
    [Handler(typeof(IHomeIslandModel), typeof(IHomeIslandMsg), typeof(IHomeIslamdCmd))]
    public interface IHomeIslandModel
    {
         void NotStarted();
        void Normal();
        void Ended();
        void NotNormal();
        
        void Tratata();
        
        void Bbbdfg();
        
        void Bbb();
        void Ddd();
    }
    
    
    [Msg]
    public interface IHomeIslandMsg
    {
        void StartInit();
        
        
        void LoadedLocalSave(MapState localSave);
        
        void CompletedMinigame(MinigameType game);
        void CompletedCasual(CasualgameType game, bool flag);
        
        void StartedNextIteration(IterationConfig ctx);
    }
    
    
    
    [Cmd]
    public interface IHomeIslamdCmd
    {
        void showWinScreen();
        void collectCoin(MinigameType island);
        void saveLocalState();
        void showCasualIslands(IEnumerable<CasualgameType> casuals);
        void launchMinigame(MinigameType game);
        void launchCasual(CasualgameType game);
    }
}

