using System.Collections.Generic;
using System.Linq;
using static Lext.Generators.CmdGenerator;

namespace Lext.Generators
{
	public class MsgGenerator
	{
		public static string argsList(IEnumerable<(string type, string name)> args) =>
			commas(args.Select(x => argSignatureMember(x.type, x.name)));
		public static string argsNamesList(IEnumerable<(string type, string name)> args) =>
			commas(args.Select(x => x.name));

		public static string msgWrapper(
			string msgBaseName,
			string msgName,
			string msgFactoryName,
			IEnumerable<(string type, string name)> args
		) => $"public void {msgName}({argsList(args)}) => send({msgBaseName}.{msgFactoryName}({argsNamesList(args)}));";
		public static string msgs(
			string className,
			string msgTypeName,
			IEnumerable<(string name, IEnumerable<(string type, string name)> args)> msgs
		) => $@"public class {className}
		{{
			Action<{msgTypeName}> send;
			private {className}(){{}} // hide default constructor
			public {className}(Action<{msgTypeName}> send)
			{{
				this.send = send;
			}}
			{lineBreaks(msgs.Select(x => msgWrapper(msgTypeName, x.name, x.name, x.args)))}
		}}";
	}
}