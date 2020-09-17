using System;
using System.Collections.Generic;
using System.Linq;

namespace Lext.Generators
{
	public class CmdGenerator
	{
		public static string argField(string type, string name) =>
			$"public readonly {type} {name};";
		public static string argSignatureMember(string type, string name) =>
			$"{type} {name}";
		public static string argMemberAssignment(string type, string name) =>
			$"this.{name} = {name};";

		public static Func<(T, U), R> detuple<T, U, R>(Func<T, U, R> f) =>
			p => f(p.Item1, p.Item2);
		public static Func<(T, U, V), R> detuple<T, U, V, R>(Func<T, U, V, R> f) =>
			p => f(p.Item1, p.Item2, p.Item3);

		public static string lineBreaks(IEnumerable<string> lines) =>
			string.Join("\n", lines);
		public static string commas(IEnumerable<string> lines) =>
			string.Join(", ", lines);

		public static IEnumerable<R> tuple_map<T, U, R>(IEnumerable<(T, U)> arr, Func<T, U, R> f) =>
			arr.Select(p => f(p.Item1, p.Item2));
		

		public static string simpleClass(string parent, string className) =>
			$"public class {className} : {parent} {{ }}";
		public static string complexClass(string parent, string className, IEnumerable<(string, string)> args) =>
			$@"
public class {className} : {parent}
{{
	{ lineBreaks(
		tuple_map(args, argField)
	) }

	private {className}() {{ }} // hide default constructor

	public {className}(
		{ commas(tuple_map(args, argSignatureMember)) }
	)
	{{
		{ lineBreaks(
			tuple_map(args, argMemberAssignment)
		) }
	}}
}}
			";
		public static string autoClass(string parent, string className, IEnumerable<(string, string)> args) =>
			(args.Count() > 0)
			? complexClass(parent, className, args)
			: simpleClass(parent, className);
		public static string factoryMethod(string className, string methodName, IEnumerable<(string, string)> args) =>
			$@"
public static {className} {methodName}({commas(tuple_map(args, argSignatureMember))}) =>
	new {className}({commas(args.Select(x => x.Item2))});
			";
		
		public static string capitalizeFirst(string str) =>
			str.First().ToString().ToUpper() + str.Substring(1);
		public static string lowercaseFirst(string str) =>
			str.First().ToString().ToLower() + str.Substring(1);

		public static string fullSingleClass(string parent, string name, IEnumerable<(string, string)> args) =>
			autoClass(parent, capitalizeFirst(name), args)
			+ "\n"
			+ factoryMethod(capitalizeFirst(name), lowercaseFirst(name), args);
		public static string fullCmdList(string parentName, IEnumerable<(string name, IEnumerable<(string, string)> args)> members) =>
			$@"
/* This code is auto-generated, do not modify manually */
public class {parentName}
{{
{ lineBreaks(
	members.Select(x => fullSingleClass(parentName, x.name, x.args))
) }
}}
			";
	}

	public static class ListExtension
	{
		/// <summary>
		/// Creates a single flat list from a list of lists input. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="inListOfLists"></param>
		/// <returns>A single flatten list from a list of lists input</returns>
		public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> inListOfLists)
		{
			List<T> outList = new List<T>();
			foreach (var list in inListOfLists)
			{
				outList.AddRange(list);
			}
			return outList;
		}
	}
}