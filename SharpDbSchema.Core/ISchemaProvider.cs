using System;

namespace SharpDbSchema
{
	/// <summary>
	/// Interface implemented by a DB Schema Provider
	/// </summary>
	public interface ISchemaProvider
	{
		IDatabaseMetadata GetDatabase(string DbName, string Connection);
	}
}
