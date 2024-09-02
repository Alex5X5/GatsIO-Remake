using System.Diagnostics;
using System.IO;
using System.Text;

namespace ShGame.game.Util
{
    public class ConsoleRedirector:TextWriter {
        public override Encoding Encoding => Encoding.Default;

        public override void WriteLine(string value) {
            Debug.WriteLine(value);
        }

        public override void Write(char value) {
            Debug.Write(value);
        }
    }
}
