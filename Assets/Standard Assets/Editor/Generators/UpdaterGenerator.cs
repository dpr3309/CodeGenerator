using System;
using System.Linq;
using System.Collections.Generic;
using static Lext.Generators.CmdGenerator;
using static Lext.Generators.MsgGenerator;

namespace Lext.Generators
{
	public class UpdaterGenerator
	{
		public static string handlerField(
			string modelBaseClass,
			string msgBaseClass,
			string cmdBaseClass,
			string msgName
		) =>
			$"public Func<{msgBaseClass}.{msgName}, TModel, ({modelBaseClass}, IEnumerable<{cmdBaseClass}>)> {msgName};";

		public static string handlerInvoke(
			string msgBaseClass,
			string msgName
		) =>
			$@"
			case {msgBaseClass}.{msgName} m:
				return {msgName}?.Invoke(m, model);
";
		public static string handler(
			string className,
			string modelBaseClass,
			string msgBaseClass,
			string cmdBaseClass,
			IEnumerable<string> msgs
		) =>
			$@"
public struct {className}<TModel> where TModel : {modelBaseClass}
{{
	{lineBreaks(msgs.Select(x => handlerField(modelBaseClass, msgBaseClass, cmdBaseClass, x)))}

	public ({modelBaseClass}, IEnumerable<{cmdBaseClass}>)? handle({msgBaseClass} msg, TModel model)
	{{
		switch (msg)
		{{
			{lineBreaks(msgs.Select(x => handlerInvoke(msgBaseClass, x)))}
		}}
		return null;
	}}
}}
";
		public static string modelHandlerField(
			string handlerName,
			string modelBaseClass,
			string modelName
		) =>
			$"public {handlerName}<{modelBaseClass}.{modelName}> {modelName};";
		
		public static string modelHandlerInvoke(
			string modelBaseClass,
			string modelName
		) =>
			$@"
			case {modelBaseClass}.{modelName} m:
				return {modelName}.handle(msg, m);
";
		public static string updater(
			string className,
			string handlerName,
			string modelBaseClass,
			string msgBaseClass,
			string cmdBaseClass,
			IEnumerable<string> models
		) => $@"
public struct {className}
{{
	{lineBreaks(models.Select(x => modelHandlerField(handlerName, modelBaseClass, x)))}

	public ({modelBaseClass}, IEnumerable<{cmdBaseClass}>)? handle({msgBaseClass} msg, {modelBaseClass} model)
	{{
		switch (model)
		{{
			{lineBreaks(models.Select(x => modelHandlerInvoke(modelBaseClass, x)))}
		}}
		return null;
	}}
}}
";
	}
}