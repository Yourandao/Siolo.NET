namespace Siolo.NET.Components.Neo4j
{
	public class Neo4jRelation
	{
		public string @from { get; set; }

		public string to { get; set; }

		public Neo4jRelation(string from, string to)
		{
			this.@from = from;
			this.to = to;
		}
	}
}
