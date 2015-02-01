using System;

namespace eventstore.contract
{
	public interface IRecordedEvent : IEvent
	{
		Guid Id { get; }
		DateTime Timestamp { get; }
		long SequenceNumber { get; }
	}
}
