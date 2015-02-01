using eventstore.contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace eventstore.internals.file
{
	internal class FileStore
	{
		private readonly string dirPath;

		internal FileStore(string path)
		{
			dirPath = path;
			var dir = new DirectoryInfo(path);
			if (!dir.Exists)
			{
				dir.Create();
			}
		}

		internal void Write(string filename, IRecordedEvent @event)
		{
			using (var writer = new StreamWriter(GetFullFilePath(filename)))
			{
				writer.WriteLine(@event.Id);
				writer.WriteLine(@event.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"));
				writer.WriteLine(@event.SequenceNumber);

				writer.WriteLine(@event.Context);
				writer.WriteLine(@event.Name);
				writer.Write(@event.Payload);
			}
		}

		private string GetFullFilePath(string fileName)
		{
			return Path.Combine(dirPath, fileName);
		}

		internal IEnumerable<IRecordedEvent> ReadAll()
		{
			return Directory.GetFiles(dirPath).Select(Read);
		}

		private static IRecordedEvent Read(string fileName)
		{
			using (var reader = new StreamReader(fileName))
			{
				var id = Guid.Parse("" + reader.ReadLine());
				var timeStamp = DateTime.Parse(reader.ReadLine());
				var sequenceNumber = long.Parse("" + reader.ReadLine());
				var context = reader.ReadLine();
				var name = reader.ReadLine();
				var payload = reader.ReadToEnd();
				return new RecordedEvent(id, timeStamp, sequenceNumber, context, name, payload);
			}
		}

		internal long GetNextSequenceNumber()
		{
			return Directory.GetFiles(dirPath).Length;
		}
	}
}
