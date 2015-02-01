using eventstore.contract;

namespace eventstore.internals
{
	public class Event : IEvent
	{
		public Event(string context, string name, string payload)
		{
			Context = context;
			Name = name;
			Payload = payload;
		}

		public string Context { get; private set; }
		public string Name { get; private set; }
		public string Payload { get; private set; }
	}
}
