namespace junit.framework
{
	public class Assert
	{
		public static void assertTrue(string message, bool condition)
		{
			NUnit.Framework.Assert.IsTrue(condition, message);
		}

		public static void assertTrue(bool condition)
		{
			NUnit.Framework.Assert.IsTrue(condition);
		}

		public static void assertFalse(string message, bool condition)
		{
			NUnit.Framework.Assert.IsFalse(condition, message);
		}

		public static void assertFalse(bool condition)
		{
			NUnit.Framework.Assert.IsFalse(condition);
		}

		public static void fail(string message)
		{
			NUnit.Framework.Assert.Fail(message);
		}

		public static void fail()
		{
			NUnit.Framework.Assert.Fail();
		}

		public static void assertEquals(string message, object expected, object actual)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual, message);
		}

		public static void assertEquals(object expected, object actual)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual);
		}

		public static void assertEquals(string message, string expected, string actual)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual, message);
		}

		public static void assertEquals(string expected, string actual)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual);
		}

		public static void assertEquals(string message, double expected, double actual, double delta)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual, delta, message);
		}

		public static void assertEquals(double expected, double actual, double delta)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual, delta);
		}

		public static void assertEquals(string message, float expected, float actual, float delta)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual, delta, message);
		}

		public static void assertEquals(float expected, float actual, float delta)
		{
		}

		public static void assertEquals(string message, long expected, long actual)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual, message);
		}

		public static void assertEquals(long expected, long actual)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual);
		}

		public static void assertEquals(string message, bool expected, bool actual)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual, message);
		}

		public static void assertEquals(bool expected, bool actual)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual);
		}

		public static void assertEquals(string message, byte expected, byte actual)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual, message);
		}

		public static void assertEquals(byte expected, byte actual)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual);
		}

		public static void assertEquals(string message, char expected, char actual)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual, message);
		}

		public static void assertEquals(char expected, char actual)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual);
		}

		public static void assertEquals(string message, short expected, short actual)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual, message);
		}

		public static void assertEquals(short expected, short actual)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual);
		}

		public static void assertEquals(string message, int expected, int actual)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual, message);
		}

		public static void assertEquals(int expected, int actual)
		{
			NUnit.Framework.Assert.AreEqual(expected, actual);
		}

		public static void assertNotNull(object _object)
		{
			NUnit.Framework.Assert.IsNotNull(_object);
		}

		public static void assertNotNull(string message, object _object)
		{
			NUnit.Framework.Assert.IsNotNull(_object, message);
		}

		public static void assertNull(object _object)
		{
			NUnit.Framework.Assert.IsNull(_object);
		}

		public static void assertNull(string message, object _object)
		{
			NUnit.Framework.Assert.IsNull(_object, message);
		}

		public static void assertSame(string message, object expected, object actual)
		{
			NUnit.Framework.Assert.AreSame(expected, actual, message);
		}

		public static void assertSame(object expected, object actual)
		{
			NUnit.Framework.Assert.AreSame(expected, actual);
		}

		public static void assertNotSame(string message, object expected, object actual)
		{
			if (expected == actual)
				failSame(message);
		}

		public static void assertNotSame(object expected, object actual)
		{
			assertNotSame(null, expected, actual);
		}

		public static void failSame(string message)
		{
			string formatted = "";
			if (message != null)
				formatted = message + " ";
			NUnit.Framework.Assert.Fail(formatted + "expected not same");
		}

		public static void failNotSame(string message, object expected, object actual)
		{
			string formatted = "";
			if (message != null)
				formatted = message + " ";
			NUnit.Framework.Assert.Fail(formatted + "expected same:<" + expected + "> was not:<" + actual + ">");
		}

		public static void failNotEquals(string message, object expected, object actual)
		{
			NUnit.Framework.Assert.Fail(format(message, expected, actual));
		}

		private static string format(string message, object expected, object actual)
		{
			string formatted = "";
			if (message != null)
				formatted = message + " ";
			return formatted + "expected:<" + expected + "> but was:<" + actual + ">";
		}
	}
}