using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

/*
 * Idea: [ExposeAs(Type)] to make a field have a different type when accessed from outside (needs to be assignable from concrete type)
 */ 

public class AwaitAny<TResult, TParams>
{
	public delegate Task<TResult> Callback(TParams p, CancellationToken c);

	private HashSet<Callback> _callbacks = new HashSet<Callback>();

	public async Task<TResult> Invoke(TParams p)
	{
		var cts = new CancellationTokenSource();
		var tasks = _callbacks.Select(c => c(p, cts.Token));
		var a = await Task.WhenAny(tasks);
		cts.Cancel();
		return a.Result;
	}

	// operators

	public static AwaitAny<TResult, TParams> operator +(AwaitAny<TResult, TParams> self, Callback cb)
	{
		self._callbacks.Add(cb);
		return self;
	}

	public static AwaitAny<TResult, TParams> operator -(AwaitAny<TResult, TParams> self, Callback cb)
	{
		self._callbacks.Remove(cb);
		return self;
	}
}
