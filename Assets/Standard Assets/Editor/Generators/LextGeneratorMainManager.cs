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


//         private static Type typeOfCmdAttribute, typeOfMsgAttribute, typeOfModelAttribute, typeOfHandlerAttribute;
//         
//         
//         public static void Generate(Assembly monitoredAssembly, string pathToTargetAssembly)
//         {
//             Debug.Log(AppDomain.CurrentDomain.FriendlyName);
//             
//             Debug.LogWarning("-----------Generate--------: " + monitoredAssembly.FullName);
//
//
//             var t = monitoredAssembly.GetTypes();
//             
//             typeOfCmdAttribute = t.First(i => i.Name.ToString() == typeof(CmdAttribute).ToString());
//             foreach (var cmd in GetTypesWithAttribute(monitoredAssembly, /*typeof(CmdAttribute)*/ typeOfCmdAttribute))
//             {
//                 if(cmd == null)
//                     continue;
//                 
//                 //var n = GetAttr<CmdAttribute>(cmd).name;
//                 string nameParameter = ((dynamic) GetAttributesOfType(typeOfCmdAttribute, cmd)).name;
//                 
//                 var (name, cont) = convertCmd(cmd,nameParameter ,pathToTargetAssembly);
//
//                 Debug.LogWarning(Path.GetDirectoryName(name));
//                 Directory.CreateDirectory(Path.GetDirectoryName(name));
//                 File.WriteAllText(name, cont);
//             }
//             
//             typeOfMsgAttribute = t.FirstOrDefault(i => i.GetType().ToString() == typeof(MsgAttribute).ToString());
//             
//             foreach (var msg in GetTypesWithAttribute(monitoredAssembly, /*typeof(MsgAttribute)*/ typeOfMsgAttribute))
//             {
//                 Debug.Log("--------------------msg----------------------------");
//                 if(msg == null)
//                     continue;
//                 
//                 string nameParameter = ((dynamic) GetAttributesOfType(typeOfMsgAttribute, msg)).name;
//                 var (name, cont) = convertMsg(msg, /*GetAttr<MsgAttribute>(msg).name*/ nameParameter, pathToTargetAssembly);
//                 
//                 Debug.LogWarning(Path.GetDirectoryName(name));
//                 Directory.CreateDirectory(Path.GetDirectoryName(name));
//                 Debug.Log(name + "\n" + cont);
//                 File.WriteAllText(name, cont);
//             }
//             
//             typeOfModelAttribute = t.FirstOrDefault(i => i.GetType().ToString() == typeof(ModelAttribute).ToString());
//             foreach (var model in GetTypesWithAttribute(monitoredAssembly, /*typeof(ModelAttribute)*/ typeOfModelAttribute))
//             {
//                 Debug.Log("--------------------model----------------------------");
//                 if(model == null)
//                     continue;
//                 
//                 string nameParameter = ((dynamic) GetAttributesOfType(typeOfModelAttribute, model)).name;
//                 var (name, cont) = convertModel(model, /*GetAttr<ModelAttribute>(model).name*/ nameParameter, pathToTargetAssembly);
//                 
//                 Debug.LogWarning(Path.GetDirectoryName(name));
//                 Directory.CreateDirectory(Path.GetDirectoryName(name));
//                 File.WriteAllText(name, cont);
//             }
//             
//             typeOfHandlerAttribute = t.FirstOrDefault(i => i.GetType().ToString() == typeof(HandlerAttribute).ToString());
//             foreach (var updater in GetTypesWithAttribute(monitoredAssembly, /*typeof(HandlerAttribute)*/ typeOfHandlerAttribute))
//             {
//                 Debug.Log("--------------------updater----------------------------");
//                 if(updater == null)
//                     continue;
//                 string prefixParameter = ((dynamic) GetAttributesOfType(typeOfHandlerAttribute, updater)).prefix;
//                 var (name, cont) = convertUpdater(updater, /*GetAttr<HandlerAttribute>(updater).prefix*/prefixParameter,pathToTargetAssembly);
//                 
//                 Debug.LogWarning(Path.GetDirectoryName(name));
//                 Directory.CreateDirectory(Path.GetDirectoryName(name));
//                 File.WriteAllText(name, cont);
//             }
//         }
//
//         private static object GetAttributesOfType(Type typeOfAttribute, Type typeObTargetClass)
//         {
//             if (typeOfAttribute == null)
//                 return null;
//             
//             return typeObTargetClass.GetCustomAttributes(typeOfAttribute, true).FirstOrDefault();
//         }
//
//         static IEnumerable<Type> GetTypesWithAttribute(Assembly assembly, Type attr)
//         {
//             if (attr == null)
//                 return new Type[] { };
//             
//             var types = new List<Type>();
//             
//             foreach (Type type in assembly.GetTypes())
//             {
//                 if (type.GetCustomAttributes(attr, true).Length > 0)
//                 {
//                     types.Add(type);
//                 }
//                 // else
//                 // {
//                 //     var allAttrs = type.CustomAttributes;
//                 //     Debug.LogWarning($"------------------{type}---------------------");
//                 //     Debug.Log(string.Join("; ", allAttrs.Select(i=>i.AttributeType.ToString())));
//                 //     foreach (var attrItem in allAttrs)
//                 //     {
//                 //         Debug.LogWarning($"{attrItem.AttributeType.ToString()} == {attr.ToString()} => {attrItem.AttributeType == attr}");
//                 //     }
//                 // }
//
//                 
//                 // if (type.CustomAttributes.Any(i => i.AttributeType.ToString() == attr.ToString()))
//                 // {
//                 //     Debug.LogWarning($"-----------{attr}------------------");
//                 //     yield return type;
//                 // }
//             }
//
//             return types;
//         }
// 		
//         static (string filename, string contents) convertMsg(Type t, string baseClassName,  string targetFilePath) =>
//             (postfixFilenameWithDir(/*GetAttr<MsgAttribute>(t).file*/((dynamic)GetAttributesOfType(typeOfMsgAttribute,t)).file, $"mvu.{baseClassName}", targetFilePath),
//                 tcall(moduleFile, prependContentToNamespace(
//                     interfaceToClass(t, baseClassName),
//                     (new []{ "System" }, MsgGenerator.msgs($"{baseClassName}s", baseClassName, lowercaseNames(methods(t))))
//                 )));
// 		
//         static (string filename, string contents) convertModel(Type t, string baseClassName,  string targetFilePath) =>
//             (postfixFilenameWithDir(/*GetAttr<ModelAttribute>(t).file*/((dynamic)GetAttributesOfType(typeOfModelAttribute, t)).file, $"mvu.{baseClassName}", targetFilePath),
//                 tcall(moduleFile, prependContentToNamespace(
//                     interfaceToClass(t, baseClassName),
//                     (new []{ "System" }, "")
//                 )));
//
//         static dynamic selectNames(IEnumerable<dynamic> list)
//         {
//             var l = new List<dynamic>();
//             foreach (var item in list)
//             {
//                 l.Add(getName(item));
//             }
//
//             return l;
//         }
//
//         static dynamic getName(dynamic x) => x.name;
// 		
//         static (string filename, string contents) convertUpdater(Type t, string namePrefix, string targetFilePath)
//         {
//             var attr = GetAttributesOfType(typeOfHandlerAttribute, t);//GetAttr<HandlerAttribute>(t);
//             var filename = postfixFilenameWithDir(((dynamic)attr).file, "tesm.updater", targetFilePath);
//             var imports = new []{
//                 "System",
//                 "System.Linq",
//                 "System.Collections.Generic",
//                 nstesm((Type)((dynamic)attr).model),
//                 nstesm((Type)((dynamic)attr).msg),
//                 nstesm((Type)((dynamic)attr).cmd)
//             }.Distinct();
//             var modelName = ((dynamic) GetAttributesOfType(typeOfModelAttribute, ((dynamic)attr).model)).name;//GetAttr<ModelAttribute>(attr.model).name;
//             var msgName = ((dynamic) GetAttributesOfType(typeOfMsgAttribute, ((dynamic)attr).msg)).name;//GetAttr<MsgAttribute>(attr.msg).name;
//             var cmdName = ((dynamic) GetAttributesOfType(typeOfCmdAttribute, ((dynamic)attr).cmd)).name;//GetAttr<CmdAttribute>(attr.cmd).name;
//
//             var handler = selectNames(UpdaterGenerator.handler($"{namePrefix}Handler", modelName, msgName, cmdName, methods(((dynamic)attr).msg)));
//             var updater = UpdaterGenerator.updater($"{namePrefix}Updater", $"{namePrefix}Handler",
//                 modelName, msgName, cmdName,
//                 selectNames(methods(((dynamic)attr).model))
//             );
//             return (filename, moduleFile(nstesm(t), imports, handler + "\n" + updater));
//         }
// 		
//         static string nstesm(Type t) =>
//             (ns(t) + ".mvu").TrimStart('.');
// 		
//         static IEnumerable<string> imports(Type t) =>
//             collectParametersTree(t).Select(ns).Distinct();
// 		
//         static string ns(Type t) =>
//             t.Namespace;
//
// 		
//         static IEnumerable<Type> collectParametersTree(Type t)
//         {
//             if (!t.IsGenericType)
//                 return new[] { t };
//
//             return t.GetGenericArguments().Select(collectParametersTree).Flatten().Concat(new[] { t });
//         }
//
//         static IEnumerable<(string name, T)> lowercaseNames<T>(IEnumerable<(string name, T)> methods) =>
//             methods.Select(x => (CmdGenerator.lowercaseFirst(x.name), x.Item2));
// 		
//         static (string filename, string contents) convertCmd(Type t, string baseClassName, string targetFilePath) =>
//             (postfixFilenameWithDir(/*GetAttr<CmdAttribute>(t).file*/ ((dynamic)GetAttributesOfType(typeOfCmdAttribute, t)).file, $"mvu.{baseClassName}", targetFilePath),
//                 tcall(moduleFile, interfaceToClass(t, baseClassName)));
// 		
//         static (string name, IEnumerable<(string type, string name)> args) methodData(MethodInfo m) =>
//             (m.Name, m.GetParameters().Select(y => (genericName(y.ParameterType), y.Name)));
// 		
//         static (string ns, IEnumerable<string> imports, string contents) interfaceToClass(Type t, string baseClassName)
//         {
//             var methods = t.GetMethods();
//             foreach (var method in methods)
//             {
//                 Debug.Log(method.Name + "( " + String.Join(", ",method.GetParameters().Select(i=> $"{i.ParameterType} {i.Name}")) + ")");
//             }
//             var imp = methods
//                     .Select(x => x.GetParameters().Select(p => p.ParameterType).Select(imports))
//                     .Flatten()
//                     .Flatten()
//                     .Distinct()
//                 ;
//             return (nstesm(t), imp, CmdGenerator.fullCmdList(baseClassName, methods.Select(methodData)));
//         }
// 		
//         static IEnumerable<(string name, IEnumerable<(string type, string name)> args)> methods(Type t) =>
//             t.GetMethods().Select(methodData);
//
//         static (string ns, IEnumerable<string> imports, string contents) appendContentToNamespace(
//             (string ns, IEnumerable<string> imports, string contents) source,
//             (IEnumerable<string> imports, string contents) extra
//         ) => (source.ns, source.imports.Concat(extra.imports), source.contents + "\n" + extra.contents);
// 		
//         static (string ns, IEnumerable<string> imports, string contents) prependContentToNamespace(
//             (string ns, IEnumerable<string> imports, string contents) source,
//             (IEnumerable<string> imports, string contents) extra
//         ) => appendContentToNamespace((source.ns, extra.imports, extra.contents), (source.imports, source.contents));
// 		
//         static string moduleFile(string ns, IEnumerable<string> imports, string contents) => $@"
// {string.Join("\n", imports.Select(x => $"using {x};"))}
// 		
// namespace {ns}
// {{
// {string.Join("\n", contents.Split('\n').Select(x => '\t' + x))}
// }}
// ";
//
//         static string GetDirectoryName(string fileName) => Path.GetDirectoryName(fileName);
//
//         public static string ReplaceDir(string parentPath, string childPath) => childPath.Replace(parentPath, "");
//         
//         
//         static string genericName(Type t) =>
//             !t.IsGenericType
//                 ? t.Name
//                 : $"{t.Name.Split('`').First()}<{string.Join(",", t.GetGenericArguments().Select(genericName))}>";
// 		
// 		
//         // public static T GetAttr<T>(Type t) where T: class =>
//         //     t.GetCustomAttributes(typeof(T), false).First() as T;
//
//         static string postfixFilename(string filename, string postfix) =>
//             filename.Replace(".cs", $".{postfix}.cs");
//
//         static string postfixFilenameWithDir(string filename, string postfix, string targetFilePath)
//         {
//             string result = string.Empty;
//
//             var dir = GetDirectoryName(filename);
//             //Debug.Log($"dir = {dir}");
//             
//             var parentDir = $"{GetDirectoryName(dir)}{Path.DirectorySeparatorChar}";
//
//             var onlyDir = ReplaceDir(parentDir, dir);
//             
//             // Debug.Log(onlyDir);
//             var _fileName = Path.GetFileName(filename);
//             _fileName = postfixFilename(_fileName, postfix);
//             result = string.Concat(targetFilePath, onlyDir, Path.DirectorySeparatorChar, _fileName);
//             // Debug.Log($"result = {result}");
//             return result;
//        }

        #region MyRegion
