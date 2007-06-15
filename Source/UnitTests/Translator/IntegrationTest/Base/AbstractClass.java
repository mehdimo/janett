package Test.Integration;
abstract class AbstractClass implements Interface
{
	private String name;

	public void setName(String name)
	{
		this.name = name;
	}
	public String getName()
	{
		return this.name;
	}

	protected final boolean existSimilarFieldAndMethod = true;
	protected boolean existSimilarFieldAndMethod()
    {
		String methodName = getName();
		setName(methodName);
        return existSimilarFieldAndMethod;
    }

	public abstract void AbstractClassMethod() throws Exception;

	public abstract static class InnerAbstractClass
	{
		public InnerAbstractClass(int num)
		{
        }
		public abstract void InnerAbstractClassMethod(int num);
	}
	
	public void methodToExclude()
	{
	}
}
