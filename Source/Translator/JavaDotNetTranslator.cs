namespace Janett.Translator
{
	using System;

	using ICSharpCode.NRefactory;

	using Janett.Commons;
	using Janett.Framework;

	[Named("janett")]
	public class JavaDotNetTranslator : Translator
	{
		protected override void ValidateParameters()
		{
			if (Mode != "DotNet" && Mode != "IKVM")
				throw new ApplicationException("Invalid value for 'Mode' parameter. Possible values: IKVM, DotNet");
		}

		protected override void BeforeParse()
		{
			codeBase.Types.Visitors.Add(new AddJavaLangObjectBaseType());
		}

		protected override void BeforeTransformation()
		{
			CallVisitor(typeof(AddJavaLangObjectBaseType), null);
		}

		protected override void BeforeRefactoring()
		{
			if (Mode == "DotNet")
				CallVisitor(typeof(RemoveJavaLangObjectBaseType), null);
		}

		public override SupportedLanguage GetLanguage()
		{
			return SupportedLanguage.Java;
		}
	}
}