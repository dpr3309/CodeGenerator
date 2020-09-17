using System;
using System.IO;
using System.Linq;
using Lext.Generators;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

[InitializeOnLoad]
public class CompilationManager: IDisposable
{
    static CompilationManager()
    {
        Debug.Log("FIRSTPASS");
        // EditorApplication.update += Update;
        CompilationPipeline.assemblyCompilationFinished += ProcessBatchModeCompileFinish;
        
    }

    private static void ProcessBatchModeCompileFinish(string s, CompilerMessage[] compilerMessages)
    {
        Debug.Log($"COMPILATION FINISHED: {s}");
        
        if(s.Trim().ToUpper().Contains("firstpass".Trim().ToUpper()))
            OnScriptsReloaded();
    }
    
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        // do something
        Debug.Log("FIRSTPASS-RECOMPILE");
        Debug.Log("Запустить кодо генерацию!");


        var assemblyes = AppDomain.CurrentDomain.GetAssemblies();

        var modelAssembly =
            assemblyes.FirstOrDefault(i => i.FullName.Trim().ToUpper().Contains("model".Trim().ToUpper()));
        
        if(modelAssembly == null)
            throw new Exception("[CompilationManager.OnScriptsReloaded] modelAssembly == null!!");
        
        var attr = modelAssembly.GetCustomAttributes(typeof(AssemblySourcesPathAttribute), false)
            .Cast<AssemblySourcesPathAttribute>().First();
        
        var specialDirPath = attr.path.Replace('/', Path.DirectorySeparatorChar);

        foreach (var assembly in assemblyes)
        {
            LextGeneratorMainManager.Generate(assembly, $"{Application.dataPath}{Path.DirectorySeparatorChar}{specialDirPath}");
        }
    }


    public void Dispose()
    {
        CompilationPipeline.assemblyCompilationFinished -= ProcessBatchModeCompileFinish;
        Debug.Log(new string('-', 30));
    }
}

