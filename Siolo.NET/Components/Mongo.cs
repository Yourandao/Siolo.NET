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
		private readonly IMongoCollection<BsonDocument> _collection;

		private readonly IGridFSBucket _gridfs;

		public Mongo(string host, string port, string collection)
		{
			string connection = $"mongodb://{host}:{port}";

			_client = new MongoClient(connection);

			_db = _client.GetDatabase("storage");
			_collection = _db.GetCollection<BsonDocument>(collection);

			_gridfs = new GridFSBucket(_db);
		}

		public async Task Insert<T1, T2>(Dictionary<T1, T2> data)
		{
			var bson = data.ToBsonDocument();

			await _collection.InsertOneAsync(bson);
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
