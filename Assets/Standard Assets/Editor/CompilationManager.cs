using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using Lext.Generators;
using UnityEditor;
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
        CompilationPipeline.compilationFinished += OnCompilationFinished;
    }

    private static void OnCompilationFinished(object obj)
    {
        var filesFromAssembly = Helper.GetFiles( /*assCheck.Location*/"Assets/SRC/Model/Headers/", "*.cs");

        IEnumerable<(string name, string contents)> filesData =
            filesFromAssembly.Select(i => (i.FullName, File.ReadAllText(i.FullName)));

        var actualHash = HashFiles(filesData);
        if (oldHash != actualHash)
        {
            Debug.LogWarning("Нужно запустить генерацию!!!!!");
            oldHash = actualHash;
            
            var currentDomain = AppDomain.CurrentDomain;

            var ass = currentDomain.GetAssemblies();
            
            var headerAss = ass.FirstOrDefault(i =>
                i.GetCustomAttributes(typeof(AssemblyCodeGenSourceAttribute), true).Length > 0);
            
            GenerateOne(headerAss.Location);
        }
    }

    private static void ProcessBatchModeCompileFinish(string s, CompilerMessage[] compilerMessages)
    {
        // Debug.Log($" ProcessBatchModeCompileFinish: {s}");
        //
        // if (s.Trim().ToUpper().Contains("HeaderAssembly".Trim().ToUpper()))
        // {
        //     Debug.Log("START GENERATION!!!!!!!!!!!!!!!!!");
        //     var assCheck = Assembly.LoadFile(s);
        //
        //     //D:\Work\DELL\first_path_test\Assets\SRC\Model\Headers
        //     var filesFromAssembly = Helper.GetFiles( /*assCheck.Location*/"Assets/SRC/Model/Headers/", "*.cs");
        //
        //     IEnumerable<(string name, string contents)> filesData =
        //         filesFromAssembly.Select(i => (i.FullName, File.ReadAllText(i.FullName)));
        //
        //     var actualHash = HashFiles(filesData);
        //     if (oldHash != actualHash)
        //     {
        //         Debug.LogWarning("Нужно запустить генерацию!!!!!");
        //         oldHash = actualHash;
        //         headerPath = s;
        //         //GenerateOne(s);
        //     }
        // }
    }


    private static string headerPath;

private static string oldHash
    {
        get => PlayerPrefs.GetString("HeadersHash", String.Empty);
        set
        {
            PlayerPrefs.SetString("HeadersHash", value);
            PlayerPrefs.Save();
        }
    }

