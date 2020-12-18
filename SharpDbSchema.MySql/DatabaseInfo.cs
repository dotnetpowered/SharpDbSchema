using System;
using System.Collections;
using System.Text;
using System.Xml;
using SharpDbSchema;
using MySql.Data.MySqlClient;

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

		internal System.Data.IDataReader Execute(StringBuilder sb)
		{
			return Execute(sb.ToString());
		}

		internal System.Data.IDataReader Execute(string s)
		{
			MySqlConnection conn=new MySqlConnection(_Connection);
			conn.Open();
			System.Data.IDbCommand cmd=conn.CreateCommand();
			cmd.CommandText=s;
			return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
		}

		private ITableMetadata[] GetTables()
		{
			System.Data.IDataReader reader=Execute("SHOW TABLES IN "+_Name);
			ArrayList tables=new ArrayList();
			while (reader.Read())
			{
				TableInfo tbl=new TableInfo(this, 
					(string) reader[0], 
					null,
					DateTime.MinValue);

				tables.Add(tbl);
			}
			return (ITableMetadata[]) tables.ToArray(typeof(ITableMetadata));
		}

		internal IColumnMetadata[] LoadColumns(string TableName)
		{
			System.Data.IDataReader reader=Execute("SHOW COLUMNS IN "+_Name+"."+TableName);
			ArrayList columns=new ArrayList();
			while (reader.Read())
			{
				bool IsKey= ( (string) reader["Key"] )=="PRI";
				string TypeName=(string) reader["Type"];

				// TODO: read other column attributes
				ColumnInfo col = new ColumnInfo()
				{
					Name = (string)reader["Field"],
					IsKey = IsKey
				};

				columns.Add(col);
			}
			return (IColumnMetadata[]) columns.ToArray(typeof(IColumnMetadata));
		}


		public IViewMetadata[] Views
		{
			get
			{
				return null;
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
				return null;
			}
		}

	}
}