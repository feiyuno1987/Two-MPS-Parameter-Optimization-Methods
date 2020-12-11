﻿namespace ConsoleProgressBar
{
    internal interface IConsole
    {
        int CursorTop { get; }
        int CursorLeft { get; }
        int WindowWidth { get; }
        int BufferHeight { get; }
        void SetCursorPosition(int left, int top);
        void WriteLine();
        void WriteLine(string value);
        void Write(string value);
    }
}