using eventstore.contract;
using eventstore.internals;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace eventstore
{
	[TestFixture]
	public class FileEventStoreTests
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
		public void Record()
		{
			// arrange
			var testEvent0 = new Event("session", "feedbackRegistered", "payload...\nmore...");
			var testEvent1 = new Event("conference", "sessionAdded", "foo bar foo bar");
			var sut = new FileEventStore(DirPath);
			var recordedEvents = new List<IRecordedEvent>();
			sut.OnRecorded += recordedEvents.Add;

			// act 
			sut.Record(testEvent0);
			sut.Record(testEvent1);

			// assert
			recordedEvents.Count.Should().Be(2);
			recordedEvents[0].ShouldBeEquivalentTo(testEvent0, options => options.ExcludingMissingProperties());
			recordedEvents[1].ShouldBeEquivalentTo(testEvent1, options => options.ExcludingMissingProperties());
		}

		[Test]
		public void Replay()
		{
			// arrange 
			var testEvent0 = new Event("session", "feedbackRegistered", "payload...\nmore...");
			var testEvent1 = new Event("conference", "sessionAdded", "foo bar foo bar");
			var testEvent2 = new Event("session", "feedbackRegistered", "payload...\nmore...");
			var sut = new FileEventStore(DirPath);
			sut.Record(testEvent0);
			sut.Record(testEvent1);
			sut.Record(testEvent2);

			// act  
			var result = sut.Replay().ToList();

			// assert
			result.Count().Should().Be(3);
			result[0].ShouldBeEquivalentTo(testEvent0, options => options.ExcludingMissingProperties());
			result[1].ShouldBeEquivalentTo(testEvent1, options => options.ExcludingMissingProperties());
		}

		[Test]
		public void Replay_FirstSequenceNumber()
		{
			// arrange
			var testEvent0 = new Event("session", "feedbackRegistered", "payload...\nmore...");
			var testEvent1 = new Event("conference", "sessionAdded", "foo bar foo bar");
			var testEvent2 = new Event("session", "feedbackRegistered", "payload...\nmore...");
			var testEvent3 = new Event("session", "feedbackRegistered", "grade: green");
			var sut = new FileEventStore(DirPath);
			sut.Record(testEvent0);
			sut.Record(testEvent1);
			sut.Record(testEvent2);
			sut.Record(testEvent3);

			// act  
			var result = sut.Replay(2).ToList();

			// assert
			result.Count().Should().Be(2);
			result[0].ShouldBeEquivalentTo(testEvent2, options => options.ExcludingMissingProperties());
			result[1].ShouldBeEquivalentTo(testEvent3, options => options.ExcludingMissingProperties());
		}

		[Test]
		public void QueryByName()
		{
			// arrange
			const string eventName0 = "feedbackRegistered";
			const string eventName1 = "fooEvent";
			var testEvent0 = new Event("session", eventName0, "payload...\nmore...");
			var testEvent1 = new Event("conference", "sessionAdded", "foo bar foo bar");
			var testEvent2 = new Event("session", eventName0, "payload...\nmore...");
			var testEvent3 = new Event("session", eventName0, "grade: green");
			var testEvent4 = new Event("conference", eventName1, "grade: green");
			var sut = new FileEventStore(DirPath);
			sut.Record(testEvent0);
			sut.Record(testEvent1);
			sut.Record(testEvent2);
			sut.Record(testEvent3);
			sut.Record(testEvent4);

			// act  
			var result = sut.QueryByName(eventName0, eventName1).ToList();

			// assert
			result.Count().Should().Be(4);
			result[0].ShouldBeEquivalentTo(testEvent0, options => options.ExcludingMissingProperties());
			result[1].ShouldBeEquivalentTo(testEvent2, options => options.ExcludingMissingProperties());
			result[2].ShouldBeEquivalentTo(testEvent3, options => options.ExcludingMissingProperties());
			result[3].ShouldBeEquivalentTo(testEvent4, options => options.ExcludingMissingProperties());
		}

		[Test]
		public void QueryByContext()
		{
			// arrange
			const string context1 = "session";
			const string context2 = "conference";
			var testEvent0 = new Event("foo context", "fooEvent", "payload...\nmore...");
			var testEvent1 = new Event(context1, "sessionAdded", "foo bar foo bar");
			var testEvent2 = new Event("bar context", "fooEvent2", "payload...\nmore...");
			var testEvent3 = new Event(context2, "fooEvent3", "grade: green");
			var sut = new FileEventStore(DirPath);
			sut.Record(testEvent0);
			sut.Record(testEvent1);
			sut.Record(testEvent2);
			sut.Record(testEvent3);

			// act  
			var result = sut.QueryByContext(context1, context2).ToList();

			// assert
			result.Count().Should().Be(2);
			result[0].ShouldBeEquivalentTo(testEvent1, options => options.ExcludingMissingProperties());
			result[1].ShouldBeEquivalentTo(testEvent3, options => options.ExcludingMissingProperties());
		}
	}
}
