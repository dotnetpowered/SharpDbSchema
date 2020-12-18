using System;
using SharpDbSchema;

namespace SharpDbSchema.MySql
{
	/// <summary>
	/// DB Schema provider for MySql
	/// </summary>
	public class MySqlSchemaProvider : ISchemaProvider
	{
		public MySqlSchemaProvider()
		{
		}

		public IDatabaseMetadata GetDatabase(string DbName, string Connection)
		{
			return new DatabaseInfo(DbName, Connection);
		}

	}
}