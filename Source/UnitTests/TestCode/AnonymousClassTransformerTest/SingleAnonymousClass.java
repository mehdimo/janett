package Test;
public class A
{
	public void Draw(int count, Shape shape){}
	public void CreateShape(ShapeName name)
	{
		Draw(0, new Shape()
					{
						public string GetName()
						{
							string shapeName = name + "Shape";
							return shapeName;
						}
					});
	}
}