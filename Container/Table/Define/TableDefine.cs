using System.Collections.Generic;

namespace Redbean.Table
{
	public interface ITable
	{
		void Apply(IEnumerable<string> value);
	}
}