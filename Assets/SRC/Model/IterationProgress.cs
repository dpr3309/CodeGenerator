using System.Collections.Generic;
using System.Linq;

namespace SRC.Model
{
    public class IterationProgress
    {
        public enum CasualState
        {
            Invisible,
            Active,
            Completed,
        }
        public enum MinigameState
        {
            Active,
            Completed,
        }
        public Dictionary<MinigameType, MinigameState> minigames;
        public Dictionary<CasualgameType, CasualState> casuals;
        public bool hasSeenWinScreen;

        private IterationProgress() { }
        public IterationProgress(
            Dictionary<MinigameType, MinigameState> minigames,
            Dictionary<CasualgameType, CasualState> casuals,
            bool hasSeenWinScreen
        )
        {
            this.minigames = minigames;
            this.casuals = casuals;
            this.hasSeenWinScreen = hasSeenWinScreen;
        }

        public IterationProgress withMinigames(Dictionary<MinigameType, MinigameState> minigames) =>
            new IterationProgress(
                minigames,
                casuals,
                hasSeenWinScreen
            );
        public IterationProgress withCasuals(Dictionary<CasualgameType, CasualState> casuals) =>
            new IterationProgress(
                minigames,
                casuals,
                hasSeenWinScreen
            );
        public IterationProgress withHasSeenWinScreen(bool hasSeenWinScreen) =>
            new IterationProgress(
                minigames,
                casuals,
                hasSeenWinScreen
            );
        public IterationProgress updateMinigameState(MinigameType type, MinigameState state)
        {
            minigames.Add(type, state);
            return withMinigames(minigames);
        }

        public IterationProgress updateCasualState(CasualgameType type, CasualState state)
        {
            casuals.Add(type, state);
            return withCasuals(casuals);
        }
            

        public int CompletedMinigamesCount() =>
            minigames.Count(x => x.Value == IterationProgress.MinigameState.Completed);
    }
}