namespace Helpers
{
	using System;
	using System.Reflection;

	public class ReflectionHelper
	{
		public static object CallInternalMethod(string method, object obj, object[] parameters)
		{
			try
			{
				return obj.GetType().InvokeMember(method, BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic, null, obj, parameters);
			}
			catch (TargetInvocationException ex)
			{
				Exception exception = ex.GetBaseException();
				exception.HelpLink = exception.StackTrace;
				throw exception;
			}
		}
	}
}