private static string HashFiles(IEnumerable<(string name, string contents)> files) =>
        HashCalculator.HashFiles(files);

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
    
    [MenuItem("GENERATOR/Regenerate")]
    public static void Regenerate()
    {
        var headerAss = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(i =>
            i.GetCustomAttributes(typeof(AssemblyCodeGenSourceAttribute), true).Length > 0).Location;
        
            GenerateOne(headerAss);
    }

    private static bool inProcess;

    private const string _headerAssemblyPrefix = "HeaderAssembly_";
    
    static string outPutAssemblyPath(string filename) =>
        $"Temp{Path.DirectorySeparatorChar}MyAssembly{Path.DirectorySeparatorChar}{filename}.dll";
    
    static string assemblyProjPath (string filename)=> $"Temp{Path.DirectorySeparatorChar}{filename}.dll";
    public static void GenerateOne(string path)
    {
        if(inProcess)
            return;

        Debug.Log("[CompilationManager.GenerateOne] -=Start generation=-");
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
        
        var domain = AppDomain.CreateDomain("reflection domain1234", new Evidence());
        var file = System.IO.File.ReadAllBytes(path);
        var ass = domain.Load(file);
        
        var attr = ass.GetCustomAttributes(typeof(AssemblyCodeGenSourceAttribute), false)
            .Cast<AssemblyCodeGenSourceAttribute>().FirstOrDefault();
        
        if (attr == null)
            return;
        
        string sourcesDir = Path.GetDirectoryName(attr.sourceFilePath);

        // тут ОБЯЗАТЕЛЬНО должен инкрементироваться индекс с каждой сборкой!
        // иначе при генерации подтянется последняя удачно скомпилированная сборка, без актуальных иземенний
        //index += 1;
        int index = UnityEngine.Random.Range(0, Int32.MaxValue);
        
        //собираю сборку из сорцов
        //тут ОБЯЗАТЕЛЬНО передавать индекс, который инкрементируется с каждой сборкой!
        // иначе подтянется последняя удачно скомпилированная сборка, без актуальных иземенний

        // поидеи можно оставить так, т.к. это папки, в которые удет генерироваться промежуточный код.
        string _outPutAssemblyPath = outPutAssemblyPath($"{_headerAssemblyPrefix}{index}");
        string _assemblyProjPath = assemblyProjPath($"{_headerAssemblyPrefix}{index}");//$"Temp{Path.DirectorySeparatorChar}{_headerAssemblyPrefix}{index}.dll";
        

        Helper.BuildAssemblyFromSources(sourcesDir, _outPutAssemblyPath, _assemblyProjPath);
        
        AppDomain.Unload(domain);
        Debug.Log("[CompilationManager.GenerateOne] -=End generation=-");
        
        inProcess = false;
        
        // второй этап генерации
        RegenerateCompleteDll(_headerAssemblyPrefix,index);
    }
    
    //[UnityEditor.MenuItem("GENERATOR/RegenerateCompleteDll")]
    private static void RegenerateCompleteDll(string tempAssemblyPrefix, long index)
    {
        if(inProcess)
            return;

        Debug.Log("[CompilationManager.RegenerateCompleteDll] -=Start generation=-");
        inProcess = true;

        var currentDomain = AppDomain.CurrentDomain;

        var ass = currentDomain.GetAssemblies();

        var fps = ass.FirstOrDefault(i => i.GetName().Name.Contains("Editor-firstpass"));
        var ucm = ass.FirstOrDefault(i => i.GetName().Name == "UnityEngine.CoreModule");
        
        var headerAss = ass.FirstOrDefault(i =>
            i.GetCustomAttributes(typeof(AssemblyCodeGenSourceAttribute), true).Length > 0);
        var modelAss = ass.FirstOrDefault(i => i.GetCustomAttributes(typeof(ModelAssemblyAttribute), true).Length > 0);

        
        var domain = AppDomain.CreateDomain("reflection domain", new Evidence());
        
        // тут ОБЯЗАТЕЛЬНО передавать индекс!
        // иначе подтянется последняя удачно скомпилированная сборка, без актуальных иземенний
        var targetSourcesAssembly_bytes = File.ReadAllBytes($"{Helper.DefaultTempPath}{Path.DirectorySeparatorChar}{tempAssemblyPrefix}{index}.dll");
        var fps_bytes = File.ReadAllBytes(fps.Location);
        var headerAss_bytes = File.ReadAllBytes(headerAss.Location);
        var ucm_bytes = File.ReadAllBytes(ucm.Location);
        var modelAss_bytes = File.ReadAllBytes(modelAss.Location);
        
        var tempAssembly = domain.Load(targetSourcesAssembly_bytes);
        domain.Load(fps_bytes);
        domain.Load(headerAss_bytes);
        domain.Load(ucm_bytes);
        domain.Load(modelAss_bytes);


        //LextGeneratorMainManager.Generate(tempAssembly, $"{specialDirPath}");
        var lextGener = domain.CreateInstance(fps.GetName().Name, typeof(LextGeneratorMainManager).FullName);
        LextGeneratorMainManager unwrapLextGener = lextGener.Unwrap() as LextGeneratorMainManager;
        try
        {
            string specialDirPath = unwrapLextGener.GenerateSpecialDirPath(tempAssembly);

            Debug.LogWarning($"----------------{specialDirPath}--------------");
            unwrapLextGener.TryGenerate(tempAssembly, $"{specialDirPath}");
            
            AppDomain.Unload(domain);
            Helper.BuildAssemblySync(specialDirPath);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            
            AppDomain.Unload(domain);
        }
        
        inProcess = false;
        Debug.Log("[CompilationManager.RegenerateCompleteDll]-=End generation=-");
    }
}

