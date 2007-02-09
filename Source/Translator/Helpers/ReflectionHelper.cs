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

		public static Type[] GetParameterTypes(MethodBase method)
		{
			ParameterInfo[] parameters = method.GetParameters();
			Type[] types = new Type[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				types[i] = parameters[i].ParameterType;
			}
			return types;
		}
	}
}