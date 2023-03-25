using Cake.Core;
using Cake.Frosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.DescriptionTests
{
    internal class ConsoleReader : IDisposable
    {
        private readonly TextWriter _standardOutput;
        private readonly IList<string> _console;

        public ConsoleReader(Action writeToConsole) 
        {
            _standardOutput = Console.Out;
            var newOutput = new StringWriter();
            Console.SetOut(newOutput);
            
            writeToConsole();
            _console = newOutput.ToString().Split(Environment.NewLine);
            
            Console.SetOut(_standardOutput);
        }

        /// <summary>
        /// Confirms a run, starting at a regex and ending in an empty line, has exactly one line per regex expected to be in the run.
        /// </summary>
        /// <param name="startingRegex">The unique start of the run</param>
        /// <param name="expectedLinesInRun">The Regexes that have a pattern to lookup.</param>
        /// <returns>The range of all lines in the run.</returns>
        public Range ConfirmRunContains(Regex startingRegex, params Regex?[] expectedLinesInRun)
        {
            // Add 1 for the beginning of the run.
            int expectedLinesCount = expectedLinesInRun.Length + 1;

            var run = GetRun(startingRegex, RegexProvider.EmptyLine);
            var runLines = run.Keys;
            foreach (var lineRegex in expectedLinesInRun)
            {
                if (lineRegex == null)
                {
                    Assert.Fail("Expected a Regex to compare lines, but found none. Perhaps one of the immediately above assertion Failures will help find why.");
                }
                else
                {
                    var indices = GetIndicesEnumerable(lineRegex,new Range(new Index(runLines.First()),new Index(runLines.Last()))).ToList();
                    Assert.That(
                        indices.Count == 1,
                        "Expected to find 1 regex match for '{0}' in run starting at '{1}' and ending right before an empty line but instead found {2}",
                        lineRegex, 
                        startingRegex, 
                        indices.Count);
                }
            }

            Assert.That(
                run.Count, 
                Is.EqualTo(expectedLinesCount), 
                "Expected to find {0} lines starting at '{1}' and ending right before an empty line but instead found {3}",
                expectedLinesCount, startingRegex, RegexProvider.EmptyLine, run.Count);

            return new Range(new Index(run.Keys.First()), new Index(run.Keys.Last()));
        }

        public void ConfirmLineBetween(int? firstIndex, Regex regex, int? lastIndex = null)
        {
            var index = GetIndex(regex);
            Assert.That(index, Is.GreaterThan(firstIndex));
            if (lastIndex.HasValue)
            {
                Assert.That(index, Is.LessThan(lastIndex.Value));
            }
        }

        public void ConfirmAllLinesBetween(int? firstIndex, Regex regex, int? lastIndex = null)
        {
            var indices = GetIndices(regex);

            Assert.IsTrue(indices.Count > 0, $"Expected at least one instance of '{regex}' but found none.");
            Assert.IsTrue(indices.All(x => x > firstIndex), $"There was an instance of '{regex}' before line {firstIndex} but expected none.");
            if (lastIndex.HasValue)
            {
                Assert.IsTrue(indices.All(x => x < lastIndex), $"There was an instance of '{regex}' after line {lastIndex} but expected none.");
            }
        }


        public (int? index, string? line) GetIndex(Regex regex, Range range, bool allowNull = false)
        {
            var indices = GetIndicesEnumerable(regex, range).ToList();
            if (indices.Count != 1)
            {
                if (indices.Count == 0 && allowNull)
                {
                    return (null,null);
                }

                Assert.Fail($"Expected 1 instance of Regex but found {indices.Count}. Regex: '{regex.ToString()}'");
            }

            return indices.Select(x => (x, _console[x])).FirstOrDefault();
        }

        public int? GetIndex(Regex regex, bool allowNull = false)
        {
            var indices = GetIndices(regex);
            if (indices.Count != 1)
            {
                if (indices.Count == 0 && allowNull)
                {
                    return null;
                }

                Assert.Fail($"Expected 1 instance of Regex but found {indices.Count}. Regex: '{regex.ToString()}'");
            }

            return indices.FirstOrDefault();
        }

        public IList<int> GetIndices(Regex regex)
        {
            return GetIndicesEnumerable(regex, new Range(new Index(0), new Index(_console.Count - 1))).ToList();
        }

        public IDictionary<int, string> GetRun(Regex firstLineRegex, Regex delimitingLine)
        {
            return GetRunEnumerable(firstLineRegex, delimitingLine).ToDictionary(x => x.Key, x => x.Value);
        }

        private IEnumerable<int> GetIndicesEnumerable(Regex regex, Range range)
        {
            for (var i = range.Start.Value; i <= range.End.Value; i++)
            {
                if (regex.IsMatch(_console[i]))
                {
                    yield return i;
                }
            }
        }

        private IEnumerable<KeyValuePair<int, string>> GetRunEnumerable(Regex firstLineRegex, Regex delimitingLineRegex)
        {
            var singleIndexToStartRun = GetIndex(firstLineRegex);
            if (!singleIndexToStartRun.HasValue)
            {
                yield break;
            }
            for (var i = singleIndexToStartRun.Value; i < _console.Count; i++)
            {
                if (delimitingLineRegex.IsMatch(_console[i]))
                {
                    yield break;
                }

                yield return new KeyValuePair<int, string>(i, _console[i]);
            }
        }

        public void Dispose()
        {
            for (var line = 0; line < _console.Count; line++)
            {
                Console.WriteLine("{0}: {1}", line, _console[line]);
            }
        }
    }
}
