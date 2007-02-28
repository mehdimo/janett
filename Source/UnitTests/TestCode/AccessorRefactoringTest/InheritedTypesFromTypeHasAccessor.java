package Test;
public interface IGraph
{
	int getDegree();
}

public interface ITree extends IGraph
{
}

public class GenericTree extends ITree
{
}
public class BinaryTree extends ITree
{
	int degree;
	public int getDegree()
	{
		return degree;
	}
}