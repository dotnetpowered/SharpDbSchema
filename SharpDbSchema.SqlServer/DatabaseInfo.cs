using System;
using System.Collections;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using System.Collections.Generic;

namespace SharpDbSchema.SqlServer
{
	/// <summary>
	/// Implements IDatabaseInfo for MS SQL Server
	/// </summary>
	public class DatabaseInfo : IDatabaseMetadata
	{
		private string _Name;
		private ITableMetadata[] _Tables;
		private IViewMetadata[] _Views;
		private IStoredProcMetadata[] _StoredProcs;
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

        internal void LoadMetaData(string procName, out IColumnMetadata[] Param, out IColumnMetadata[] Col)
        {
            using (var conn = new SqlConnection(_Connection))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT PARAMETER_NAME,DATA_TYPE,PARAMETER_MODE,CHARACTER_MAXIMUM_LENGTH,PARAMETER_MODE " +
                    "FROM INFORMATION_SCHEMA.PARAMETERS WHERE SPECIFIC_NAME='" + procName + "'";

                var cmdResult = conn.CreateCommand();
                cmdResult.CommandText = procName;
                cmdResult.CommandType = System.Data.CommandType.StoredProcedure;

                var ParamList = new List<ColumnInfo>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var paramName = (string)reader["PARAMETER_NAME"];
                        var paramDataType = (string)reader["DATA_TYPE"];
                        DbType dbType;
                        SqlDbType sqlDbType;
                        ConvertToDbType(paramDataType, out dbType, out sqlDbType);
                        var size = reader["CHARACTER_MAXIMUM_LENGTH"];
                        if (size is DBNull)
                        {
                            ParamList.Add(new ColumnInfo() { Name = paramName, DbType = dbType, Type = paramDataType });
                            cmdResult.Parameters.Add(paramName, sqlDbType).Value = DBNull.Value;
                        }
                        else
                        {
                            ParamList.Add(new ColumnInfo() { Name = paramName, DbType = dbType, Type = paramDataType, Length = (int)size });
                            cmdResult.Parameters.Add(paramName, sqlDbType, (int)size).Value = DBNull.Value;
                        }
                    }
                }

                cmd.CommandText = "SET FMTONLY ON;";
                cmd.ExecuteNonQuery();

                var resultColumns = new List<ColumnInfo>();
                var schema = cmdResult.ExecuteReader().GetSchemaTable();
                if (schema != null)
                {
                    foreach (DataRow row in schema.Rows)
                    {
                        var DataTypeName = (string)row["DataTypeName"];
                        var dbType = ConvertToDbType(DataTypeName);
                        var name = (string)row["ColumnName"];
                        var length = (int)row["ColumnSize"];
                        resultColumns.Add(new ColumnInfo() { Name = name, Type=DataTypeName, DbType = ConvertToDbType(DataTypeName), Length = length });
                    }
                }

