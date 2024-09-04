using System.Collections.Concurrent;

namespace ShGame.game.Util;
public unsafe class TempData<T> where T : unmanaged {

	public T* data;
	public T* write;

	public int Length => (int)(write-data);

	public TempData() {
		data=(T*)Allocator.Alloc(sizeof(T));
	}

	public void PreMeshing() {
		write=data;
	}

	public void Dispose() {
		Allocator.Free(ref data);
		write=null;
	}
}

public static unsafe class TempStorageAllocator<T> where T : unmanaged {

	const int BURST_THRESHOLD = 32;
	//a collection TempData objects
	private static readonly BlockingCollection<TempData<T>> storage = [];

	public static TempData<T> Get() {
		// Reuse
		if(storage.TryTake(out var data))
			return data;

		// Burst
		return new();
	}

	public static void Recycle(ref TempData<T> data) {
		if(data==null) {
			return;
		}

		// Dispose burst data rather than keeping them around forever
		if(storage.Count>=BURST_THRESHOLD) {
			data.Dispose();
			data=null;
			return;
		}

		storage.Add(data);
		data=null;
	}
}
