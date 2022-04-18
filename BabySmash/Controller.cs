using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using BabySmash.Properties;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace BabySmash
{
    public class Controller
    {
        public bool isOptionsDialogShown { get; set; }
        private bool isDrawing;

        // TODO: Disabled Speech features since it's not cross platform compatible as well.
        // private readonly SpeechSynthesizer objSpeech = new SpeechSynthesizer();
        private readonly List<MainWindow> windows = new List<MainWindow>();

        private DispatcherTimer timer = new DispatcherTimer();
        private Queue<Shape> ellipsesQueue = new Queue<Shape>();

        private Dictionary<string, List<UserControl>> figuresUserControlQueue =
            new Dictionary<string, List<UserControl>>();

        // TODO: Disabled ApplicationDeployment since ClickOnce doesnt work in dotnet core.
        // private ApplicationDeployment deployment = null;
        private WordFinder wordFinder = new WordFinder("Words.txt");

        /// <summary>Prevents a default instance of the Controller class from being created.</summary>
        private Controller()
        {
        }

        public static Controller Instance { get; } = new Controller();

        // TODO: Disabled ApplicationDeployment since ClickOnce doesnt work in dotnet core.
        /*
        void deployment_CheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e)
        {
            if (e.Error == null && e.UpdateAvailable)
            {
                try
                {
                    MainWindow w = this.windows[0];
                    w.updateProgress.Value = 0;
                    w.UpdateAvailableLabel.Visibility = Visibility.Visible;

                    deployment.UpdateAsync();
                }
                catch (InvalidOperationException ex)
                {
                    Debug.WriteLine(ex.ToString());
                    MainWindow w = this.windows[0];
                    w.UpdateAvailableLabel.Visibility = Visibility.Hidden;
                }
            }
        }

        void deployment_UpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            MainWindow w = this.windows[0];
            w.updateProgress.Value = e.ProgressPercentage;
        }

        void deployment_UpdateCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Debug.WriteLine(e.ToString());
                return;
            }
            MainWindow w = this.windows[0];
            w.UpdateAvailableLabel.Visibility = Visibility.Hidden;
        }*/

        public void Launch(IClassicDesktopStyleApplicationLifetime desktop)
        {
            var Number = 0;

            // TODO: Disabled ApplicationDeployment since ClickOnce doesnt work in dotnet core.
            /*
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                deployment = ApplicationDeployment.CurrentDeployment;
                deployment.UpdateCompleted += new System.ComponentModel.AsyncCompletedEventHandler(deployment_UpdateCompleted);
                deployment.UpdateProgressChanged += deployment_UpdateProgressChanged;
                deployment.CheckForUpdateCompleted += deployment_CheckForUpdateCompleted;
                try
                {
                    deployment.CheckForUpdateAsync();
                }
                catch (InvalidOperationException e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }*/

            var dummyWindow = new Window();

            foreach (var s in dummyWindow.Screens.All)
            {
                var m = new MainWindow
                {
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    Position = s.WorkingArea.Position,
                    Width = s.WorkingArea.Width,
                    Height = s.WorkingArea.Height,
                    WindowState = WindowState.FullScreen,
                    SystemDecorations = SystemDecorations.None,
                    Topmost = true,
                    Background = (Settings.Default.TransparentBackground
                        ? Brushes.Transparent
                        : Brushes.WhiteSmoke),
                    // Name = "Window" + Number++,
                    Controller = this,
                    DataContext = this
                };
                var windowName = $"Window{Number++}";
                m.Classes.Add(windowName);

                figuresUserControlQueue[windowName] = new List<UserControl>();

                if (windows.Count == 0)
                {
                    desktop.MainWindow = m;
                }

                m.Show();

                m.PointerPressed += HandleMouseLeftButtonDown;
                m.PointerWheelChanged += HandleMouseWheel;

                windows.Add(m);
            }

            dummyWindow.Close();

            //Only show the info label on the FIRST monitor.
            windows[0].infoLabel.IsVisible = true;

            //Startup sound
            Win32Audio.PlayWavResourceYield("EditedJackPlaysBabySmash.wav");

            string[] args = Environment.GetCommandLineArgs();
            var ext = Path.GetExtension(Assembly.GetExecutingAssembly().CodeBase);

            // TODO: Disabled ApplicationDeployment since ClickOnce doesnt work in dotnet core.
            /*if (ApplicationDeployment.IsNetworkDeployed && (ApplicationDeployment.CurrentDeployment.IsFirstRun || ApplicationDeployment.CurrentDeployment.UpdatedVersion != ApplicationDeployment.CurrentDeployment.CurrentVersion))
            {
                //if someone made us a screensaver, then don't show the options dialog.
                if ((args != null && args[0] != "/s") && String.CompareOrdinal(ext, ".SCR") != 0)
                {
                    ShowOptionsDialog();
                }
            }*/
        }

        public void ProcessChar(Control uie, char inChar)
        {
            AddFigure(uie, inChar);
        }

        private void AddFigure(Control uie, char c)
        {
            var template = FigureGenerator.GenerateFigureTemplate(c);
            foreach (var window in windows)
            {
                var figure = FigureGenerator.NewUserControlFrom(template);
                AnimationHelpers.ApplyRandomAnimationEffect(figure);

                figure.Classes.Add("shapes");

                window.AddFigure(figure);

                var queue = figuresUserControlQueue[window.Classes.First()];
                queue.Add(figure);

                // Letters should already have accurate width and height, but others may them assigned.
                if (double.IsNaN(figure.Width) || double.IsNaN(figure.Height))
                {
                    figure.Width = 300;
                    figure.Height = 300;
                }

                figure.ClipToBounds = false;

                Canvas.SetLeft(figure,
                    Utils.RandomBetweenTwoNumbers(0, Convert.ToInt32(window.Bounds.Width - figure.Width)));
                Canvas.SetTop(figure,
                    Utils.RandomBetweenTwoNumbers(0, Convert.ToInt32(window.Bounds.Height - figure.Height)));

                if (Settings.Default.FadeAway)
                {
                    var fadeOut = new Avalonia.Animation.Animation
                    {
                        Delay = TimeSpan.FromSeconds(Settings.Default.FadeAfter),
                        Duration = TimeSpan.FromSeconds(2),
                        Children =
                        {
                            new KeyFrame
                            {
                                Cue = new Cue(0),
                                Setters =
                                {
                                    new Setter(Visual.OpacityProperty, 1d)
                                }
                            },
                            new KeyFrame
                            {
                                Cue = new Cue(1),
                                Setters =
                                {
                                    new Setter(Visual.OpacityProperty, 0d)
                                }
                            }
                        }
                    };
                    fadeOut.RunAsync(figure, null);
                }

                if (figure is IHasFace face)
                {
                    face.IsFaceVisible = Settings.Default.FacesOnShapes;
                }

                if (queue.Count <= Settings.Default.ClearAfter) continue;

                window.RemoveFigure(queue[0]);
                queue.RemoveAt(0);
            }

            // Find the last word typed, if applicable.
            var lastWord = wordFinder.LastWord(figuresUserControlQueue.Values.First());
            if (!string.IsNullOrEmpty(lastWord))
            {
                foreach (var window in windows)
                {
                    wordFinder.AnimateLettersIntoWord(figuresUserControlQueue[window.Classes.First()], lastWord);
                }

                SpeakString(lastWord);
            }
            else
            {
                PlaySound(template);
            }
        }

        void HandleMouseWheel(object sender, PointerWheelEventArgs e)
        {
            if (sender is not UserControl foo || !foo.Classes.Contains("shape")) return;
            if (e.Delta.Y < 0)
            {
                 AnimationHelpers.ApplyZoom(foo, TimeSpan.FromSeconds(0.5), 2.5);
            }
        }

        void HandleMouseLeftButtonDown(object sender, PointerPressedEventArgs e)
        {
            var f = e.Source as UserControl;
            if (f != null && f.Opacity > 0.1) //can it be seen? 
            {
                isDrawing = true; //HACK: This is a cheat to stop the mouse draw action.
                AnimationHelpers.ApplyRandomAnimationEffect(f);
                PlayLaughter(); //Might be better to re-speak the name, color, etc.
            }
        }

        public void PlaySound(FigureTemplate template)
        {
            if (Settings.Default.Sounds == "Laughter")
            {
                PlayLaughter();
            }

            // TODO: Disabled Speech features since it's not cross platform compatible as well.
            /*
            if (objSpeech != null && Settings.Default.Sounds == "Speech")
            {
                if (template.Letter != null && template.Letter.Length == 1 && Char.IsLetterOrDigit(template.Letter[0]))
                {
                    SpeakString(template.Letter);
                }
                else
                {
                    SpeakString(GetLocalizedString(Utils.ColorToString(template.Color)) + " " + template.Name);
                }
            }*/
        }

        /// <summary>
        /// Returns <param name="key"></param> if value or culture is not found.
        /// </summary>
        public static string GetLocalizedString(string key)
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>()!;
            var path = new Uri($@"avares://BabySmash/Resources/Strings/{CultureInfo.CurrentCulture}.json");
            var path2 = new Uri(@"avares://BabySmash/Resources/Strings/en-EN.json");

            var jsonConfig = string.Empty;

            if (assets.Exists(path))
            {
                using var s = new StreamReader(assets.Open(path));
                jsonConfig = s.ReadToEnd();
            }
            else if (assets.Exists(path2))
            {
                using var s = new StreamReader(assets.Open(path2));
                jsonConfig = s.ReadToEnd();
            }

            if (!string.IsNullOrEmpty(jsonConfig))
            {
                var config =
                    JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonConfig);
                if (config.ContainsKey(key))
                {
                    return config[key].ToString();
                }
            }
            else
            {
                Debug.Assert(false, "No file");
            }

            return key;
        }

        private void PlayLaughter()
        {
            Win32Audio.PlayWavResource(Utils.GetRandomSoundFile());
        }

        private void SpeakString(string s)
        {
            // TODO: Disabled Speech features since it's not cross platform compatible as well.
            /*
            ThreadedSpeak ts = new ThreadedSpeak(s);
            ts.Speak();
            */
        }

        // TODO: Disabled Speech features since it's not cross platform compatible as well.
        /*
        private class ThreadedSpeak
        {
            private string Word = null;
            SpeechSynthesizer SpeechSynth = new SpeechSynthesizer();

            public ThreadedSpeak(string Word)
            {
                this.Word = Word;
                CultureInfo keyboardLanguage = Avalonia.Forms.InputLanguage.CurrentInputLanguage.Culture;
                InstalledVoice neededVoice = this.SpeechSynth.GetInstalledVoices(keyboardLanguage).FirstOrDefault();
                if (neededVoice == null)
                {
                    //http://superuser.com/questions/590779/how-to-install-more-voices-to-windows-speech
                    //https://msdn.microsoft.com/en-us/library/windows.media.speechsynthesis.speechsynthesizer.voice.aspx
                    //http://stackoverflow.com/questions/34776593/speechsynthesizer-selectvoice-fails-with-no-matching-voice-is-installed-or-th
                    this.Word = "Unsupported Language";
                }
                else if (!neededVoice.Enabled)
                {
                    this.Word = "Voice Disabled";
                }
                else
                {
                    try
                    {
                        this.SpeechSynth.SelectVoice(neededVoice.VoiceInfo.Name);
                    }
                    catch (Exception ex)
                    {
                        Debug.Assert(false, ex.ToString());
                    }
                }

                SpeechSynth.Rate = -1;
                SpeechSynth.Volume = 100;
            }

            public void Speak()
            {
                Thread oThread = new Thread(new ThreadStart(this.Start));
                oThread.Start();
            }

            private void Start()
            {
                try
                {
                    SpeechSynth.Speak(Word);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.ToString());
                }
            }
        }*/

        public async void ShowOptionsDialog()
        {
            var foo = Settings.Default.TransparentBackground;
            isOptionsDialogShown = true;
            var o = new Options();

            foreach (var m in windows)
            {
                m.Topmost = false;
            }

            o.Topmost = true;
            await o.ShowDialog(windows[0]);

            foreach (var m in windows)
            {
                m.Topmost = true;
                //m.ResetCanvas();
            }

            isOptionsDialogShown = false;
        }
        //
        // public void MouseDown(MainWindow main, MouseButtonEventArgs e)
        // {
        //     if (isDrawing || Settings.Default.MouseDraw) return;
        //
        //     // Create a new Ellipse object and add it to canvas.
        //     Point ptCenter = e.GetPosition(main.mouseCursorCanvas);
        //     MouseDraw(main, ptCenter);
        //     isDrawing = true;
        //     main.CaptureMouse();
        //
        //     Win32Audio.PlayWavResource("smallbumblebee.wav");
        // }
        //
        public void MouseWheel(MainWindow main, PointerWheelEventArgs e)
        {
            if (e.Delta.Y > 0)
            {
                Win32Audio.PlayWavResourceYield("rising.wav");
            }
            else
            {
                Win32Audio.PlayWavResourceYield("falling.wav");
            }
        }
        //
        // public void MouseUp(MainWindow main, MouseButtonEventArgs e)
        // {
        //     isDrawing = false;
        //     if (Settings.Default.MouseDraw) return;
        //     main.ReleaseMouseCapture();
        // }
        //
        // public void MouseMove(MainWindow main, MouseEventArgs e)
        // {
        //     if (isOptionsDialogShown)
        //     {
        //         main.ReleaseMouseCapture();
        //         return;
        //     }
        //
        //     if (Settings.Default.MouseDraw && main.IsMouseCaptured == false)
        //         main.CaptureMouse();
        //
        //     if (isDrawing || Settings.Default.MouseDraw)
        //     {
        //         MouseDraw(main, e.GetPosition(main));
        //     }
        //
        //     // Cheesy, but hotkeys are ignored when the mouse is captured.
        //     // However, if we don't capture and release, the shapes will draw forever.
        //     if (Settings.Default.MouseDraw && main.IsMouseCaptured)
        //         main.ReleaseMouseCapture();
        // }
        //
        // private void MouseDraw(MainWindow main, Point p)
        // {
        //     //randomize at some point?
        //     Shape shape = new Ellipse
        //     {
        //         Stroke = Theme,
        //         StrokeThickness = 0,
        //         Fill = Utils.GetGradientBrush(Utils.GetRandomColor()),
        //         Width = 50,
        //         Height = 50
        //     };
        //
        //     ellipsesQueue.Enqueue(shape);
        //     main.mouseDragCanvas.Children.Add(shape);
        //     Canvas.SetLeft(shape, p.X - 25);
        //     Canvas.SetTop(shape, p.Y - 25);
        //
        //     if (Settings.Default.MouseDraw)
        //         Win32Audio.PlayWavResourceYield("smallbumblebee.wav");
        //
        //     if (ellipsesQueue.Count > 30) //this is arbitrary
        //     {
        //         Shape shapeToRemove = ellipsesQueue.Dequeue();
        //         main.mouseDragCanvas.Children.Remove(shapeToRemove);
        //     }
        // }
        //
        // //private static void ResetCanvas(MainWindow main)
        // //{
        // //   main.ResetCanvas();
        // //}
        //
        // public void LostMouseCapture(MainWindow main, MouseEventArgs e)
        // {
        //     if (Settings.Default.MouseDraw) return;
        //     if (isDrawing) isDrawing = false;
        // }
    }
}