using System;

namespace SharpDbSchema
{
	/// <summary>
	/// Describes a view within a database
	/// </summary>
	public interface IViewMetadata
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

		IColumnMetadata[] Columns
		{
			get;
		}
	}
}
