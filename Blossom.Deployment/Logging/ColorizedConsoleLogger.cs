using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace Blossom.Deployment.Logging
{
    public class ColorProfile : ICloneable
    {
        /// <summary>
        /// A Regular Expression describing the text to
        /// color. If there are any capture groups,
        /// the first is used when choosing the text
        /// to color. Otherwise, the entire match is
        /// used.
        /// </summary>
        public string MatchExpr { get; set; }

        /// <summary>
        /// Any extra options to use when matching.
        /// Defaults to None.
        /// </summary>
        public RegexOptions RegexOptions { get; set; }

        /// <summary>
        /// Set of logging levels the profile applies to.
        /// If null or empty, all levels are eligible.
        /// </summary>
        public HashSet<LogLevel> LogLevels { get; set; }

        /// <summary>
        /// Foreground color for the match.
        /// </summary>
        public Color? Foreground { get; set; }

        /// <summary>
        /// Background color for the match.
        /// </summary>
        public Color? Background { get; set; }

        public ColorProfile()
        {
            LogLevels = new HashSet<LogLevel>();
        }

        public object Clone()
        {
            return new ColorProfile
            {
                MatchExpr = MatchExpr,
                LogLevels = LogLevels,
                Foreground = Foreground,
                Background = Background
            };
        }
    }

    /// <summary>
    /// Simple threadsafe logger implementation that
    /// prints to the console.
    /// </summary>
    public class ColorizedConsoleLogger : SimpleConsoleLogger
    {
        private static readonly ColorProfile NullProfile = new ColorProfile();

        public readonly List<ColorProfile> Profiles; 

        public ColorizedConsoleLogger()
        {
            Profiles = new List<ColorProfile>
                {
                    new ColorProfile
                        {
                           MatchExpr = "beginning task",
                           RegexOptions = RegexOptions.IgnoreCase,
                           Foreground = Color.DarkGreen
                        },
                    new ColorProfile
                        {
                            LogLevels = new HashSet<LogLevel>
                                {
                                    LogLevel.Error,
                                    LogLevel.Fatal
                                },
                            MatchExpr = ".+",
                            Foreground = Color.Red
                        }
                };
        }

        private static ConsoleColor ToConsoleColor(Color color)
        {
            ConsoleColor ret = 0;
            var delta = double.MaxValue;

            foreach (ConsoleColor cc in Enum.GetValues(typeof(ConsoleColor)))
            {
                var n = Enum.GetName(typeof(ConsoleColor), cc);
                var c = Color.FromName(n == "DarkYellow" ? "Orange" : (n ?? "White")); // bug fix
                var t = Math.Pow(c.R - color.R, 2.0) + Math.Pow(c.G - color.G, 2.0) + Math.Pow(c.B - color.B, 2.0);
                if (Equals(t, 0.0))
                    return cc;
                if (t < delta)
                {
                    delta = t;
                    ret = cc;
                }
            }
            return ret;
        }

        protected override void WriteLog(
            LogLevel level, string title, string message, Exception exception = null)
        {
            var text = title + TitleSeparator + message;
            var mapping = new ColorProfile[text.Length];

            var profiles = Profiles.Where(
                p => p.LogLevels == null ||
                !p.LogLevels.Any() ||
                p.LogLevels.Contains(level));

            foreach (var colorProfile in profiles)
            {
                var match = Regex.Match(text, colorProfile.MatchExpr, colorProfile.RegexOptions);
                if (!match.Success) continue;

                // If present, use the first explicit group instead
                // of the whole match.
                var group = match.Groups.Count > 1 ?
                    match.Groups[1] :
                    match.Groups[0] ;

                var start = group.Index;
                var end = group.Index + group.Length;

                var started = false;
                for (var ix = start; ix < end; ix++)
                {
                    if (!started)
                    {
                        started = true;
                        mapping[ix] = colorProfile;
                    }
                    else
                    {
                        mapping[ix] = null;
                    }
                }
                if (end < mapping.Length)
                {
                    mapping[end] = NullProfile;
                }
            }

            var oldFg = Console.ForegroundColor;
            var oldBg = Console.BackgroundColor;
            //var prevFg = oldFg;
            //var prevBg = oldBg;
            var fg = oldFg;
            var bg = oldBg;

            var i = 0;
            while(i < text.Length)
            {
                var start = i;
                
                // Set new foreground
                if (mapping[i] != null)
                {
                    // Go back to old colors
                    if (ReferenceEquals(mapping[i], NullProfile))
                    {
                        fg = oldFg;
                        bg = oldBg;
                    }
                    // Use new colors
                    else
                    {
                        //prevFg = fg;
                        //prevBg = bg;
                        if (mapping[i].Foreground.HasValue)
                        {
                            fg = ToConsoleColor(mapping[i].Foreground.Value);
                        }
                        if (mapping[i].Background.HasValue)
                        {
                            bg = ToConsoleColor(mapping[i].Background.Value); 
                        } 
                    }
                }

                // Find end
                i++;
                while (i < mapping.Length && mapping[i] == null)
                {
                    i++;
                }

                Console.ForegroundColor = fg;
                Console.BackgroundColor = bg;
                Console.Write(text.Substring(start, i - start));
            }

            Console.WriteLine();
            Console.ForegroundColor = oldFg;
            Console.BackgroundColor = oldBg;
        }
    }
}