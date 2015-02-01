using eventstore.internals;
using eventstore.internals.file;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace eventstore
{
	[TestFixture]
	public class FileStoreTests
	{
		private const string DirPath = "eventStoreDir";

		[SetUp]
		public void Init()
		{
			if (Directory.Exists(DirPath))
			{
				Directory.Delete(DirPath, true);
			}
		}

		[Test]
		public void WriteAndRead()
		{
			// arrange 
			const string fileName = "fooEvent.txt";
			var testEvent = new RecordedEvent(Guid.NewGuid(), DateTime.Now, 0, "session", "feedbackRegistered", "foo bar payload");
			var sut = new FileStore(DirPath);

			// act 
			sut.Write(fileName, testEvent);
			var result = sut.ReadAll().ToList();

			// assert
			result.Count().Should().Be(1);
			result[0].Context.Should().Be(testEvent.Context);
			result[0].Name.Should().Be(testEvent.Name);
			result[0].Payload.Should().Be(testEvent.Payload);
		}

		[Test]
		public void GetNextSequenceNumber()
		{
			// arrange 
			var sut = new FileStore(DirPath);

			// act + assert
			sut.GetNextSequenceNumber().Should().Be(0);
			sut.Write("fooEvent1.txt", new RecordedEvent(Guid.NewGuid(), DateTime.Now, 0, "session", "feedbackRegistered", "foo bar payload"));
			sut.GetNextSequenceNumber().Should().Be(1);
			sut.Write("fooEvent2.txt", new RecordedEvent(Guid.NewGuid(), DateTime.Now, 1, "conference", "sessionAdded", "foo bar"));
			sut.GetNextSequenceNumber().Should().Be(2);
		}
	}
}
