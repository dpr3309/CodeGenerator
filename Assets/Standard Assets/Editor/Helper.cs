using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Assembly = System.Reflection.Assembly;

public class Helper 
{
    // [MenuItem("AssemblyBuilder Example/Build Assembly Async")]
    // public static void BuildAssemblyAsync()
    // {
    //     var files = GetFiles();
    //     BuildAssembly(false, files:files);
    // }

    [MenuItem("AssemblyBuilder Example/Build Assembly Sync")]
    public static void BuildAssemblySync()
    {
        var files = GetFiles();
        BuildAssembly(true, files:files);
    }

    private static FileInfo[] GetFiles()
    {

        var files = Directory.GetFiles("Temp/MVU","*.*", SearchOption.AllDirectories);

        return files.Select(i => new FileInfo(i)).ToArray();
        //return new[] {new FileInfo($"Temp/MVU/SomeClass.cs")};
    }

    public static Assembly BuildAssemblyFromSources(string sourcesPath, int index)
    {
        var files = Directory.GetFiles(sourcesPath,"*.cs", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            Debug.Log(file);
        }
        return BuildAssembly(true, $"Temp/MyAssembly/HeaderAssembly_{index}.dll",$"Temp/HeaderAssembly_{index}.dll", files.Select(i => new FileInfo(i)).ToArray());
    }

    static Assembly BuildAssembly(bool wait, string assPath = "" , string assPP = "", params FileInfo[] files)
    {
        Debug.Log("BA: " + files.Length);
        //var scripts = new[] { "Temp/MyAssembly/MyScript1.cs", "Temp/MyAssembly/MyScript2.cs" };
        List<string> scripts = new List<string>();
        var outputAssembly = (!string.IsNullOrEmpty(assPath))?assPath:"Temp/MyAssembly/MyAssembly.dll";
        var assemblyProjectPath = !string.IsNullOrEmpty(assPP)?assPP:"Assets/SRC/MyAssembly.dll";

        Directory.CreateDirectory("Temp/MyAssembly");

        // Create scripts
        foreach (var fileInfo in files)
        {
            var scriptName = Path.GetFileNameWithoutExtension(fileInfo.Name);
            string path = $"Temp/MyAssembly/{scriptName}.cs";
            var content = File.ReadAllText(fileInfo.FullName);
            Debug.Log("write text in file:" + path);
            Debug.Log("text:" + content);
            File.WriteAllText(path, content);
            scripts.Add(path);
        }

        var assemblyBuilder = new AssemblyBuilder(outputAssembly, scripts.ToArray());

        // Exclude a reference to the copy of the assembly in the Assets folder, if any.
        assemblyBuilder.excludeReferences = new string[] { assemblyProjectPath };

        // Called on main thread
        assemblyBuilder.buildStarted += delegate(string assemblyPath)
        {
            Debug.LogFormat("Assembly build started for {0}", assemblyPath);
        };

        // Called on main thread
        assemblyBuilder.buildFinished += delegate(string assemblyPath, CompilerMessage[] compilerMessages)
        {
            var errorCount = compilerMessages.Count(m => m.type == CompilerMessageType.Error);
            var warningCount = compilerMessages.Count(m => m.type == CompilerMessageType.Warning);

            Debug.LogFormat("Assembly build finished for {0}", assemblyPath);
            Debug.LogFormat("Warnings: {0} - Errors: {0}", errorCount, warningCount);

            if(errorCount == 0)
            {
                File.Copy(outputAssembly, assemblyProjectPath, true);
                AssetDatabase.ImportAsset(assemblyProjectPath);
            }
            else
            {
                foreach (var error in  compilerMessages.Where(m => m.type == CompilerMessageType.Error))
                {
                    Debug.LogError($"{error.message}");
                }
            }
        };

        // Start build of assembly
        if(!assemblyBuilder.Build())
        {
            throw new Exception($"Failed to start build of assembly {assemblyBuilder.assemblyPath}!");
            
        }

        if(wait)
        {
            while(assemblyBuilder.status != AssemblyBuilderStatus.Finished)
                System.Threading.Thread.Sleep(10);
        }

        return Assembly.LoadFile(outputAssembly);
    }
}
