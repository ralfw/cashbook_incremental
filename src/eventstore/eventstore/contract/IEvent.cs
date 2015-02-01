namespace eventstore.contract
{
	public interface IEvent
	{
		string Context { get; }
		string Name { get; }
		string Payload { get; }
	}
}
