using Newtonsoft.Json;

namespace Redbean.MVP
{
	public interface ISerializeModel : IModel
	{
		[JsonIgnore]
		IRxModel Rx { get; }
	}

	public interface IRxModel : IModel
	{
		void Publish(ISerializeModel value) { }
	}
}