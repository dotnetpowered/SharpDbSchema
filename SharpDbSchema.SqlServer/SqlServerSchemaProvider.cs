using System;
using SharpDbSchema;

namespace SharpDbSchema.SqlServer
{
	/// <summary>
	/// DB Schema provider for MS SQL Server
	/// </summary>
	public class SqlServerSchemaProvider : ISchemaProvider
	{
		public SqlServerSchemaProvider()
		{
		}

		public IDatabaseMetadata GetDatabase(string DbName, string Connection)
		{
			return new DatabaseInfo(DbName, Connection);
		}

	}
}
