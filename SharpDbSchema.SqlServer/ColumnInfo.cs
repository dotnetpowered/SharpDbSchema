using System;
using SharpDbSchema;

namespace SharpDbSchema.SqlServer
{
	/// <summary>
	/// Implements IColumnInfo for MS SQL Server
	/// </summary>
	public class ColumnInfo : IColumnMetadata
	{

        #region IColumnMetadata Members

        public string Name
        {
            get;
            internal set;
        }

        public string Type
        {
            get;
            internal set;
        }

        public System.Data.DbType DbType
        {
            get;
            internal set;
        }

        public string Attributes
        {
            get;
            internal set;
        }

        public bool IsKey
        {
            get;
            internal set;
        }

        public int Length
        {
            get;
            internal set;
        }

        #endregion
    }
}
