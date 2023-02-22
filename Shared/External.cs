using System.IO;
using System.Windows.Forms;

namespace Shared
{
    // public struct Resolution { public int Width; public int Height; }

    // Thanks: nmarkou.blogspot.com/2011/12/redirect-console-output-to-textbox.html?showComment=1553282531886#c8782177256700696736
    public class LogStreamWriter : StringWriter
    {
        protected StreamWriter writer;
        protected MemoryStream mem;

        RichTextBox textboxOutput;

        public LogStreamWriter(RichTextBox richTextBox)
        {
            textboxOutput = richTextBox;
            mem = new MemoryStream(1000000);
            writer = new StreamWriter(mem);
            writer.AutoFlush = true;
        }

        public override void Write(char value)
        {
            base.Write(value);
            textboxOutput.AppendText(value.ToString());
            writer.Write(value);
        }

        public override void Write(string value)
        {
            base.Write(value);
            textboxOutput.AppendText(value);
            writer.Write(value);
        }

        public override void Write(object value)
        {
            base.Write(value);
            textboxOutput.AppendText(value.ToString());
            writer.Write(value);
        }

        public override void WriteLine(string value)
        {
            base.WriteLine(value);
            textboxOutput.AppendText(value);
            writer.Write(value);
        }

        public override void WriteLine(object value)
        {
            base.WriteLine(value);
            textboxOutput.AppendText(value.ToString());
            writer.Write(value);
        }
    }
}
