using System.Collections.Generic;
using SRC.Model;
using UnityEngine;

namespace MapCatalog
{
    [Model]
    // [Handler(typeof(IMapCatalogModel), typeof(IMapMessageMsg), typeof(IMapCmd))]
    public interface IMapCatalogModel
    {
        void NotStarted();
        void Started();
        void NotNormal();
        
        void NotNormal2342();
    }


    // [Msg]
    public interface IMapMessageMsg
    {
        void StartInit(Vector2 tratata, int abc);

        void LoadedLocalSave(MapState localSave);

        void CompletedMinigame(MinigameType game);
        void CompletedCasual(CasualgameType game);

        void StartedNextIteration(IterationConfig ctx, int tratata, double orolo);
    }


    [Cmd]
    public interface IMapCmd
    {
        void showWinScreen();
        void collectCoin(MinigameType island);
        void saveLocalState();
        void showCasualIslands(IEnumerable<CasualgameType> casuals);
        void launchMinigame(MinigameType game);
        void launchCasual(CasualgameType game);
    }
}