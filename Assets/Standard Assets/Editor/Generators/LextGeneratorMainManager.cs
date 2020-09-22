using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Lext.Generators
{
    public class LextGeneratorMainManager : MarshalByRefObject
    {
        public static R tcall<T1, T2, R>(Func<T1, T2, R> f, (T1, T2) tuple) => f(tuple.Item1, tuple.Item2);
        public static R tcall<T1, T3, T2, R>(Func<T1, T3, T2, R> f, (T1, T3, T2) tuple) => f(tuple.Item1, tuple.Item2, tuple.Item3);
        public static R tcall<T1, T3, T4, T2, R>(Func<T1, T3, T4, T2, R> f, (T1, T3, T4, T2) tuple) => f(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
        public static R tcall<T1, T3, T4, T5, T2, R>(Func<T1, T3, T4, T5, T2, R> f, (T1, T3, T4, T5, T2) tuple) => f(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5);

        public void TryGenerate(Assembly monitoredAssembly, string pathToTargetAssembly)
         {
             Generate(monitoredAssembly, pathToTargetAssembly);
         }


        public static void Generate(Assembly monitoredAssembly, string pathToTargetAssembly)
         {
             foreach (var cmd in GetTypesWithAttribute(monitoredAssembly, typeof(CmdAttribute)))
             {
                 string nameFromAttr = GetDynamicAttr(cmd, typeof(CmdAttribute)).name;
                 var (name, cont) = convertCmd(cmd, nameFromAttr, pathToTargetAssembly);

                 Directory.CreateDirectory(Path.GetDirectoryName(name));
                 File.WriteAllText(name, cont);
             }
             
             Debug.Log("++++++++++++++++++1111++++++++++++++++++++");
             foreach (var msg in GetTypesWithAttribute(monitoredAssembly, typeof(MsgAttribute)))
             {
                 string nameFromAttr = GetDynamicAttr(msg, typeof(MsgAttribute)).name;
                 
                 var (name, cont) = convertMsg(msg, nameFromAttr,pathToTargetAssembly);
                 
                 Directory.CreateDirectory(Path.GetDirectoryName(name));
                 //Debug.Log(name + "\n" + cont);
                 File.WriteAllText(name, cont);
             }
             
             Debug.Log("++++++++++++++++++2222++++++++++++++++++++");
             foreach (var model in GetTypesWithAttribute(monitoredAssembly, typeof(ModelAttribute)))
             {
                 string nameFromAttr = GetDynamicAttr(model, typeof(ModelAttribute)).name;
                 var (name, cont) = convertModel(model, nameFromAttr,pathToTargetAssembly);
                 
                 Directory.CreateDirectory(Path.GetDirectoryName(name));
                 File.WriteAllText(name, cont);
             }
             
             Debug.Log("++++++++++++++++++3333++++++++++++++++++++");
             foreach (var updater in GetTypesWithAttribute(monitoredAssembly, typeof(HandlerAttribute)))
             {
                 string prefixFromAttr = GetDynamicAttr(updater, typeof(HandlerAttribute)).prefix;
                 var (name, cont) = convertUpdater(updater, prefixFromAttr,pathToTargetAssembly);
                 
                 Directory.CreateDirectory(Path.GetDirectoryName(name));
                 File.WriteAllText(name, cont);
             }
             
             Debug.Log("++++++++++++++++++4444++++++++++++++++++++");
         }
 		
         static IEnumerable<Type> GetTypesWithAttribute(Assembly assembly, Type attr)
         {
             foreach (Type type in assembly.GetTypes())
             {
                 var attrsOfType = type.GetCustomAttributes();
                 if (attrsOfType.Any(i => i.GetType().Name == attr.Name))
                 {
                     //Debug.LogWarning($"Ret of type!!!: {type.Name}");
                     yield return type;
                 }
             }
         }
 		
         static (string filename, string contents) convertMsg(Type t, string baseClassName,  string targetFilePath) =>
             (postfixFilenameWithDir(GetDynamicAttr(t, typeof(MsgAttribute)).file, $"mvu.{baseClassName}", targetFilePath),
                 tcall(moduleFile, prependContentToNamespace(
                     interfaceToClass(t, baseClassName),
                     (new []{ "System" }, MsgGenerator.msgs($"{baseClassName}s", baseClassName, lowercaseNames(methods(t))))
                 )));
 		
         static (string filename, string contents) convertModel(Type t, string baseClassName,  string targetFilePath) =>
             (postfixFilenameWithDir(GetDynamicAttr(t, typeof(ModelAttribute)).file, $"mvu.{baseClassName}", targetFilePath),
                 tcall(moduleFile, prependContentToNamespace(
                     interfaceToClass(t, baseClassName),
                     (new []{ "System" }, "")
                 )));
 		
 		
         static (string filename, string contents) convertUpdater(Type t, string namePrefix, string targetFilePath)
         {
             var attr = GetDynamicAttr(t , typeof(HandlerAttribute));

             Type modelFromAttr = attr.model;
             Type msgFromAttr = attr.msg;
             Type cmdFromAttr = attr.cmd;
             
             var filename = postfixFilenameWithDir(attr.file, "tesm.updater", targetFilePath);
             var imports = new []{
                 "System",
                 "System.Linq",
                 "System.Collections.Generic",
                 nstesm(modelFromAttr),
                 nstesm(msgFromAttr),
                 nstesm(cmdFromAttr)
             }.Distinct();
             var modelName = GetDynamicAttr(modelFromAttr, typeof(ModelAttribute)).name;
             var msgName = GetDynamicAttr(msgFromAttr, typeof(MsgAttribute)).name;
             var cmdName = GetDynamicAttr(cmdFromAttr, typeof(CmdAttribute)).name;
             var handler = UpdaterGenerator.handler($"{namePrefix}Handler", modelName, msgName, cmdName, methods(msgFromAttr).Select(x => x.name));
             var updater = UpdaterGenerator.updater($"{namePrefix}Updater", $"{namePrefix}Handler",
                 modelName, msgName, cmdName,
                 methods(modelFromAttr).Select(x => x.name)
             );
             return (filename, moduleFile(nstesm(t), imports, handler + "\n" + updater));
         }
 		
         static string nstesm(Type t) =>
             (ns(t) + ".mvu").TrimStart('.');
 		
         static IEnumerable<string> imports(Type t) =>
             collectParametersTree(t).Select(ns).Distinct();
 		
         static string ns(Type t) =>
             t.Namespace;

 		
         static IEnumerable<Type> collectParametersTree(Type t)
         {
             if (!t.IsGenericType)
                 return new[] { t };

             return t.GetGenericArguments().Select(collectParametersTree).Flatten().Concat(new[] { t });
         }

         static IEnumerable<(string name, T)> lowercaseNames<T>(IEnumerable<(string name, T)> methods) =>
             methods.Select(x => (CmdGenerator.lowercaseFirst(x.name), x.Item2));
 		
         static (string filename, string contents) convertCmd(Type t, string baseClassName, string targetFilePath) =>
             (postfixFilenameWithDir(GetDynamicAttr(t, typeof(CmdAttribute)).file, $"mvu.{baseClassName}", targetFilePath),
                 tcall(moduleFile, interfaceToClass(t, baseClassName)));
 		
         static (string name, IEnumerable<(string type, string name)> args) methodData(MethodInfo m) =>
             (m.Name, m.GetParameters().Select(y => (genericName(y.ParameterType), y.Name)));
 		
         static (string ns, IEnumerable<string> imports, string contents) interfaceToClass(Type t, string baseClassName)
         {
             var methods = t.GetMethods();
             // foreach (var method in methods)
             // {
             //     Debug.Log(method.Name + "( " + String.Join(", ",method.GetParameters().Select(i=> $"{i.ParameterType} {i.Name}")) + ")");
             // }
             var imp = methods
                     .Select(x => x.GetParameters().Select(p => p.ParameterType).Select(imports))
                     .Flatten()
                     .Flatten()
                     .Distinct()
                 ;
             return (nstesm(t), imp, CmdGenerator.fullCmdList(baseClassName, methods.Select(methodData)));
         }
 		
         static IEnumerable<(string name, IEnumerable<(string type, string name)> args)> methods(Type t) =>
             t.GetMethods().Select(methodData);

         static (string ns, IEnumerable<string> imports, string contents) appendContentToNamespace(
             (string ns, IEnumerable<string> imports, string contents) source,
             (IEnumerable<string> imports, string contents) extra
         ) => (source.ns, source.imports.Concat(extra.imports), source.contents + "\n" + extra.contents);
 		
         static (string ns, IEnumerable<string> imports, string contents) prependContentToNamespace(
             (string ns, IEnumerable<string> imports, string contents) source,
             (IEnumerable<string> imports, string contents) extra
         ) => appendContentToNamespace((source.ns, extra.imports, extra.contents), (source.imports, source.contents));
 		
         static string moduleFile(string ns, IEnumerable<string> imports, string contents) => $@"
 {string.Join("\n", imports.Select(x => $"using {x};"))}
 		
 namespace {ns}
 {{
 {string.Join("\n", contents.Split('\n').Select(x => '\t' + x))}
 }}
 ";

         static string GetDirectoryName(string fileName) => Path.GetDirectoryName(fileName);

         public static string ReplaceDir(string parentPath, string childPath) => childPath.Replace(parentPath, "");
         
         
         static string genericName(Type t) =>
             !t.IsGenericType
                 ? t.Name
                 : $"{t.Name.Split('`').First()}<{string.Join(",", t.GetGenericArguments().Select(genericName))}>";
 		
         public static dynamic GetDynamicAttr(Type t, Type attrType)
         {
             var attrsOfType = t.GetCustomAttributes();
             if (attrsOfType.Any(i => i.GetType().Name == attrType.Name))
             {
                 //Debug.Log(attrsOfType.First(i => i.GetType().Name == attrType.Name).GetType());
                 return attrsOfType.First(i => i.GetType().Name == attrType.Name);
             }
             else
             {
                 return null;
             }
         }
         
         static string postfixFilename(string filename, string postfix) =>
             filename.Replace(".cs", $".{postfix}.cs");

         static string postfixFilenameWithDir(string filename, string postfix, string targetFilePath)
         {
             string result = string.Empty;
         
             var dir = GetDirectoryName(filename);
             //Debug.Log($"dir = {dir}");
             
             var parentDir = $"{GetDirectoryName(dir)}{Path.DirectorySeparatorChar}";
         
             var onlyDir = ReplaceDir(parentDir, dir);
             
             // Debug.Log(onlyDir);
             var _fileName = Path.GetFileName(filename);
             _fileName = postfixFilename(_fileName, postfix);
             result = string.Concat(targetFilePath, onlyDir, Path.DirectorySeparatorChar, _fileName);
             // Debug.Log($"result = {result}");
             return result;
         }

         public string GenerateSpecialDirPath(Assembly tempAssembly)
         {
            var  attr = tempAssembly.GetCustomAttributes(typeof(AssemblyCodeGenSourceAttribute), false)
                 .Cast<AssemblyCodeGenSourceAttribute>().FirstOrDefault();
        
             if (attr == null)
                 return string.Empty;
             
             Debug.Log(attr.sourceFilePath);

             var specialDirPath = attr.path.Replace('/', Path.DirectorySeparatorChar);

             return specialDirPath;
         }
    }
}