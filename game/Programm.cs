using System.Threading;
using System.Windows.Forms;
namespace ShGame.game;

public static class Programm {

	//[STAThread]
	public static void Main() {
		unsafe {
			int[] key = [12311231];
            string s = "testtesttest";
            char[] chars = s.ToCharArray();
            byte[] bytes = new byte[chars.Length*sizeof(char)];
            for (int i = 0; i < chars.Length; i++) {
				byte[] buffer = new byte[sizeof(char)]; 
				buffer = BitConverter.GetBytes(chars[i]);
				buffer.CopyTo(bytes, i*sizeof(char));
            }
			for (int i = 0; i < bytes.Length/sizeof(char); i++)
				Console.WriteLine("byte:"+BitConverter.ToChar(bytes,i*sizeof(char)));
        }

		Application.EnableVisualStyles();
		Console.WriteLine(sizeof(char));
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