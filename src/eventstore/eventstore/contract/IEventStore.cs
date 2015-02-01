using System;
using System.Collections.Generic;

namespace eventstore.contract
{
	public interface IEventStore
	{
		void Record(IEvent @event);
		IEnumerable<IRecordedEvent> Replay();
		IEnumerable<IRecordedEvent> Replay(long firstSequenceNumber);
		IEnumerable<IRecordedEvent> QueryByName(params string[] eventNames);
		IEnumerable<IRecordedEvent> QueryByContext(params string[] contexts);

		event Action<IRecordedEvent> OnRecorded;
	}
}
