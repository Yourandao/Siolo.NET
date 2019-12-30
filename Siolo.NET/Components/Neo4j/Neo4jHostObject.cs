namespace Siolo.NET.Components.Neo4j
{
	public class Neo4jHostObject
	{
		public string Ip { get; set; }

		public bool IsSubnet { get; set; }

		public Neo4jHostObject(string ip, bool isSubnet)
		{
			this.Ip = ip;
			this.IsSubnet = isSubnet;
		}
	}
}
