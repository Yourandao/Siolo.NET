namespace Siolo.NET.Components.Postgre
{
	public class HostInfo
	{
		public int Mask { get; set; }

		public string Ip { get; set; }

		public string Wildcart { get; set; }

		public HostInfo(int mask, string ip, string wildcart)
		{
			this.Mask = mask;
			this.Ip = ip;
			this.Wildcart = wildcart;
		}
	}
}
