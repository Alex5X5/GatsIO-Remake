namespace ShGame.Drawing;

using System.Runtime.InteropServices;

internal unsafe class TriangleBuffer : IDisposable {

	private uint count;
	public uint Count {
		get => count;
		private set => count = value;
	}

	private uint capacity;
	public uint Capacity {
		get => capacity;
		private set => capacity = value;
	}

	private TriangleShape* buffer;
	public byte* Data {
		get {
			byte* data = (byte*)buffer;
			return data;
		}
	}

	public TriangleBuffer() {
		Capacity = 0;
		Count = 0;
		EnsureCapacity(1);
	}

	private void EnsureCapacity(uint requested) {
		if (requested<=Capacity)
			return;
		uint newCapacity = Capacity;
		if (newCapacity==0)
			newCapacity = 1;
		while (newCapacity < requested) {
			newCapacity*=2;
		}
		TriangleShape* ptr = (TriangleShape*)NativeMemory.AllocZeroed(newCapacity*TriangleShape.SizeInBytes);
		NativeMemory.Copy(buffer, ptr, Capacity*TriangleShape.SizeInBytes);
		Capacity = newCapacity;
		buffer = ptr;
	}

	public unsafe void Buffer(TriangleShape* triangle, uint triangleCount) {
		uint requestedCapacity = Count + triangleCount;
		EnsureCapacity(requestedCapacity);
		TriangleShape* buffer_ = buffer + Count;
		NativeMemory.Copy(triangle, buffer_, triangleCount*TriangleShape.SizeInBytes);
		Count += triangleCount;
	}

	public void Clear() {
		NativeMemory.Free(buffer);
		Capacity = 0;
		Count = 0;
		EnsureCapacity(1);
	}

	public void Dispose() {
		NativeMemory.Free(buffer);
	}
}
