namespace org.apache.commons.logging
{
	using System;
	using System.Reflection;

	public class LogFactory
	{
		public static Log getLog(object type)
		{
			string logName = "";
			if (type is Type)
				logName = ((Type) type).FullName;
			if (type.GetType().FullName == "java.lang.Class")
				logName = (string) type.GetType().InvokeMember("getName", BindingFlags.InvokeMethod, null, type, new object[] {});
			return new Log4NetLog(logName);
		}
	}
}