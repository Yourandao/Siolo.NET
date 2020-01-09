using System.Collections.Generic;

using Nest;

namespace Siolo.NET.Components.ElasticSearch
{
	public class LogEvent
	{
		[Text(Name = "logtime")]
		public string logtime { get; set; }

		[Text(Name = "md5")]
		public string md5 { get; set; }

		[Date(Name = "@timestamp")]
		public string timestamp { get; set; }

		[Text(Name = "logdate")]
		public string logdate { get; set; }

		[Text(Name = "@version")]
		public string version { get; set; }

		[Text(Name = "event_type")]
		public string event_type { get; set; }

		[Text(Name = "ip")]
		public string ip { get; set; }

		[Text(Name = "full_class")]
		public string full_class { get; set; }

		[Object(Name = "PossibleRoutes")]
		public List<List<string>> PossibleRoutes { get; set; }

		[Object(Name = "RestrictingPolicy")]
		public string RestrictingPolicy { get; set; }
	}
}