using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using Lext.Generators;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Compilation;
using UnityEngine;
using Assembly = System.Reflection.Assembly;

[InitializeOnLoad]
public class CompilationManager
{
    static CompilationManager()
    {
        Debug.Log("FIRSTPASS");
        // EditorApplication.update += Update;
        CompilationPipeline.assemblyCompilationFinished += ProcessBatchModeCompileFinish;
    }

    private static void ProcessBatchModeCompileFinish(string s, CompilerMessage[] compilerMessages)
    {
        // Debug.Log($" ProcessBatchModeCompileFinish: {s}");
        //
        // foreach (var message in compilerMessages)
        // {
        //     Debug.Log(message.message);
        // }
        //
        //
        //
        //
        // // if(s.Trim().ToUpper().Contains("firstpass".Trim().ToUpper()))
        //     OnScriptsReloaded(s);
    }

    // [DidReloadScripts]
    // private static void Did() => OnScriptsReloaded();
    //
    // private static void OnScriptsReloaded(string s = "")
    // {
    //     var assemblyes = AppDomain.CurrentDomain.GetAssemblies()
    //         .Where(x => x.GetCustomAttributes(typeof(AssemblyCodeGenSourceAttribute), false).Any());
    //
    //     if (!assemblyes.Any())
    //         return;
    //
    //     // Debug.Log($"1: COMPILATION FINISHED: {s}");
    //     // // do something
    //     // Debug.Log("FIRSTPASS-RECOMPILE");
    //     // Debug.Log("Запустить кодо генерацию!");
    //
    //     // var modelAssembly =
    //     //     assemblyes.FirstOrDefault(i => i.FullName.Trim().ToUpper().Contains("model".Trim().ToUpper()));
    //     
    //     // if(modelAssembly == null)
    //     //     throw new Exception("[CompilationManager.OnScriptsReloaded] modelAssembly == null!!");
    //
    //     // foreach (var modelAssembly in assemblyes)
    //     // {
    //     //     Debug.Log(modelAssembly.FullName);
    //     //     Debug.Log(modelAssembly.Location);
    //     //     GenerateOne(modelAssembly.Location);
    //     // }
    //
    //      // dlls = dlls.Concat(assemblyes.Select(x => x.Location)).Distinct().ToList();
    //      // Regenerate();
    // }

    public static List<string> dlls = new List<string>();

    [UnityEditor.MenuItem("GENERATOR/Regenerate")]
    public static void Regenerate()
    {
        //foreach (var dll in dlls)
        string dll = $"Library/ScriptAssemblies/HeaderAssembly.dll";
            GenerateOne(dll);
    }

    private static bool inProcess;

    private static int index
    {
        get => PlayerPrefs.GetInt("LastAssemblyIndex", 0);
        set
        {
            PlayerPrefs.SetInt("LastAssemblyIndex", value);
            PlayerPrefs.Save();
        }
    }
    
    public static void GenerateOne(string path)
    {
        if(inProcess)
            return;

        Debug.LogError("-=Start generation=-");
        inProcess = true;

        if (string.IsNullOrEmpty(path))
        {
            inProcess = false;
            return;
        }


        var assCheck = Assembly.LoadFile(path);
        var attrCheck = assCheck.GetCustomAttributes(typeof(AssemblyCodeGenSourceAttribute), false)
            .Cast<AssemblyCodeGenSourceAttribute>().FirstOrDefault();  

        if (attrCheck == null)
            return;
        
        Debug.Log($"generating from {path}");
        var domain = AppDomain.CreateDomain("reflection domain1234", new Evidence());
        var file = System.IO.File.ReadAllBytes(path);
        var ass = domain.Load(file);
        
        var attr = ass.GetCustomAttributes(typeof(AssemblyCodeGenSourceAttribute), false)
            .Cast<AssemblyCodeGenSourceAttribute>().FirstOrDefault();
        
        if (attr == null)
            return;
        
        Debug.Log(path);
        
        Debug.Log(attr.sourceFilePath);
        string sourcesDir = Path.GetDirectoryName(attr.sourceFilePath);
        Debug.Log("ЗАПУСТИТЬ генерацию длл-ки из файлов в подкаталогах!!!!: " + sourcesDir);

        index += 1;
        Debug.LogError("Index = " + index);


        var tempAssembly = Helper.BuildAssemblyFromSources(sourcesDir, index);
        
        var specialDirPath = attr.path.Replace('/', Path.DirectorySeparatorChar);
        
        
        
        //LextGeneratorMainManager.Generate(tempAssembly, $"{specialDirPath}");
        inProcess = false;
        AppDomain.Unload(domain);
        Debug.LogWarning("-=End generation=-");
        
    }
    
