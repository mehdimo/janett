namespace org.apache.commons.logging
{
	using System;

	using log4net;

	public class Log4NetLog : Log
	{
		private string name;
		private ILog logger;

		public Log4NetLog()
		{
		}

		public Log4NetLog(string name)
		{
			this.name = name;
		}

		public ILog getLogger()
		{
			if (logger == null)
				logger = log4net.LogManager.GetLogger(name);
			return (logger);
		}

		public bool isDebugEnabled()
		{
			return getLogger().IsDebugEnabled;
		}

		public bool isErrorEnabled()
		{
			return getLogger().IsErrorEnabled;
		}

		public bool isFatalEnabled()
		{
			return getLogger().IsFatalEnabled;
		}

		public bool isInfoEnabled()
		{
			return getLogger().IsInfoEnabled;
		}

		public bool isTraceEnabled()
		{
			return getLogger().IsDebugEnabled;
		}

		public bool isWarnEnabled()
		{
			return getLogger().IsWarnEnabled;
		}

		public void trace(object message)
		{
			debug(message);
		}

		public void trace(object message, Exception t)
		{
			debug(message, t);
		}

		public void debug(object message)
		{
			getLogger().Debug(message);
		}

		public void debug(object message, Exception t)
		{
			getLogger().Debug(message);
		}

		public void info(object message)
		{
			getLogger().Info(message);
		}

		public void info(object message, Exception t)
		{
			getLogger().Info(message, t);
		}

		public void warn(object message)
		{
			getLogger().Warn(message);
		}

		public void warn(object message, Exception t)
		{
			getLogger().Warn(message, t);
		}

		public void error(object message)
		{
			getLogger().Error(message);
		}

		public void error(object message, Exception t)
		{
			getLogger().Error(message, t);
		}

		public void fatal(object message)
		{
			getLogger().Fatal(message);
		}

		public void fatal(object message, Exception t)
		{
			getLogger().Fatal(message, t);
		}
	}
}