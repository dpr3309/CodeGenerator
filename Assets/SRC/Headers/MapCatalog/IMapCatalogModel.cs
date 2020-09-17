using System.Collections.Generic;
using SRC.Model;
using UnityEngine;

namespace MapCatalog
{
    [Model]
    [Handler(typeof(IMapCatalogModel), typeof(IMapMessageMsg), typeof(IMapCmd))]
    public interface IMapCatalogModel
    {
        void NotStarted();
    }


    [Msg]
    public interface IMapMessageMsg
    {
        void StartInit();

        void LoadedLocalSave(MapState localSave);

        void CompletedMinigame(MinigameType game, Component comp);
        void CompletedCasual(CasualgameType game, Vector2 tratata);

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