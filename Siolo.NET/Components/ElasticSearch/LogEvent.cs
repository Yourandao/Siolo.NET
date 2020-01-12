using System.Collections.Generic;

using Nest;

namespace Siolo.NET.Components.ElasticSearch
{
	public class LogEvent
	{
		[Text(Name = "logtime")]
		public string Logtime { get; set; }

		[Text(Name = "md5")]
		public string Md5 { get; set; }

		[Date(Name = "@timestamp")]
		public string Timestamp { get; set; }

		[Text(Name = "logdate")]
		public string Logdate { get; set; }

		[Text(Name = "@version")]
		public string Version { get; set; }

		[Text(Name = "event_type")]
		public string EventType { get; set; }

		[Text(Name = "ip")]
		public string Ip { get; set; }

		[Text(Name = "full_class")]
		public string FullClass { get; set; }

		[Object(Name = "PossibleRoutes")]
		public List<List<string>> PossibleRoutes { get; set; }

		[Object(Name = "RestrictingPolicy")]
		public string RestrictingPolicy { get; set; }
	}
}