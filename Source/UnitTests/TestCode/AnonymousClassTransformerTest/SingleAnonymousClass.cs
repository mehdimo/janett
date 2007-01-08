namespace Test
{
	public class A
	{
		public void Draw(int count, Shape shape){}
		public void CreateShape(ShapeName name)
		{
			Draw(0, new AnonymousClassShape1(this, name));
		}
		private class AnonymousClassShape1 : Shape
		{
			public AnonymousClassShape1(A enclosingInstance, ShapeName name)
			{
				this.enclosingInstance = enclosingInstance;
				this.name = name;
			}

			public string GetName()
			{
				string shapeName = name + "Shape";
				return shapeName;
			}

			private A enclosingInstance;
			private ShapeName name;

			public A Enclosing_Instance
			{
				get {return enclosingInstance;}
			}
		}
	}
}