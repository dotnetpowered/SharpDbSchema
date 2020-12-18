using System;
using System.Xml;

namespace SharpDbSchema
{
	/// <summary>
	/// Factory for creating a named schema provider
	/// </summary>
	public class DbSchemaFactory
	{
		// TODO: Upgrade to .NET Core

		//public static ISchemaProvider Create(string SchemaProviderType)
		//{
		//	XmlNode SchemaConfig=null;
		//	SchemaConfig=(XmlNode) 
		//		System.Configuration.ConfigurationSettings.GetConfig("DbSchema");
		//	if (SchemaConfig==null)
		//		throw new InvalidOperationException("Missing DbSchema configuration section");
		//	XmlNode SchemaProviderNode=SchemaConfig.SelectSingleNode("SchemaProvider[@Name='"+SchemaProviderType+"']");
		//	if (SchemaProviderNode==null)
		//		throw new InvalidOperationException("Missing configuration for SchemaProvider '"+SchemaProviderType+"'");
		//	string ProviderCLRType=SchemaProviderNode.Attributes["Type"].Value;
		//	Type type=Type.GetType(ProviderCLRType);
		//	if (type==null)
		//		throw new InvalidOperationException("Unable to load SchemaProvider: "+ProviderCLRType);
		//	return (ISchemaProvider) Activator.CreateInstance(type);
		//}
	}
}