    [UnityEditor.MenuItem("GENERATOR/RegenerateCompleteDll")]
    public static void RegenerateCompleteDll()
    {
        if(inProcess)
            return;

        Debug.LogError("-=Start generation=-");
        inProcess = true;

        // if (string.IsNullOrEmpty(path))
        // {
        //     inProcess = false;
        //     return;
        // }
        //
        //
        // var assCheck = Assembly.LoadFile(path);
        // var attrCheck = assCheck.GetCustomAttributes(typeof(AssemblyCodeGenSourceAttribute), false)
        //     .Cast<AssemblyCodeGenSourceAttribute>().FirstOrDefault();  
        //
        // if (attrCheck == null)
        //     return;
        //
        // Debug.Log($"generating from {path}");
        //
        // //var file = System.IO.File.ReadAllBytes(path);
        // var ass = Assembly.LoadFile(path);
        //
        // var attr = ass.GetCustomAttributes(typeof(AssemblyCodeGenSourceAttribute), false)
        //     .Cast<AssemblyCodeGenSourceAttribute>().FirstOrDefault();
        //
        // if (attr == null)
        //     return;
        //
        // Debug.Log(path);
        //
        // Debug.Log(attr.sourceFilePath);
        // string sourcesDir = Path.GetDirectoryName(attr.sourceFilePath);
        // Debug.Log("ЗАПУСТИТЬ генерацию длл-ки из кацлов в подкаталогах!!!!: " + sourcesDir);

        var currentDomain = AppDomain.CurrentDomain;

        var ass = currentDomain.GetAssemblies();

        var fps = ass.FirstOrDefault(i => i.GetName().Name.Contains("Editor-firstpass"));
        var headerAss = ass.FirstOrDefault(i => i.GetName().Name == "HeaderAssembly");
        var ucm = ass.FirstOrDefault(i => i.GetName().Name == "UnityEngine.CoreModule");
        var modelAss = ass.FirstOrDefault(i => i.GetName().Name == "ModelAssembly");
        var path = fps.Location;
        
        Debug.Log("=-=-=-=-= curdom name = " + currentDomain.FriendlyName);
        
        var domain = AppDomain.CreateDomain("reflection domain", new Evidence());
        var file2 = System.IO.File.ReadAllBytes($"Temp/MyAssembly/HeaderAssembly_{index}.dll");
        Debug.LogError("fuck fuck fuck!!!!!");
        var tempAssembly = domain.Load(file2);

        var fps_bytes = File.ReadAllBytes(path);
        var headerAss_bytes = File.ReadAllBytes(headerAss.Location);
        var ucm_bytes = File.ReadAllBytes(ucm.Location);
        var modelAss_bytes = File.ReadAllBytes(modelAss.Location);
        
        var ass_2 = domain.GetAssemblies();
        domain.Load(fps_bytes);
        ass_2 = domain.GetAssemblies();
        
        domain.Load(headerAss_bytes);
        ass_2 = domain.GetAssemblies();
        domain.Load(ucm_bytes);

        domain.Load(modelAss_bytes);
        
        Debug.Log(domain.FriendlyName);
        
        Assembly[] assemblies = domain.GetAssemblies();
        foreach (Assembly asm in assemblies)
            Debug.Log(asm.GetName().Name+" : " + asm.Location);
        
        //var tempAssembly = Assembly.LoadFile("Temp/MyAssembly/HeaderAssembly.dll");
        
        //var specialDirPath = attr.path.Replace('/', Path.DirectorySeparatorChar);
        
        //Debug.Log("=-=-=-=-=-=-= " + specialDirPath);
        
        Debug.LogWarning("-------------------------------------"+tempAssembly.Location);

        string specialDirPath = "Temp/MVU/";
        //LextGeneratorMainManager.Generate(tempAssembly, $"{specialDirPath}");
        var lextGener = domain.CreateInstance(fps.GetName().Name, typeof(LextGeneratorMainManager).FullName);
        LextGeneratorMainManager unwrapLextGener = lextGener.Unwrap() as LextGeneratorMainManager;
        try
        {
            unwrapLextGener.TryGenerate(tempAssembly, $"{specialDirPath}");
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            
            AppDomain.Unload(domain);
        }
        
        inProcess = false;
        AppDomain.Unload(domain);
        Debug.LogWarning("-=End generation=-");
    }
}

