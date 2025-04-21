using Verse;

namespace Logistics
{
    public enum TerminalType
    {
        Input, Output, IO
    }

    public interface ITerminal
    {
        Thing Thing { get; }
        TerminalType TermType { get; }
    }
}
