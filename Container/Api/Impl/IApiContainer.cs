using System.Threading.Tasks;

namespace Redbean
{
	public interface IApiContainer : IExtension
	{
		Task<object> Request(params object[] args);
	}
}