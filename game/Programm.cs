using ShGame.game.Logic.PrimitiveVector3d;

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace ShGame.game;

public static class Programm {

	//[STAThread]
	public static void Main() {
		unsafe {
            ShGame.game.Logic.PrimitiveVector3d.Vector3d vector = new();
			//Vector3d* vector2 = vector;
			//Vector3d* vector3 = &vector;
            vector.Set(0, 0, 0)->Add(100)->Add(new Logic.PrimitiveVector3d.Vector3d(10,10,10));

            RuntimeTypeHandle th = vector.GetType().TypeHandle;
			int size = *(*(int**)&th + 1);
			Console.WriteLine(size);
            Console.WriteLine(Marshal.SizeOf<ShGame.game.Logic.PrimitiveVector3d.Vector3d>());
        }
		Thread.Sleep(10000);
		return;

		Stopwatch sw = new();
		sw.Start();
		//Util.Imaging.CreateImage2();
		sw.Stop();
		Console.WriteLine(sw.ElapsedMilliseconds);
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Logging.DisableColors();
		Console.WriteLine("start");
		Application.Run(new InitialScreen());
		return;
		object lock_ = new();

		Thread thread1;
		Thread thread2;
		Thread thread3;
		Thread thread4;
		Thread thread5;

		thread1 = new Thread(
				() => {
					lock (lock_)
						Monitor.Wait(lock_);
					Console.WriteLine("1");
				}
		);
		thread2 =  new Thread(
				() => {
					lock(lock_)
						Monitor.Wait(lock_);
					Console.WriteLine("2");
				}
		);
		thread3 = new Thread(
				() => {
					lock (lock_)
						Monitor.Wait(lock_);
					Console.WriteLine("3");
				}
		);

		thread4 = new Thread(
				() => {
					lock (lock_)
						Monitor.Wait(lock_);
					Console.WriteLine("4");
				}
		);
		thread5 = new Thread(
				() => {
					lock (lock_)
						Monitor.Wait(lock_);
					Console.WriteLine("5");
				}
		);

		thread1.Start();
		thread2.Start();
		thread3.Start();
		thread4.Start();
		thread5.Start();
		Thread.Sleep(3000);
		lock (lock_)
			Monitor.PulseAll(lock_);
		if(thread1.ThreadState==System.Threading.ThreadState.Stopped);

        //Task task = Task.Factory.StartNew(
        //        () => {
        //            Console.WriteLine("");
        //        }
        //);
    }
}