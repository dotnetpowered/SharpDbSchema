using System;

namespace SharpDbSchema
{
	/// <summary>
	/// Describes a table within a database
	/// </summary>
	public interface ITableMetadata
	{
		string Name
		{
			get;
		}
		string Description
		{
			get;
		}
		DateTime Created
		{
			get;
		}

		IColumnMetadata[] Columns
		{
			get;
		}
	}
}
