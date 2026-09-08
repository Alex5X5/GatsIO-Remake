namespace ShGame.Net.Shared;

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public unsafe class Packet : IDisposable {

	private readonly byte* data;
	
	public PacketType Type {
		get => Unsafe.Read<PacketType>(data + Protocoll.TYPE_OFFSET);
		private set => Unsafe.Write(data + Protocoll.TYPE_OFFSET, (byte)value);
	}

	public int Page {
		get => Unsafe.Read<byte>(data + Protocoll.PAGE_OFFSET);
		private set => Unsafe.Write(data + Protocoll.PAGE_OFFSET, (byte)value);
	}

	public byte* Payload {
		get => data + Protocoll.PAYLOAD_OFFSET;
	}
	
	public byte* Data {
		get => data;
	}

	public Packet(PacketType type) {
		data = (byte*)NativeMemory.AllocZeroed(Protocoll.PACKET_BYTE_LENGTH);
		Type = type;
	}

	public Packet(byte* newData) {
		data = (byte*)NativeMemory.AllocZeroed(Protocoll.PACKET_BYTE_LENGTH);
		NativeMemory.Copy(newData, data, Protocoll.PACKET_BYTE_LENGTH);
	}

	public byte[] GetBufferArray() {
		byte[] buffer = new byte[Protocoll.PACKET_BYTE_LENGTH];
		unsafe {
			fixed (byte* ptr = &buffer[0])
				NativeMemory.Copy(data, ptr, Protocoll.PACKET_BYTE_LENGTH);
		}
		return buffer;
	}

	public void Dispose() {
		NativeMemory.Free(data);
	}
}
