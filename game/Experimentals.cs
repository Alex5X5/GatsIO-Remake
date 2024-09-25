using System.Threading;

namespace ShGame.game;
internal class Experimentals {
    public void Threads() {
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
                    lock (lock_)
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
        //if (thread1.ThreadState==System.Threading.ThreadState.Stopped);
    }
}
