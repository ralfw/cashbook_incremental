namespace eventstore.internals
{
	using System;
	using System.Threading;

	internal static class ReaderWriterLockSlimExtensions
	{
		public static void Read(this ReaderWriterLockSlim rwLock, Action toBeExecuted)
		{
			rwLock.EnterReadLock ();
			try {
				toBeExecuted();
			} finally {
				rwLock.ExitReadLock ();
			}
		}

		public static void Write(this ReaderWriterLockSlim rwLock, Action toBeExecuted)
		{
			rwLock.EnterWriteLock ();
			try {
				toBeExecuted();
			} finally {
				rwLock.ExitWriteLock ();
			}
		}
	}
}
