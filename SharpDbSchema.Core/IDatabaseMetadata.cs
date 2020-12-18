using System;

namespace SharpDbSchema
{
	/// <summary>
	/// Describes database schema
	/// </summary>
	public interface IDatabaseMetadata
	{
		string Name
		{
			get;
		}

		ITableMetadata[] Tables
		{
			get;
		}

		IViewMetadata[] Views
		{
			get;
		}

		IStoredProcMetadata[] StoredProcs
		{
			get;
		}

	}
}
