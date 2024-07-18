using Newtonsoft.Json;

namespace Redbean.MVP
{
	public interface ISerializeModel : IModel
	{
		[JsonIgnore]
		public IRxModel Rx { get; }
	}

	public interface IRxModel : IModel
	{
		public void Publish(ISerializeModel value) { }
	}
}