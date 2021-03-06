using System;
using System.Text;
using System.Collections;
using SharpDbSchema;

namespace SharpDbSchema.PostgreSQL
{
	/// <summary>
	/// Implements ITableInfo for PostgreSQL
	/// </summary>
	public class TableInfo : ITableMetadata
	{
		private string _Name;
		private string _Description;
		private DateTime? _Created;
		private DatabaseInfo _db;
		private IColumnMetadata[] _Columns;

		internal TableInfo(DatabaseInfo db, string Name, string Description)
		{
			_db=db;
			_Name=Name;
			_Description=Description;
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

		public DateTime? Created
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
					_Columns=_db.LoadColumns(Name);
				return _Columns;
			}
		}
	}
}
