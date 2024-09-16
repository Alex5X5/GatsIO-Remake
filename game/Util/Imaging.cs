using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Permissions;

using static System.Windows.Forms.DataFormats;

namespace ShGame.game.Util;

class Imaging {

	public static unsafe void CreateImage(int width, int height) {

		//IntPtr dataP = (IntPtr)data.Scan0();
		Bitmap map = new(1, 1);
		var bufferAndStride = map.ToBufferAndStride();

		Console.WriteLine();
		int pixelLength = Image.GetPixelFormatSize(new Bitmap(1, 1).PixelFormat);
		Console.WriteLine(pixelLength.ToString());
		int byteCount = width * height * pixelLength;
		Console.WriteLine(byteCount.ToString());
		byte* ptr = (byte*)NativeMemory.AllocZeroed((nuint)byteCount);

		Random r = new();
		for (int i = 0; i < byteCount; i++) {
			*ptr = (byte)r.Next(255);
			//ptr+=pixelLength;
		}
		Image.FromHbitmap((nint)ptr).Save("image.png",ImageFormat.Png);
	}

	public static unsafe void CreateImage2() {
        const int width = 1000;
        const int height = 1000;
        const int bytesPerPixel = 4;  // 32-bit pixel format (e.g., ARGB)

        // Calculate the total memory required for the bitmap (width * height * bytes per pixel)
        const long totalBytes = width * height * bytesPerPixel;

		// Allocate unmanaged memory using NativeMemory.Alloc
		IntPtr unmanagedMemory = (IntPtr)NativeMemory.Alloc((nuint)totalBytes);

        try {
			// Initialize the memory with some pixel data if needed (this part is optional)
			Random random = new();
			unsafe {
                byte* ptr = (byte*)unmanagedMemory.ToPointer();
                for (int i = 0; i < totalBytes; i++) {
					//ptr[i] = (byte)ToArgb(255,255,100,0);
                    ptr[i] = (byte)random.Next(255); // Example: Fill all bytes with 255 (white pixels)
                }
            }

            // Create a Bitmap object from the unmanaged memory
            Bitmap bitmap = new(
                width,
                height,
                width * bytesPerPixel, // Stride (bytes per row)
                PixelFormat.Format32bppArgb, // 32-bit pixel format (ARGB)
                unmanagedMemory // Pointer to the unmanaged memory
            );

			// Save or display the bitmap
			bitmap.Save("output.png");

            // Dispose of the bitmap when done
            bitmap.Dispose();
        } finally {
            // Free unmanaged memory when done
            NativeMemory.Free((void*)unmanagedMemory);
        }
    }

    private static uint ToArgb(byte alpha, byte red, byte green, byte blue) {
        //Alpha(255 or 0xFF): Shifted 24 bits to the left → 0xFF000000
        //Red(255 or 0xFF): Shifted 16 bits to the left → 0x00FF0000
        //Green(0 or 0x00): Shifted 8 bits to the left → 0x00000000
        //Blue(0 or 0x00): Stays in place → 0x00000000
        return (uint)((alpha << 24) | (red << 16) | (green << 8) | blue);
    }

    public static unsafe void Iterate() {
	}

	public static unsafe void Iterate2() {
		//int[,] array = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
		int[,] array = new int[1000,100];
		for (int i = 0; i < array.GetLength(0); i++)
			for (int j = 0; j < array.GetLength(1); j++)
				array[i,j] = i*j;

		Stopwatch s = Stopwatch.StartNew();
		s.Start();

		fixed (int* p = array) {
			int rows = array.GetLength(0);
			int cols = array.GetLength(1);

			for (int i = 0; i < rows; i++)
				for (int j = 0; j < cols; j++) {
					int* ptr = p + (i * cols + j);
					Console.WriteLine(*ptr);
				}
		}

		s.Stop();

		Stopwatch s2 = Stopwatch.StartNew();
		s2.Start();

		for (int i = 0; i < array.GetLength(0); i++)
			for (int j = 0; j < array.GetLength(1); j++)
				Console.WriteLine(array[i,j]);
		s2.Stop();

		Console.WriteLine("ms1:"+s.ElapsedMilliseconds);
		Console.WriteLine("ms2:"+s2.ElapsedMilliseconds);
	}

	public void ProcessImageArea() {

	}
}

public class BitmapContainer {
	public PixelFormat Format { get; }

	public int Width { get; }

	public int Height { get; }

	public IntPtr Buffer { get; }

	public int Stride { get; set; }

	public BitmapContainer(Bitmap bitmap) {
		ArgumentNullException.ThrowIfNull(bitmap);
		Format = bitmap.PixelFormat;
		Width = bitmap.Width;
		Height = bitmap.Height;

		var bufferAndStride = Helper.ToBufferAndStride(bitmap);
		Buffer = bufferAndStride.Item1;
		Stride = bufferAndStride.Item2;
	}

	public Bitmap ToBitmap() {
		return new Bitmap(Width, Height, Stride, Format, Buffer);
	}
}

static class Helper {

	public static Tuple<IntPtr, int> ToBufferAndStride(this Bitmap bitmap) {
		BitmapData? bitmapData = null;

		try {
			bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
				ImageLockMode.ReadOnly, bitmap.PixelFormat);

			return new Tuple<IntPtr, int>(bitmapData.Scan0, bitmapData.Stride);
		} finally {
			if (bitmapData != null)
				bitmap.UnlockBits(bitmapData);
		}
	}
}