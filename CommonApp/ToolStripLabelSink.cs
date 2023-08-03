using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace LabelSink;

/// <summary>
/// <see cref="Serilog"/> sink, that outputs to a <see cref="ToolStripItem"/>.
/// </summary>
public class ToolStripLabelSink : ILogEventSink
{
    readonly ToolStripItem label;
    readonly IFormatProvider? formatProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolStripLabelSink"/> class.
    /// </summary>
    /// <param name="label">The <see cref="ToolStripItem"/> to sink logs into.</param>
    /// <param name="formatProvider">Supplies culture-specific formatting information, or null.</param>
    public ToolStripLabelSink(ToolStripItem label, IFormatProvider? formatProvider = null)
    {
        this.label = label;
        this.formatProvider = formatProvider;
    }

    /// <inheritdoc/>
    public void Emit(LogEvent logEvent)
    {
        var message = logEvent.RenderMessage(formatProvider);

        if (!label.GetCurrentParent().IsHandleCreated)
        {
            return;
        }

        label.GetCurrentParent().Invoke((MethodInvoker)delegate
        {
            label.Text = message;
        });
    }
}

public static class ToolStripLabelSinkExtensions
{
    public static LoggerConfiguration ToolStripLabel(
              this LoggerSinkConfiguration loggerConfiguration,
              ToolStripItem label,
              IFormatProvider? formatProvider = null)
    {
        return loggerConfiguration.Sink(new ToolStripLabelSink(label, formatProvider));
    }
}