using eventstore.contract;
using System;

namespace eventstore.internals
{
	internal class RecordedEvent : IRecordedEvent
	{
		internal RecordedEvent(long sequenceNumber, IEvent @event)
			: this(Guid.NewGuid(), DateTime.Now.ToUniversalTime(), sequenceNumber, @event.Context, @event.Name, @event.Payload)
		{
		}

		internal RecordedEvent(Guid id, DateTime timestamp, long sequenceNumber, string context, string name, string payload)
		{
			Id = id;
			Timestamp = timestamp;
			SequenceNumber = sequenceNumber;
			Name = name;
			Context = context;
			Name = name;
			Payload = payload;
		}

		public Guid Id { get; private set; }
		public DateTime Timestamp { get; private set; }
		public long SequenceNumber { get; private set; }

		public string Context { get; private set; }
		public string Name { get; private set; }
		public string Payload { get; private set; }
	}
}
