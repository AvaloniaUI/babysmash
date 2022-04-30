﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Pointer = Avalonia.Input.Pointer;

namespace BabySmash
{
    public class WordFinder
    {
        private const int MinimumWordLength = 2, MaximumWordLength = 15;

        private bool wordsReady;

        private HashSet<string> words = new HashSet<string>();

        public WordFinder(string wordsFilePath)
        {


            // File path provided should be relative to our running location, so combine for full path safety.
            // You should use AppContext.BaseDirectory on .NET 5 to get the truth
            var basedir = Assembly.GetExecutingAssembly().Location;
            if (string.IsNullOrEmpty(basedir))
            {
                basedir = AppContext.BaseDirectory;
            }
            var dir = Path.GetDirectoryName(basedir);
            wordsFilePath = Path.Combine(dir, wordsFilePath);

            StreamReader defaultSr;

            // Bail if the source word file is not found.
            if (!File.Exists(wordsFilePath))
            {
                // Source word file was not found; place a 'words.txt' file next to BabySmash.exe to enable combining 
                // letters into typed words. Some common names may work too (but successful OS speech synth may vary).
                
                
                // Try loading the default wordlist from the resource file.
                
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>()!;
                var resxPath = new Uri(@"avares://BabySmash/Resources/Strings/Words.txt");

                if (assets.Exists(resxPath))
                {
                    defaultSr = new StreamReader(assets.Open(resxPath));
                }
                else
                {
                    return;
                }
            }
            else
            {
                defaultSr = new StreamReader(wordsFilePath);
            }

            // Load up the string dictionary in the background.
            var t = new Thread(() =>
            {
                // Read through the word file and create a hashtable entry for each one with some 
                // further parsed word data (such as various game scores, etc)
                ParseWordList(defaultSr);
            })
            {
                IsBackground = true
            };
            t.Start();
        }

        private void ParseWordList(StreamReader sr)
        {
            var s = sr.ReadLine();
            while (s != null)
            {
                // Ignore invalid lines, comment lines, or words which are too short or too long.
                if (!s.Contains(";") && !s.Contains("/") && !s.Contains("\\") &&
                    s.Length >= MinimumWordLength && s.Length <= MaximumWordLength)
                {
                    words.Add(s.ToUpper());
                }

                s = sr.ReadLine();
            }

            // Store all words into separate buckets based on the last letter for faster compares.

            // Mark that we're done loading so we can speak words instead of just letters.
            wordsReady = true;
            
        }

        public string LastWord(List<UserControl> figuresQueue)
        {
            // If not done loading, or could not yet form a word based on queue length, just abort.
            var figuresPos = figuresQueue.Count - 1;
            if (!wordsReady || figuresPos < MinimumWordLength - 1)
            {
                return null;
            }

            // Loop while the most recently pressed things are still letters; loop proceeds from the
            // most recent letter, back towards the beginning, as we only care about the longest word
            // that we JUST now finished typing.
            string longestWord = null;
            var stringToCheck = new StringBuilder();
            var lowestIndexToCheck = Math.Max(0, figuresPos - MaximumWordLength);
            while (figuresPos >= lowestIndexToCheck)
            {
                var lastFigure = figuresQueue[figuresPos] as CoolLetter;
                if (lastFigure == null)
                {
                    // If we encounter a non-letter, move on with the best word so far (if any).
                    // IE typing "o [bracket] p e n" can match word "pen" but not "open" since our
                    // intention back at [bracket] shows we don't necessarily mean to type "open".
                    break;
                }

                // Build up the string and check to see if it is a word so far.
                stringToCheck.Insert(0, lastFigure.Character);
                var s = stringToCheck.ToString();
                if (words.Contains(stringToCheck.ToString(), StringComparer.InvariantCultureIgnoreCase) && s.Length >= MinimumWordLength)
                {
                    // Since we're progressively checking longer and longer letter combinations,
                    // each time we find a word, it is our new "longest" word so far.
                    longestWord = s;
                }

                figuresPos--;
            }

            return longestWord;
        }

        public void AnimateLettersIntoWord(List<UserControl> figuresQueue, string lastWord)
        {
            // Prepare to animate the letters into their respective positions, on each screen.
            var duration = TimeSpan.FromMilliseconds(1200);
            var totalLetters = lastWord.Length;

            var wordCenter = FindWordCenter(figuresQueue, totalLetters);
            var wordSize = FindWordSize(figuresQueue, totalLetters);
            var wordLeftEdge = wordCenter.X - wordSize.X / 2f;

            // Figure out where to move each letter used in the word; find the letters used based on
            // the word length; they are the last several figures in the figures queue.
            for (var i = figuresQueue.Count - 1; i >= figuresQueue.Count - totalLetters; i--)
            {
                var currentFigure = figuresQueue[i];

         
                // We know where we want to center the word, and the word's left edge based on figure
                // sizes, and now just need to figure out how far from that left edge we need to adjust
                // to make this letter move to the correct relative position to spell out the word.
                var wordOffsetX = 0d;
                for (var j = figuresQueue.Count - totalLetters; j < i; j++)
                {
                    wordOffsetX += figuresQueue[j].Width;
                }

                // Start translating from wherever we were already translated to (or 0 if not yet
                // translated) and going to the new position for this letter based for the word.
                var wordTranslationX = wordLeftEdge - Canvas.GetLeft(currentFigure);
                var wordTranslationY = wordCenter.Y - Canvas.GetTop(currentFigure);
                AnimationHelpers.TranslateFigure(currentFigure,  wordTranslationX + wordOffsetX, wordTranslationY);
                // var animationX = new DoubleAnimation(transform.X,, duration);
                // var animationY = new DoubleAnimation(transform.Y, , duration);
                // transform.BeginAnimation(TranslateTransform.XProperty, );
                // transform.BeginAnimation(TranslateTransform.YProperty, animationY);
                
                
            }
        }

        private Point FindWordCenter(List<UserControl> letterQueue, int letterCount)
        {
            // For now, target centering the word at the average position of all its letters.
            var x = (from c in letterQueue select Canvas.GetLeft(c)).Reverse().Take(letterCount).Average();
            var y = (from c in letterQueue select Canvas.GetTop(c)).Reverse().Take(letterCount).Average();
            return new Point(x, y);
        }

        private Point FindWordSize(List<UserControl> letterQueue, int letterCount)
        {
            var x = (from c in letterQueue select c.Width).Reverse().Take(letterCount).Sum();
            var y = (from c in letterQueue select c.Height).Reverse().Take(letterCount).Max();
            return new Point(x, y);
        }

        private TranslateTransform FindOrAddTranslationTransform(TransformGroup transformGroup)
        {
            var translationTransform = (from t in transformGroup.Children
                                        where t is TranslateTransform
                                        select t).FirstOrDefault() as TranslateTransform;
            if (translationTransform == null)
            {
                translationTransform = new TranslateTransform();
                transformGroup.Children.Add(translationTransform);
            }

            return translationTransform;
        }
    }
}