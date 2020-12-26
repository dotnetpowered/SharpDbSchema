using System;
using System.Text;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace SharpDbSchema.MySql
{
    /// <summary>
    /// Implements IDatabaseInfo for MySql
    /// </summary>
    public class DatabaseInfo : IDatabaseMetadata
	{
		private string _Name;
		private ITableMetadata[] _Tables;
		private string _Connection;

		internal DatabaseInfo(string Name, string Connection)
		{
			_Name=Name;
			_Connection=Connection;
		}

		public string Name
		{
			get
			{
				return _Name;
			}
		}		

		internal IDataReader Execute(StringBuilder sb)
		{
			return Execute(sb.ToString());
		}

		internal IDataReader Execute(string s)
		{
			MySqlConnection conn=new(_Connection);
			conn.Open();
			IDbCommand cmd=conn.CreateCommand();
			cmd.CommandText=s;
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        private ITableMetadata[] GetTables()
		{
			using IDataReader reader=Execute("SHOW TABLES IN "+_Name);
			List<TableInfo> tables=new ();
			while (reader.Read())
			{
				TableInfo tbl=new(this, 
					(string) reader[0], 
					null,
					DateTime.MinValue);

				tables.Add(tbl);
			}
            return tables.ToArray();
        }

		internal IColumnMetadata[] LoadColumns(string TableName)
		{
			using IDataReader reader=Execute("SHOW COLUMNS IN "+_Name+"."+TableName);
			List<ColumnInfo> columns=new ();
			while (reader.Read())
			{
				bool IsKey= ( (string) reader["Key"] )=="PRI";
				string TypeName=(string) reader["Type"];

				// TODO: read other column attributes
				ColumnInfo col = new()
				{
					Name = (string)reader["Field"],
					IsKey = IsKey
				};

				columns.Add(col);
			}
			return columns.ToArray();
		}


		public IViewMetadata[] Views
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public ITableMetadata[] Tables
		{
			get
			{
				if (_Tables==null)
					_Tables=GetTables();
				return _Tables;
			}
		}

		public IStoredProcMetadata[] StoredProcs
		{
			get
			{
				throw new NotImplementedException();
			}
		}

	}
}