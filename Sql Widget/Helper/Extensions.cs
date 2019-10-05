using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sql_Widget.Helper
{
	public static class Extensions
	{
		public static async Task TimeoutAfter(this Task task, int timeout)
		{
			using (var timeoutCancellationTokenSource = new CancellationTokenSource())
			{

				var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
				if (completedTask == task)
				{
					timeoutCancellationTokenSource.Cancel();
					await task;  // Very important in order to propagate exceptions
				}
				else
				{
					throw new TimeoutException("The operation has timed out.");
				}
			}
		}
	}
}
