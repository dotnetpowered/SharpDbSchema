using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;
using Npgsql;

namespace SharpDbSchema.PostgreSQL
{
	/// <summary>
	/// Implements IDatabaseInfo for PostgreSQL
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
			NpgsqlConnection conn = new(_Connection);
			conn.Open();
			IDbCommand cmd=conn.CreateCommand();
			cmd.CommandText=s;
			return cmd.ExecuteReader(CommandBehavior.CloseConnection);
		}

		private ITableMetadata[] GetTables()
		{
			// Based on http://golden13.blogspot.com/2012/08/how-to-get-some-information-about_7.html
			using IDataReader reader = Execute(@"SELECT c.relname AS table_name, n.nspname as schema_name,    
							u.usename as owner_name,
							(SELECT obj_description(c.oid, 'pg_class')) AS comment
							FROM pg_catalog.pg_class c
							LEFT JOIN pg_catalog.pg_user u ON u.usesysid = c.relowner
							LEFT JOIN pg_catalog.pg_namespace n ON n.oid = c.relnamespace
							WHERE n.nspname = 'public' AND c.relkind IN('r','') 
							AND n.nspname NOT IN('pg_catalog', 'pg_toast', 'information_schema')
							ORDER BY datname ASC; ");
			List<TableInfo> tables = new();
			while (reader.Read())
			{
				TableInfo tbl = new(this,
					(string)reader[0], // table_name 
					(string)reader[3]  // description
					);

				tables.Add(tbl);
			}
			return tables.ToArray();
		}

		internal IColumnMetadata[] LoadColumns(string TableName)
		{
			// Based on http://golden13.blogspot.com/2012/08/how-to-get-some-information-about_7.html
			IDataReader reader = Execute(@"SELECT pg_tables.tablename, pg_attribute.attname AS field, 
					format_type(pg_attribute.atttypid, NULL) AS data_type, 
					pg_attribute.atttypmod AS len,
					(SELECT col_description(pg_attribute.attrelid, 
							pg_attribute.attnum)) AS comment, 
					CASE pg_attribute.attnotnull 
						WHEN false THEN 1  ELSE 0  
					END AS not_null
				FROM pg_tables, pg_class 
				JOIN pg_attribute ON pg_class.oid = pg_attribute.attrelid 
					AND pg_attribute.attnum > 0 
				WHERE pg_class.relname = pg_tables.tablename  
					AND pg_attribute.atttypid <> 0::oid  
					AND tablename='table1' 
				ORDER BY field ASC ");
			List<ColumnInfo> columns=new ();
			while (reader.Read())
			{
				// TODO: read other column attributes
				ColumnInfo col = new ColumnInfo()
				{
					Name = (string)reader["field"],
					Type = (string)reader["data_type"]
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