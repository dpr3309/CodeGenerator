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
        CompilationPipeline.compilationFinished += OnCompilationFinished;
    }

    private static void OnCompilationFinished(object obj)
    {
        var assemblySourcesPath = GetHeaderAssemblySourcesPath();

        var filesFromAssembly = Helper.GetFiles(assemblySourcesPath, "*.cs");

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

    /// <summary>
    /// возвращает путь к исходникам файлов, собержащихся в сборке HeaderAssembly.dll
    /// </summary>
    /// <returns>директорию, в которй лежит файт помеченный атрибутом AssemblyCodeGenSourceAttribute - это должен быть файл AssemblyInfo, в корне директории хедеров</returns>
    private static string GetHeaderAssemblySourcesPath()
    {
        var currentDomain = AppDomain.CurrentDomain;

        var ass = currentDomain.GetAssemblies();

        var headerAss = ass.FirstOrDefault(i =>
            i.GetCustomAttributes(typeof(AssemblyCodeGenSourceAttribute), true).Length > 0);
        var p =  headerAss.GetCustomAttributes(typeof(AssemblyCodeGenSourceAttribute), false)
            .Cast<AssemblyCodeGenSourceAttribute>().FirstOrDefault().sourceFilePath;

        var directory = Path.GetDirectoryName(p);

        return directory;
    }
    
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

