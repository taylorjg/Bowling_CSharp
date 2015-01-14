using System;
using System.Collections.Generic;
using System.Linq;
using BowlingLib;
using MonadLib;

namespace BowlingApp
{
    using Lines = IEnumerable<string>;

    internal static class Formatting
    {
        public static Lines FormatFrames(IEnumerable<Frame> frames)
        {
            var emptyLines = Enumerable.Repeat(string.Empty, 9);
            var initialLines = AddFrameSeperator(emptyLines);
            return frames.Aggregate(initialLines, FormatFrame);
        }

        public static Lines FormatFrame(Lines lines, Frame frame)
        {
            return AddFrameSeperator(
                (frame.IsLastFrame)
                    ? FormatLastFrame(frame, lines)
                    : FormatNormalFrame(frame, lines));
        }

        private static Lines FormatNormalFrame(Frame frame, Lines lines)
        {
            var fn = FormatFrameNumber(frame, 3).Highlight();
            var roll1 = FormatFirstRoll(frame).Highlight();
            var roll2 = FormatSecondRoll(frame).Highlight();
            var rt = FormatRunningTotal(frame, 4).Highlight();
            return CombineLines(
                lines,
                new[]
                {
                    "-----",
                    "  " + fn,
                    "-----",
                    " |" + roll1 + "|" + roll2,
                    " +-+-",
                    "     ",
                    " " + rt,
                    "     ",
                    "-----"
                });
        }

        private static Lines FormatLastFrame(Frame frame, Lines lines)
        {
            var fn = FormatFrameNumber(frame, 5).Highlight();
            var roll1 = FormatFirstRoll(frame).Highlight();
            var roll2 = FormatSecondRoll(frame).Highlight();
            var roll3 = FormatThirdRoll(frame).Highlight();
            var rt = FormatRunningTotal(frame, 6).Highlight();
            return CombineLines(
                lines,
                new[]
                {
                    "-------",
                    "  " + fn,
                    "-------",
                    " |" + roll1 + "|" + roll2 + "|" + roll3,
                    " +-+-+-",
                    "       ",
                    " " + rt,
                    "       ",
                    "-------"
                });
        }

        private static string MakeFormatString(int width)
        {
            return string.Format("{{0,-{0}}}", width);
        }

        private static string FormatFrameNumber(Frame frame, int width)
        {
            var formatString = MakeFormatString(width);
            return string.Format(formatString, frame.FrameNumber);
        }

        private static string FormatRunningTotal(Frame frame, int width)
        {
            return frame.RunningTotal.Match(
                runningTotal =>
                {
                    var formatString = MakeFormatString(width);
                    return string.Format(formatString, runningTotal);
                },
                () => new string(' ', width));
        }

        private static string FormatFirstRoll(Frame frame)
        {
            return FormatRoll(frame.FirstRoll);
        }

        private static string FormatSecondRoll(Frame frame)
        {
            return (frame.IsSpareFrame) ? SpareSymbol : FormatRoll(frame.SecondRoll);
        }

        private static string FormatThirdRoll(Frame frame)
        {
            return FormatRoll(frame.ThirdRoll);
        }

        private const string NoRollSymbol = " ";
        private const string GutterSymbol = "-";
        private const string SpareSymbol = "/";
        private const string StrikeSymbol = "X";

        private static string FormatRoll(Maybe<int> roll)
        {
            return roll.Match(
                x =>
                {
                    if (x == 0) return GutterSymbol;
                    if (x == 10) return StrikeSymbol;
                    if (x > 0 && x < 10) return Convert.ToString(x);
                    throw new InvalidOperationException(string.Format("FormatRoll: invalid roll ({0})", x));
                },
                () => NoRollSymbol);
        }

        private static Lines AddFrameSeperator(Lines lines)
        {
            return CombineLines(
                lines,
                new[]
                    {
                        "+",
                        "|",
                        "+",
                        "|",
                        "|",
                        "|",
                        "|",
                        "|",
                        "+"
                    });
        }

        private static Lines CombineLines(Lines lines1, Lines lines2)
        {
            return lines1.Zip(lines2, (s1, s2) => s1 + s2);
        }
    }
}
