using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Siolo.NET.Components
{
	public class Mongo
	{
		private readonly MongoClient _client;

		private readonly IMongoDatabase _db;
		private readonly IMongoCollection<BsonDocument> _report_collection;

		private readonly IGridFSBucket _gridfs;

		public Mongo(string host, string port, string report_collection)
		{
			string connection = $"mongodb://root:example@{host}:{port}";

			_client = new MongoClient(connection);

			_db = _client.GetDatabase("storage");
			_report_collection = _db.GetCollection<BsonDocument>(report_collection);

			_gridfs = new GridFSBucket(_db);
		}

		public async Task InsertReport(string hash, string data)
		{
			var bson = (new Dictionary<string, string>() {  { "hash" , hash },
																			{ "data" , data }
																		}).ToBsonDocument();

			await _report_collection.InsertOneAsync(bson);
		}

		public async Task<string> GetReport(string hash)
		{
			var result = await _report_collection.FindAsync(new BsonDocument("hash", hash));

			var single_or_default = result.SingleOrDefault();
			var data = single_or_default is null ? null : single_or_default["data"];

			return data is null ? "" : data.ToString();
		}

		public async Task<string> UploadFile(string filePath)
		{
			await using (var fs = new FileStream(filePath, FileMode.Open))
			{
				var id = await _gridfs.UploadFromStreamAsync(Path.GetFileName(filePath), fs);

				return id.ToString();
			}
		}

		public async Task<string> UploadFile(string fileName, Stream fileStream)
		{
			var id = await _gridfs.UploadFromStreamAsync(fileName, fileStream);

			return id.ToString();
		}

		public async Task GetFile(string filename)
		{
			await using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
			{
				await _gridfs.DownloadToStreamByNameAsync(filename, fs);
			}
		}
	}
}
