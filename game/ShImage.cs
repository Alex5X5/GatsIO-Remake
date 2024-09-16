using System;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows;
using System.Runtime.InteropServices;

namespace sh_game.game {
	internal class ShImage {

        //private static WriteableBitmap writeableBitmap;
		private static Image i;

		//[STAThread]
		//static void Main(string[] args) {
		//	i=new Bitmap(100, 100);
		//writeableBitmap=new WriteableBitmap(
		//		1,
		//		1,
		//	   96,
		//	   96,
		//	   PixelFormats.Bgr32,
		//	   null);
		//}

		// The DrawPixel method updates the WriteableBitmap by using
		// unsafe code to write a pixel into the back buffer.
		static void DrawPixel(System.Windows.Forms.MouseEventArgs e) {
			int column = (int)e.Location.X;
			int row = (int)e.Location.Y;

			try {
				// Reserve the back buffer for updates.
				//writeableBitmap.Lock();

				unsafe {
					Image image = new Bitmap(10, 10);
					// Get a pointer to the back buffer.
					//IntPtr pBackBuffer = writeableBitmap.BackBuffer;

					// Find the address of the pixel to draw.
					//pBackBuffer+=row*writeableBitmap.BackBufferStride;
					//pBackBuffer+=column*4;

					// Compute the pixel's color.
					int color_data = 255<<16; // R
					color_data|=128<<8;   // G
					color_data|=255<<0;   // B

					// Assign the color data to the pixel.
					//*((int*)pBackBuffer)=color_data;
				}

				// Specify the area of the bitmap that changed.
				//writeableBitmap.AddDirtyRect(new Rectangle(column, row, 1, 1));
			} finally {
				// Release the back buffer and make it available for display.
				//writeableBitmap.Unlock();
			}
		}
	}
}