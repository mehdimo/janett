namespace Helpers
{
	using System;
	using System.Threading;

	public class ThreadHelper : Runnable
	{
		private Thread thread;

		public ThreadHelper()
		{
			thread = new Thread(new ThreadStart(run));
		}

		public ThreadHelper(string Name)
		{
			thread = new Thread(new ThreadStart(run));
			this.Name = Name;
		}

		public ThreadHelper(ThreadStart Start)
		{
			thread = new Thread(Start);
		}

		public ThreadHelper(ThreadStart Start, string Name)
		{
			thread = new Thread(Start);
			this.Name = Name;
		}

		public virtual void run()
		{
		}

		public void Start()
		{
			thread.Start();
		}

		public void Interrupt()
		{
			thread.Interrupt();
		}

		public Thread Instance
		{
			get { return thread; }
			set { thread = value; }
		}

		public string Name
		{
			get { return thread.Name; }
			set
			{
				if (thread.Name == null)
				{
					thread.Name = value;
				}
			}
		}

		public ThreadPriority Priority
		{
			get { return thread.Priority; }
			set { thread.Priority = value; }
		}

		public bool IsAlive
		{
			get { return thread.IsAlive; }
		}

		public bool IsBackground
		{
			get { return thread.IsBackground; }
			set { thread.IsBackground = value; }
		}

		public void Join()
		{
			thread.Join();
		}

		public static void Sleep(int miliseconds)
		{
			Thread.Sleep(miliseconds);
		}

		public static void Sleep(TimeSpan timeSpan)
		{
			Thread.Sleep(timeSpan);
		}

		public void Join(long MiliSeconds)
		{
			lock (this)
			{
				thread.Join(new TimeSpan(MiliSeconds * 10000));
			}
		}

		public void Join(long MiliSeconds, int NanoSeconds)
		{
			lock (this)
			{
				thread.Join(new TimeSpan(MiliSeconds * 10000 + NanoSeconds * 100));
			}
		}

		public void Resume()
		{
			thread.Resume();
		}

		public void Abort()
		{
			thread.Abort();
		}

		public void Abort(object stateInfo)
		{
			lock (this)
			{
				thread.Abort(stateInfo);
			}
		}

		public void Suspend()
		{
			thread.Suspend();
		}

		public static ThreadHelper CurrentThread()
		{
			ThreadHelper CurrentThread = new ThreadHelper();
			CurrentThread.Instance = Thread.CurrentThread;
			return CurrentThread;
		}
	}
}