public void TryGenerate(Assembly monitoredAssembly, string pathToTargetAssembly)
         {
             Generate(monitoredAssembly, pathToTargetAssembly);
         }


 public static void Generate(Assembly monitoredAssembly, string pathToTargetAssembly)
         {
             Debug.LogError("GENERATION!!!!!!!");
             
             foreach (var cmd in GetTypesWithAttribute(monitoredAssembly, typeof(CmdAttribute)))
             {
                 string nameFromAttr = GetAttrStupid(cmd, typeof(CmdAttribute)).name;
                 var (name, cont) = convertCmd(cmd, nameFromAttr, pathToTargetAssembly);

                 Directory.CreateDirectory(Path.GetDirectoryName(name));
                 File.WriteAllText(name, cont);
             }
             
             Debug.Log("++++++++++++++++++1111++++++++++++++++++++");
             foreach (var msg in GetTypesWithAttribute(monitoredAssembly, typeof(MsgAttribute)))
             {
                 string nameFromAttr = GetAttrStupid(msg, typeof(MsgAttribute)).name;
                 
                 var (name, cont) = convertMsg(msg, nameFromAttr,pathToTargetAssembly);
                 
                 Directory.CreateDirectory(Path.GetDirectoryName(name));
                 Debug.Log(name + "\n" + cont);
                 File.WriteAllText(name, cont);
             }
             
             Debug.Log("++++++++++++++++++2222++++++++++++++++++++");
             foreach (var model in GetTypesWithAttribute(monitoredAssembly, typeof(ModelAttribute)))
             {
                 string nameFromAttr = GetAttrStupid(model, typeof(ModelAttribute)).name;
                 var (name, cont) = convertModel(model, nameFromAttr,pathToTargetAssembly);
                 
                 Directory.CreateDirectory(Path.GetDirectoryName(name));
                 File.WriteAllText(name, cont);
             }
             
             Debug.Log("++++++++++++++++++3333++++++++++++++++++++");
             foreach (var updater in GetTypesWithAttribute(monitoredAssembly, typeof(HandlerAttribute)))
             {
                 string prefixFromAttr = GetAttrStupid(updater, typeof(HandlerAttribute)).prefix;
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
                 // Debug.LogWarning($"[GetTypesWithAttribute] for [{type.Name}] ; attr = {attr.Name} count = {type.GetCustomAttributes(attr, true).Length}");
                 //
                 // if (type.GetCustomAttributes(attr, true).Length > 0)
                 // {
                 //     yield return type;
                 // }

                 var attrsOfType = type.GetCustomAttributes();
                 if (attrsOfType.Any(i => i.GetType().Name == attr.Name))
                 {
                     Debug.LogWarning($"Ret of type!!!: {type.Name}");
                     yield return type;
                 }
             }
         }
 		
         static (string filename, string contents) convertMsg(Type t, string baseClassName,  string targetFilePath) =>
             (postfixFilenameWithDir(GetAttrStupid(t, typeof(MsgAttribute)).file, $"mvu.{baseClassName}", targetFilePath),
                 tcall(moduleFile, prependContentToNamespace(
                     interfaceToClass(t, baseClassName),
                     (new []{ "System" }, MsgGenerator.msgs($"{baseClassName}s", baseClassName, lowercaseNames(methods(t))))
                 )));
 		
         static (string filename, string contents) convertModel(Type t, string baseClassName,  string targetFilePath) =>
             (postfixFilenameWithDir(GetAttrStupid(t, typeof(ModelAttribute)).file, $"mvu.{baseClassName}", targetFilePath),
                 tcall(moduleFile, prependContentToNamespace(
                     interfaceToClass(t, baseClassName),
                     (new []{ "System" }, "")
                 )));
 		
 		
         static (string filename, string contents) convertUpdater(Type t, string namePrefix, string targetFilePath)
         {
             var attr = GetAttrStupid(t , typeof(HandlerAttribute));

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
             var modelName = GetAttrStupid(modelFromAttr, typeof(ModelAttribute)).name;
             var msgName = GetAttrStupid(msgFromAttr, typeof(MsgAttribute)).name;
             var cmdName = GetAttrStupid(cmdFromAttr, typeof(CmdAttribute)).name;
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
             (postfixFilenameWithDir(GetAttrStupid(t, typeof(CmdAttribute)).file, $"mvu.{baseClassName}", targetFilePath),
                 tcall(moduleFile, interfaceToClass(t, baseClassName)));
 		
         static (string name, IEnumerable<(string type, string name)> args) methodData(MethodInfo m) =>
             (m.Name, m.GetParameters().Select(y => (genericName(y.ParameterType), y.Name)));
 		
         static (string ns, IEnumerable<string> imports, string contents) interfaceToClass(Type t, string baseClassName)
         {
             var methods = t.GetMethods();
             foreach (var method in methods)
             {
                 Debug.Log(method.Name + "( " + String.Join(", ",method.GetParameters().Select(i=> $"{i.ParameterType} {i.Name}")) + ")");
             }
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
 		
 		
         // public static T GetAttr<T>(Type t) where T: class =>
         //     t.GetCustomAttributes(typeof(T), false).First() as T;

         public static dynamic GetAttrStupid(Type t, Type attrType)
         {
             var attrsOfType = t.GetCustomAttributes();
             if (attrsOfType.Any(i => i.GetType().Name == attrType.Name))
             {
                 Debug.Log(attrsOfType.First(i => i.GetType().Name == attrType.Name).GetType());
                 return attrsOfType.First(i => i.GetType().Name == attrType.Name);
             }
             else
             {
                 return null;
             }
         }
         // public static T GetAttr<T>(Type t, Type attrType) where T: Attribute
         // {
         //     var attrsOfType = t.GetCustomAttributes();
         //     if (attrsOfType.Any(i => i.GetType().Name == attrType.Name))
         //     {
         //         Debug.Log(attrsOfType.First(i => i.GetType().Name == attrType.Name).GetType());
         //         return (T)attrsOfType.First(i => i.GetType().Name == attrType.Name);
         //     }
         //     else
         //     {
         //         return null;
         //     }
         // }
             

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
         #endregion

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