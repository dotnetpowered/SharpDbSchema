using System;
using System.Text;
using System.Data.SqlClient;
using System.Collections;
using SharpDbSchema;

namespace SharpDbSchema.SqlServer
{
	/// <summary>
	/// Implements IViewInfo for MS SQL Server
	/// </summary>
	public class ViewInfo : IViewMetadata
	{
		private int _ID;
		private string _Name;
		private string _Description;
		private DateTime _Created;
		private DatabaseInfo _db;
		private IColumnMetadata[] _Columns;
		string _Definition;

		internal ViewInfo(DatabaseInfo db, int ID, string Name, string Description, string Definition, DateTime Created)
		{
			_db=db;
			_ID=ID;
			_Name=Name;
			_Description=Description;
			_Definition=Definition;
			_Created=Created;
		}

		public string Name
		{
			get
			{
				return _Name;
			}
		}

		public string Description
		{
			get
			{
				return _Description;
			}
		}

		public string Definition
		{
			get
			{
				return _Definition;
			}
		}

		public DateTime Created
		{
			get
			{
				return _Created;
			}
		}

		public IColumnMetadata[] Columns
		{
			get
			{
				if (_Columns==null)
					_Columns=_db.LoadColumns(_ID);
				return _Columns;
			}
		}
	}
}
