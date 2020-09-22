using System;

[AttributeUsage(AttributeTargets.Interface)]
public class CmdAttribute : Attribute
{
	public string file;
	public string name;
	public CmdAttribute(
		string name = "Cmd",
		[System.Runtime.CompilerServices.CallerFilePath]
		string sourceFilePath = ""
	)
	{
		// Console.WriteLine(sourceFilePath);
		this.file = sourceFilePath;
		this.name = name;
	}
}
[AttributeUsage(AttributeTargets.Interface)]
public class MsgAttribute : Attribute
{
	public string file;
	public string name;
	public MsgAttribute(
		string name = "Msg",
		[System.Runtime.CompilerServices.CallerFilePath]
		string sourceFilePath = ""
	)
	{
		// Console.WriteLine(sourceFilePath);
		this.file = sourceFilePath;
		this.name = name;
	}
}
[AttributeUsage(AttributeTargets.Interface)]
public class ModelAttribute : Attribute
{
	public string file;
	public string name;
	public ModelAttribute(
		string name = "Model",
		[System.Runtime.CompilerServices.CallerFilePath]
		string sourceFilePath = ""
	)
	{
		// Console.WriteLine(sourceFilePath);
		this.file = sourceFilePath;
		this.name = name;
	}
}
[AttributeUsage(AttributeTargets.Interface)]
public class HandlerAttribute : Attribute
{
	public readonly Type model;
	public readonly Type msg;
	public readonly Type cmd;
	public string file;
	public string prefix;
	public HandlerAttribute(
		Type Model,
		Type Msg,
		Type Cmd,
		string prefix = "",
		[System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
	{
		model = Model;
		msg = Msg;
		cmd = Cmd;
		// Console.WriteLine(sourceFilePath);
		this.file = sourceFilePath;
		this.prefix = prefix;
	}
}
[AttributeUsage(AttributeTargets.Class)]
public class SimpleHandlerAttribute : Attribute
{
	public string file;
	public SimpleHandlerAttribute(
		[System.Runtime.CompilerServices.CallerFilePath]
		string sourceFilePath = ""
	)
	{
		// Console.WriteLine(sourceFilePath);
		this.file = sourceFilePath;
	}
}
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public class ContextAttribute : Attribute
{
}

