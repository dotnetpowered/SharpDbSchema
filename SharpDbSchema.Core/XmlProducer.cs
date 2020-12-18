using System;
using System.Xml;

namespace SharpDbSchema
{
	/// <summary>
	/// Generates XML for a database schema
	/// </summary>
	public class XmlProducer
	{

		private static XmlNode StoredProcToNode(XmlDocument doc, IStoredProcMetadata proc)
		{
			XmlElement node=doc.CreateElement("storedproc");
			node.SetAttribute("name",proc.Name);
			node.SetAttribute("created",proc.Created.ToString());
			node.SetAttribute("description",proc.Description);
			
			XmlElement defNode=doc.CreateElement("definition");
			defNode.AppendChild(doc.CreateCDataSection(proc.Definition));
			node.AppendChild(defNode);

			return node;
		}

		private static XmlNode ViewToNode(XmlDocument doc, IViewMetadata view)
		{
			XmlElement node=doc.CreateElement("view");
			node.SetAttribute("name",view.Name);
			node.SetAttribute("created",view.Created.ToString());
			node.SetAttribute("description",view.Description);

			XmlElement defNode=doc.CreateElement("definition");
			defNode.AppendChild(doc.CreateCDataSection(view.Definition));
			node.AppendChild(defNode);
			
			return node;
		}

		private static XmlNode TableToNode(XmlDocument doc, ITableMetadata tbl)
		{
			XmlElement node=doc.CreateElement("table");
			node.SetAttribute("name",tbl.Name);
			node.SetAttribute("created",tbl.Created.ToString());
			node.SetAttribute("description",tbl.Description);
			return node;
		}

		private static XmlDocument DatabaseToDocument(IDatabaseMetadata Database)
		{
			XmlDocument doc=new XmlDocument();
			doc.AppendChild(doc.CreateElement("database"));
			doc.DocumentElement.SetAttribute("name",Database.Name);
			return doc;
		}

        private static void AppendColumns(XmlNode node, IColumnMetadata[] cols)
        {
            AppendColumns(node, "columns", cols);
        }

		private static void AppendColumns(XmlNode node, string childName, IColumnMetadata[] cols)
		{
            XmlNode ColumnsNode = node.AppendChild(node.OwnerDocument.CreateElement(childName));
			foreach (IColumnMetadata col in cols)
			{
				XmlElement ColNode=ColumnsNode.OwnerDocument.CreateElement("column");
				ColumnsNode.AppendChild(ColNode);
				ColNode.SetAttribute("name",col.Name);
				ColNode.SetAttribute("attributes",col.Attributes);
				ColNode.SetAttribute("type",col.Type.ToString());
				ColNode.SetAttribute("key",col.IsKey.ToString());
			}
		}

		public static XmlDocument DatabaseToXml(IDatabaseMetadata Database)
		{
			XmlDocument doc=DatabaseToDocument(Database);
			XmlNode TablesNode=doc.DocumentElement.AppendChild(doc.CreateElement("tables"));
			foreach (ITableMetadata tbl in Database.Tables)
			{
				TablesNode.AppendChild(TableToNode(doc,tbl));
			}
			IViewMetadata[] views=Database.Views;
			if (views!=null)
			{
				XmlNode ViewsNode=doc.DocumentElement.AppendChild(doc.CreateElement("views"));
				foreach (IViewMetadata view in views)
				{
					ViewsNode.AppendChild(ViewToNode(doc,view));
				}
			}
			IStoredProcMetadata[] procs=Database.StoredProcs;
			if (procs!=null)
			{
				XmlNode StoredProcsNode=doc.DocumentElement.AppendChild(doc.CreateElement("storedprocs"));
				foreach (IStoredProcMetadata proc in procs)
				{
					StoredProcsNode.AppendChild(StoredProcToNode(doc,proc));
				}
			}
			return doc;
		}

		public static XmlDocument TableToXml(IDatabaseMetadata Database, string TableName)
		{
			XmlDocument doc=DatabaseToDocument(Database);
			XmlNode TablesNode=doc.DocumentElement.AppendChild(doc.CreateElement("tables"));
			ITableMetadata table=null;
			foreach (ITableMetadata tbl in Database.Tables)
			{
				if (tbl.Name.ToLower()==TableName.ToLower())
				{
					table=tbl;
					break;
				}
			}
			if (table==null)
				throw new InvalidOperationException("Invalid table name");
			XmlNode TableNode=TablesNode.AppendChild(TableToNode(doc,table));
			AppendColumns(TableNode, table.Columns);
			return doc;
		}

		public static XmlDocument ViewToXml(IDatabaseMetadata Database, string ViewName)
		{
			XmlDocument doc=DatabaseToDocument(Database);
			XmlNode ViewsNode=doc.DocumentElement.AppendChild(doc.CreateElement("views"));
			IViewMetadata view=null;
			foreach (IViewMetadata v in Database.Views)
			{
				if (v.Name.ToLower()==ViewName.ToLower())
				{
					view=v;
					break;
				}
			}
			if (view==null)
				throw new InvalidOperationException("Invalid view name");
			XmlNode ViewNode=ViewsNode.AppendChild(ViewToNode(doc,view));
			AppendColumns(ViewNode, view.Columns);
			return doc;
		}

		public static XmlDocument StoredProcToXml(IDatabaseMetadata Database, string ProcName)
		{
			XmlDocument doc=DatabaseToDocument(Database);
			XmlNode StoredProcsNode=doc.DocumentElement.AppendChild(doc.CreateElement("storedprocs"));
			IStoredProcMetadata proc=null;
			foreach (IStoredProcMetadata p in Database.StoredProcs)
			{
				if (p.Name.ToLower()==ProcName.ToLower())
				{
					proc=p;
					break;
				}
			}
			if (proc==null)
				throw new InvalidOperationException("Invalid stored procedure name");
			XmlNode ProcNode=StoredProcsNode.AppendChild(StoredProcToNode(doc,proc));
			AppendColumns(ProcNode, proc.Columns);
            AppendColumns(ProcNode, "parameters", proc.Parameters);
            return doc;
		}

	}
}
