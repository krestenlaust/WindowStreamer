using System;
using System.Windows.Forms;
using Serilog.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace LabelSink
{
    public class ToolStripLabelSink : ILogEventSink
    {
        readonly ToolStripItem label;
        readonly IFormatProvider formatProvider;

        public ToolStripLabelSink(IFormatProvider formatProvider, ToolStripItem label)
        {
            this.formatProvider = formatProvider;
            this.label = label;
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(formatProvider);
            label.Text = message;
        }
    }

    public static class ToolStripLabelSinkExtensions
    {
        public static LoggerConfiguration ToolStripLabel(
                  this LoggerSinkConfiguration loggerConfiguration, ToolStripItem label,
                  IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new ToolStripLabelSink(formatProvider, label));
        }
    }
}