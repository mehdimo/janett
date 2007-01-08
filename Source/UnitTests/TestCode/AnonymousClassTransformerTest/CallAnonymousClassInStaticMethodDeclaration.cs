namespace Test
{
	public class A
	{
		public static void Method1()
		{
			int sum;
			int i = new AnonymousClassCalc1(sum);
		}
		public void Method2()
		{
			int[] result;
			int i = new AnonymousClassCalc2(this, result);
		}
		private class AnonymousClassCalc1 : Calc
		{
			public AnonymousClassCalc1(int sum)
			{
				this.sum = sum;
			}
			public int addAll(){ return sum; }
			private int sum;
		}

		private class AnonymousClassCalc2 : Calc
		{
			public AnonymousClassCalc2(A enclosingInstance, int[] result)
			{
				this.enclosingInstance = enclosingInstance;
				this.result = result;
			}
			public int[] calculate(){ B.StMethod(); return result; }
			private A enclosingInstance;
			private int[] result;
			public A Enclosing_Instance
			{
				get { return enclosingInstance;}
			}
		}
	}
	public class B 
	{
		public static void StMethod() {}
	}
}