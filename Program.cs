using System;
using System.Timers;
using SpotifyApp;
using System.Threading;

namespace Pomodoro
{
    class Program
    {
        static CancellationTokenSource s_cts = new CancellationTokenSource();
        static CancellationToken token = s_cts.Token;
        static bool enableSpotify = false;
        static bool enableHomeAssistant = false;
        static Player _spotify = null;
        static HomeAssistant _homeAssistant = null;
        static System.Timers.Timer pomodoro = new System.Timers.Timer();
        static void Main(string[] args)
        {
            int elapsedCycles = 0;
            int cycles;
            bool onBreak = false;
            int miniCycles = 0;

            StartUp();

            try
            {
                InitiateTimer();
                // Keep the console app running.
                while (Console.ReadLine() != "q")
                {

                }
                Console.WriteLine("shutting down...");
                s_cts.Cancel();
            }
            catch (Exception e)
            {
                Console.WriteLine("caught exception " + e);
            }
            void InitiateTimer()
            {
                int interval = 1500000;
                cycles = GetCycles();
                Console.WriteLine("Lets run for: " + cycles + " cycles.\nStarting now! First break at " + DateTime.Now.AddMilliseconds(interval).ToString("h:mm tt"));
                pomodoro.Interval = interval;
                pomodoro.Elapsed += new ElapsedEventHandler(ResetPomodoro);
                pomodoro.Start();
                if (enableSpotify) _spotify.InvokeSpotify(Commands.PLAY);
                if (enableHomeAssistant) _homeAssistant.HomeAssistantPost(HomeAssistant.Commands.START);
            }
            void ResetPomodoro(Object source, ElapsedEventArgs e)
            {
                // Determine what should happen when the timer ends.
                // If we're on a break, start a new work cycle.
                // If the work cycle finishes, take a break or check if we are done.
                if (!onBreak)
                {
                    TakeBreakOrFinish();
                }
                else
                {
                    BreakOver();
                }
            }
            void TakeBreakOrFinish()
            {
                elapsedCycles++;
                miniCycles++;
                if (enableSpotify) _spotify.InvokeSpotify(Commands.PAUSE);
                if (enableHomeAssistant) _homeAssistant.HomeAssistantPost(HomeAssistant.Commands.PAUSE);
                for (int i = 0; i < miniCycles; i++)
                {
                    DrawArt("break");
                }
                if (elapsedCycles < cycles)
                {
                    TakeBreak();
                }
                else
                {
                    FinishTimer();
                }
            }
            void BreakOver()
            {
                int interval = 1500000;
                if (enableSpotify) _spotify.InvokeSpotify(Commands.PLAY);
                if (enableHomeAssistant) _homeAssistant.HomeAssistantPost(HomeAssistant.Commands.START);
                Console.WriteLine("Break's over! Back to work. Next break at " + DateTime.Now.AddMilliseconds(interval).ToString("h:mm tt"));
                pomodoro.Interval = interval;
                pomodoro.Start();
                onBreak = false;
            }
            void FinishTimer()
            {
                pomodoro.Stop();
                Console.WriteLine("All cycles complete!");
                Console.WriteLine("press q to exit");
            }
            void TakeBreak()
            {
                // If we've done 4 continuous cycles, take a long break.
                if (miniCycles % 4 == 0)
                {
                    int interval = 1800000;
                    pomodoro.Interval = interval;
                    miniCycles = 0;
                    Console.WriteLine("You've done a full pomodoro! Time for a long break. You've earned it! See you at " + DateTime.Now.AddMilliseconds(interval).ToString("h:mm tt"));
                }
                else
                {
                    int interval = 300000;
                    Console.WriteLine("Time for a break. Cycles left: " + (cycles - elapsedCycles).ToString() + ". See you at " + DateTime.Now.AddMilliseconds(interval).ToString("h:mm tt"));
                    pomodoro.Interval = interval;
                }
                onBreak = true;
                pomodoro.Start();
            }
        }

        static void StartUp()
        {
            Console.WriteLine("Welcome to Torban's Pomodoro Timer! Let's get to work!");
            Console.WriteLine("Would you like to enable Spotify Integration? (Requires spotify account + Spotify developer account) y/n");
            string spotifyText = Console.ReadLine().ToUpper();
            if (spotifyText == "Y" || spotifyText == "YES")
            {
                enableSpotify = true;
            }
            Console.WriteLine("Would you like to enable Home Assistant Integration? (requires configured Home Assistant running) y/n");
            string haText = Console.ReadLine().ToUpper();
            if (spotifyText == "Y" || spotifyText == "YES")
            {
                enableHomeAssistant = true;
            }
            // Setup helper objects.

            if (enableSpotify)
            {
                _spotify = new Player();
                _spotify.StartPlayer(_spotify);
            }

            if (enableHomeAssistant) _homeAssistant = new HomeAssistant();
        }

        static int GetCycles()
        {
            int cycles = -1;
            Console.WriteLine("(1 cycle = four 25 minute work sessions with 5 minute breaks. 30 minutes each )");
            while (cycles <= 0)
            {
                Console.WriteLine("How many Pomodoro cycles would you like to run?");
                string input = Console.ReadLine();
                // Validate the input is an int greater than 0.
                if (!int.TryParse(input, out cycles) || cycles == 0)
                {
                    Console.WriteLine("invalid input");
                }
            }
            return cycles;
        }
        static void DrawArt(string tag)
        {
            // ASCII art.
            string[] breakArt = new string[]{"     ,--./,-.",
                                         "    / #      \\",
                                         "   |          |",
                                         "    \\        /       ",
                                         "     `._,._,'"};

            if (tag == "break")
            {
                foreach (string s in breakArt)
                {
                    Console.WriteLine(s);
                }
            }
        }
    }
}
