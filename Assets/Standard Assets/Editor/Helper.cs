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
    public static void BuildAssemblySync(string path)
    {
        var files = GetFiles(path);
        BuildAssembly(true, files:files);
    }

    public static FileInfo[] GetFiles(string path,  string searchPattern = "*.*")
    {
        var files = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
        return files.Select(i => new FileInfo(i)).ToArray();
    }

    public static Assembly BuildAssemblyFromSources(string sourcesPath, string outPutAssemblyPath, string assemblyProjPath)
    {
        var filesInfo = GetFiles(sourcesPath, "*.cs");
        
        return BuildAssembly(true, outPutAssemblyPath,assemblyProjPath, filesInfo);
    }

    private static string _defaultOutputAssemblyPath = $"Temp{Path.DirectorySeparatorChar}MyAssembly{Path.DirectorySeparatorChar}MyAssembly.dll";
    private static string _defaultAssemblyProjectPath = $"Assets{Path.DirectorySeparatorChar}SRC{Path.DirectorySeparatorChar}MyAssembly.dll";
    public static string DefaultTempPath = $"Temp{Path.DirectorySeparatorChar}MyAssembly";

    static Assembly BuildAssembly(bool wait, string outputAssemblyPath = "" , string assemblyProjPath = "", params FileInfo[] files)
    {
        //Debug.Log("BA: " + files.Length);
        
        List<string> scripts = new List<string>();
        var outputAssembly = (!string.IsNullOrEmpty(outputAssemblyPath))
            ? outputAssemblyPath
            : _defaultOutputAssemblyPath;
        var assemblyProjectPath =
            !string.IsNullOrEmpty(assemblyProjPath) ? assemblyProjPath : _defaultAssemblyProjectPath;

        Directory.CreateDirectory(DefaultTempPath);

        // Create scripts
        foreach (var fileInfo in files)
        {
            var scriptName = Path.GetFileNameWithoutExtension(fileInfo.Name);
            string path = $"{DefaultTempPath}{Path.DirectorySeparatorChar}{scriptName}.cs";
            var content = File.ReadAllText(fileInfo.FullName);
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
