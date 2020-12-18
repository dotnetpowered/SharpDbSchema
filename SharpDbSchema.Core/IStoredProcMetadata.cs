using System;

namespace SharpDbSchema
{
	/// <summary>
	/// Describes stored procedure within a database
	/// </summary>
	public interface IStoredProcMetadata
	{
		string Name
		{
			get;
		}
		
		string Description
		{
			get;
		}

		string Definition
		{
			get;
		}

		DateTime Created
		{
			get;
		}
        IColumnMetadata[] Parameters
        {
            get;
        }
		IColumnMetadata[] Columns
		{
			get;
		}
	}
}
