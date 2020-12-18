using System;
using System.Data;

namespace SharpDbSchema
{
	/// <summary>
	/// Describes a column within a database, view, or stored procedure
	/// </summary>
	public interface IColumnMetadata
	{
		string Name
		{
			get;
		}
        string Type
        {
            get;
        }
		DbType DbType
		{
			get;
		}
		string Attributes
		{
			get;
		}
		bool IsKey
		{
			get;
		}
        int Length
        {
            get;
        }
	}
}
