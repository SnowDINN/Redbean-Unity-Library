namespace Redbean.MVP
{
	public class Model<T> : IModel where T : new()
	{
		public T Database = new();
	}
}