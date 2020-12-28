using System;
using System.Data;
using SharpDbSchema;

namespace SharpDbSchema.PostgreSQL
{
	/// <summary>
	/// Implements IColumnMetadata for PostgreSQL
	/// </summary>
	public class ColumnInfo : IColumnMetadata
	{
		public string Name { get; set; }

		public string Type { get; set; }

		public string Attributes { get; set; }

		public bool IsKey { get; set; }

		public DbType DbType { get; set; }

        public int Length { get; set; }
	}
}
