
using Siolo.NET.Components.Neo4j;
using Siolo.NET.Components.Postgre;

namespace Siolo.NET.Components
{
	public class DatabaseManager
	{
		private const string Host = "localhost";

		public Mongo Mongo { get; }

		public Redis Redis { get; }

		public Neo4J Neo4J { get; }

		public Postgres Postgres { get; }

		public Elastic Elastic { get; }

		public DatabaseManager()
		{
			Mongo = new Mongo(Host, "27017", "storage");
			Redis = new Redis(Host, "6379");
			Neo4J = new Neo4J(Host, "7687", "neo4j", "test");
			Postgres = new Postgres(Host, "5432");
			//Elastic = new Elastic();
		}
	}
}