                Col = resultColumns.ToArray();
                Param = ParamList.ToArray();

            }
        }

		internal SqlDataReader Execute(StringBuilder sb)
		{
			SqlConnection conn=new SqlConnection(_Connection);
			conn.Open();
			SqlCommand cmd=conn.CreateCommand();
			cmd.CommandText=sb.ToString();
			return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
		}

		private StringBuilder GetObjectSQL(string objecttype, bool IncludeDefinition)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("select ");
			sb.Append("sysobjects.id as Id, ");
			sb.Append("sysobjects.name as Name, ");
			sb.Append("sysusers.name as Owner, ");
			sb.Append("sysobjects.crdate as CreateDate ");
			if (IncludeDefinition)
				sb.Append(",syscomments.text as Definition ");
			sb.Append("from sysobjects ");
			sb.Append("join sysusers on sysobjects.uid = sysusers.uid ");
			if (IncludeDefinition)
				sb.Append("join syscomments on sysobjects.id = syscomments.id ");
			sb.Append("where sysobjects.type = '"+objecttype+"' ");
			sb.Append("order by sysobjects.name");
			return sb;
		}

		private ITableMetadata[] GetTables()
		{
			using SqlDataReader reader=Execute(GetObjectSQL("U",false));
			List<TableInfo> tables=new ();
			while (reader.Read())
			{
				TableInfo tbl=new TableInfo(this, 
					(int) reader["Id"], 
					(string) reader["Name"], 
					null,
					//(string) reader["Description"], 
					(DateTime) reader["CreateDate"]);

				tables.Add(tbl);
			}
			return tables.ToArray();
		}

		private IViewMetadata[] GetViews()
		{
			using SqlDataReader reader=Execute(GetObjectSQL("V",true));
			List<ViewInfo> views=new ();
			while (reader.Read())
			{
				ViewInfo view=new ViewInfo(this, 
					(int) reader["Id"], 
					(string) reader["Name"], 
					null,
					//(string) reader["Description"],
					(string) reader["Definition"],
					(DateTime) reader["CreateDate"]);

				views.Add(view);
			}
			return views.ToArray();
		}

		private IStoredProcMetadata[] GetStoredProcs()
		{
			using SqlDataReader reader=Execute(GetObjectSQL("P",true));
			List<StoredProcInfo> proclist=new ();
			while (reader.Read())
			{
				StoredProcInfo proc=new StoredProcInfo(this, 
					(int) reader["Id"], 
					(string) reader["Name"], 
					null,
					//(string) reader["Description"],
					(string) reader["Definition"],
					(DateTime) reader["CreateDate"]);

				proclist.Add(proc);
			}
			return proclist.ToArray();
		}

		internal IColumnMetadata[] LoadColumns(int ObjectId)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("select  ");
			sb.Append("	sa.Name,  ");
			sb.Append("	sa.Id,  ");
			sb.Append("	sb.Name as Type,  ");
			sb.Append("	sa.Length,  ");
			sb.Append("	sa.IsNullable,  ");
			sb.Append("	sa.Collation,  ");
			sb.Append("	sa.IsOutParam,  ");
			sb.Append("	sa.ColOrder, ");
			sb.Append("	Ident = case ");
			sb.Append("		when (sa.status & 0x80)>0 then 1 ");
			sb.Append("		else 0 ");
			sb.Append("		end, ");
			sb.Append("	PK = IsNull(si.colid, 0) ");
			sb.Append("from  ");
			sb.Append("	sysobjects so ");
			sb.Append("	join syscolumns sa on so.id = sa.id ");
			sb.Append("	join systypes sb on sa.xtype = sb.xtype  ");
			sb.Append("	left outer join (  ");
			sb.Append("		select si.id, sk.colid from sysindexes si  ");
			sb.Append("		join sysobjects so on so.id = object_id(si.name) and so.xtype = 'PK'  ");
			sb.Append("		join sysindexkeys sk on sk.id = si.id and sk.indid = si.indid) as si  ");
			sb.Append("	on si.id = sa.id and si.colId = sa.colid  ");
			sb.Append("where so.xtype in ('U','V','P','FN') and so.Id="+ObjectId.ToString());
			sb.Append("order by sa.Id, sa.ColOrder ");

			using SqlDataReader reader=Execute(sb);
			List<ColumnInfo> columns=new ();
			while (reader.Read())
			{
				bool IsKey= ( (Int16) reader["PK"] )!=0;
				string TypeName=(string) reader["Type"];
                var dbType = ConvertToDbType(TypeName);
                ColumnInfo col = new ColumnInfo()
                {
                    Name = (string)reader["Name"],
                    Type = TypeName,
                    DbType = dbType,
                    IsKey = IsKey,
                    Length = (Int16) reader["Length"]
                };

				columns.Add(col);
			}
			return columns.ToArray();
		}

        private static DbType ConvertToDbType(string TypeName)
        {
            try
            {
                SqlParameter p = new SqlParameter("t", (SqlDbType)Enum.Parse(typeof(SqlDbType), TypeName, true));
                return p.DbType;
            }
            catch
            {
                return DbType.Object;
            }
        }

        private static void ConvertToDbType(string TypeName, out DbType dbType, out SqlDbType sqlDbType)
        {
            try
            {
                SqlParameter p = new SqlParameter("t", (SqlDbType)Enum.Parse(typeof(SqlDbType), TypeName, true));
                dbType = p.DbType;
                sqlDbType = p.SqlDbType;
            }
            catch
            {
                dbType = DbType.AnsiString;
                sqlDbType = SqlDbType.VarChar;
            }
        }

		public IViewMetadata[] Views
		{
			get
			{
				if (_Views==null)
					_Views=GetViews();
				return _Views;
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
				if (_StoredProcs==null)
					_StoredProcs=GetStoredProcs();
				return _StoredProcs;
			}
		}

	}
}