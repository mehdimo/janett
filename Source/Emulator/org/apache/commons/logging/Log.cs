namespace org.apache.commons.logging
{
	using System;

	public interface Log
	{
		bool isDebugEnabled();
		bool isErrorEnabled();
		bool isFatalEnabled();
		bool isInfoEnabled();
		bool isTraceEnabled();
		bool isWarnEnabled();
		void trace(object message);
		void trace(object message, Exception t);
		void debug(object message);
		void debug(object message, Exception t);
		void info(object message);
		void info(object message, Exception t);
		void warn(object message);
		void warn(object message, Exception t);
		void error(object message);
		void error(object message, Exception t);
		void fatal(object message);
		void fatal(object message, Exception t);
	}
}