using System;
using SharpDbSchema;

namespace SharpDbSchema.PostgreSQL
{
	/// <summary>
	/// DB Schema provider for PostgreSQL
	/// </summary>
	public class NgSchemaProvider : ISchemaProvider
	{
		public NgSchemaProvider()
		{
		}

		public IDatabaseMetadata GetDatabase(string DbName, string Connection)
		{
			return new DatabaseInfo(DbName, Connection);
		}

	}
}