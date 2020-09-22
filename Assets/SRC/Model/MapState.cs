using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SRC.Model
{
    public class MapState
    {
        public readonly int iterationNum;
        public readonly IterationConfig cfg;
        public readonly IterationProgress progress;

        private MapState() { }
        public MapState(
            int iterationNum,
            IterationConfig cfg,
            IterationProgress progress
        )
        {
            this.iterationNum = iterationNum;
            this.cfg = cfg;
            this.progress = progress;
        }

        public MapState withConfig(IterationConfig cfg) => new MapState(iterationNum, cfg, progress);
        public MapState withIterationNum(int iterationNum) => new MapState(iterationNum, cfg, progress);
        public MapState withProgress(IterationProgress progress) => new MapState(iterationNum, cfg, progress);
    }
    
    public class MinigameType { }
    public class CasualgameType { }
    
    public class IterationConfig
    {
        public IEnumerable<(MinigameType t, int coins)> minigames;
        public IEnumerable<(CasualgameType t, int turn)> casuals;
        public IEnumerable<CasualgameType> getCasualsForTurn(int turnIdx) =>
            casuals.Where(x => x.turn <= turnIdx).Select(x => x.t);
    }
}

[AttributeUsage(AttributeTargets.Assembly)]
public class AssemblyCodeGenSourceAttribute : Attribute
{
    public readonly string path;
    public readonly string sourceFilePath;

    public AssemblyCodeGenSourceAttribute(
        string path,
        [System.Runtime.CompilerServices.CallerFilePath]
        string sourceFilePath = ""
    )
    {
        this.path = path;
        this.sourceFilePath = sourceFilePath;
    }
}

[AttributeUsage(AttributeTargets.Assembly)]
public class ModelAssemblyAttribute : Attribute
{
    
}

