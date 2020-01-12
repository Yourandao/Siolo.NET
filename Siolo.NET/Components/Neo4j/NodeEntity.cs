namespace Siolo.NET.Components.Neo4j
{
	public class NodeEntity
	{
		public string subnet { get; set; }

		public string ip { get; set; }

		public NodeEntity(string ip, bool subnet)
		{
			this.subnet = subnet.ToString();
			this.ip = ip;
		}

		public bool IsSubnet() => bool.Parse(subnet);
	}
}
