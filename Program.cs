﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

namespace Fool;

class Program
{

    public static int PlayersAmount = 3;
    public static bool ShowHelp = false;
    public static bool ShowRules = false;
    public static bool DecreaseInput = true;
    public static int TextDelay = 0;
    public static int TextDelayInterval = 1;

    public static string PlayerName = "";
    public static bool SetName = false;

    public static Card.SortType sortType = Card.SortType.Rank;

    public static string SettingsLocation = @"%USERPROFILE%/AppData/Local/Dzhake/Fool/settings.txt";
    public static Dictionary<string, string> Settings = new Dictionary<string, string>()
    {
        {"wasRun", "false"},
        {"name","Dzhake" },
    };
    public static bool DefaultSettings = false;

    public enum Modifier
    {
        Wildcards
    }

    public static Dictionary<Modifier, bool> Modifiers = new Dictionary<Modifier, bool>()
    {
        {Modifier.Wildcards, false},
    };


    static void Main(string[] args)
    {
        using var game = new Game1(PlayersAmount);
        game.Run();
        return;

        SettingsLocation = Environment.ExpandEnvironmentVariables(SettingsLocation);

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--seed" && args.Length > (i + 1))
            {
                Utils.random = new Random(int.Parse(args[i + 1]));
                i++;
            }
            else if (args[i] == "--players-amount" && args.Length > (i + 1))
            {
                PlayersAmount = int.Parse(args[i + 1]);
                i++;
                if (PlayersAmount * 6 > 52)
                {
                    ColorConsole.WriteLine($"<{PlayersAmount}> is higher than maximum allowed number of players, because whole deck is smaller than cards which should be given at start.");
                    Environment.Exit(0);
                }
            }
            else if (args[i] == "--help")
            {
                ShowHelp = true;
            }
            else if (args[i] == "--rules")
            {
                ShowRules = true;
            }
            else if (args[i] == "--default-settings")
            {
                DefaultSettings = true;
            }
            else if (args[i] == "--no-decrease")
            {
                DecreaseInput = false;
            }
            else if (args[i] == "--text-delay" && args.Length > (i + 1))
            {
                TextDelay = int.Parse(args[i + 1]);
                i++;
            }
            else if (args[i] == "--text-delay-interval" && args.Length > (i + 1))
            {
                TextDelayInterval = int.Parse(args[i + 1]);
                i++;
            }
            else if (args[i] == "--candy")
            {
                ColorConsole.WriteLine("        .---.\r\n       |   '.|  __\r\n       | ___.--'  )\r\n     _.-'_` _%%%_/\r\n  .-'%%% a: a %%%\r\n      %%  L   %%_\r\n      _%\\'-' |  /-.__\r\n   .-' / )--' #/     '\\\r\n  /'  /  /---'(    :   \\\r\n /   |  /( /|##|  \\     |\r\n/   ||# | / | /|   \\    \\\r\n|   ||##| I \\/ |   |   _|\r\n|   ||: | o  |#|   |  / |\r\n|   ||  / I  |:/  /   |/\r\n|   ||  | o   /  /    /\r\n|   \\|  | I  |. /    /\r\n \\  /|##| o  |.|    /\r\n  \\/ \\::|/\\_ /  ---'|");
            }
            else if (args[i] == "--name" && args.Length > (i + 1))
            {
                PlayerName = args[i + 1];
                i++;
            }
            else if (args[i] == "--set-name" && args.Length > (i + 1))
            {
                PlayerName = args[i + 1];
                SetName = true;
                i++;
            }
            else if (args[i] == "--sort-type" && args.Length > (i + 1))
            {
                sortType = (Card.SortType)int.Parse(args[i + 1]);
                i++;
            }
            else if ((args[i] == "--modifier" || args[i] == "-m") && args.Length > (i + 1))
            {
                Modifier m = (Modifier)Enum.Parse(typeof(Modifier), args[i + 1]);
                Modifiers[m] = true;
                i++;
            }
            else
            {
                ColorConsole.WriteLine($"Unknown flag {{#red}}{args[i]}{{#}}!");
            }
        }

            

        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8; // it supports funny characters which allow to draw ascii art

        if (File.Exists(SettingsLocation) && !DefaultSettings)
        {
            foreach (string line in File.ReadAllLines(SettingsLocation))
            {
                string[] kv = line.Split(',');
                if (kv.Length > 1)
                    Settings[kv[0]] = kv[1];
            }
            //ColorConsole.WriteLine("{#green}Settings loaded successfully!{#}");
        }
        else
        {
            SaveSettings(true, true);
        }

        if (SetName)
        {
            Settings["name"] = PlayerName;
            SaveSettings(false);
        }
        else if (PlayerName == "")
        {
            PlayerName = Settings["name"];
        }

        ShowHelp = ShowHelp || !bool.Parse(Settings["wasRun"]);

        bool play = true;

        if (ShowHelp)
        {
            ColorConsole.WriteLine("Welcome ʕっ•ᴥ•ʔっ! This is card game \"Fool\".");
            ColorConsole.WriteLine("IMPORTANT! {#red}This text should be red{#}. If instead of seeing colors here you see a lot of weird symbols, use other terminal! Don't forget to use --help there.");
            ColorConsole.WriteLine("Arguments (ex. \"./Fool.exe --rules\", to run game with \"--rules\" argument, separate arguments by spaces to use more than one) list:");
            ColorConsole.WriteLine("--rules - see rules (explained pretty badly :c, use google instead)");
            ColorConsole.WriteLine("--help - shows this message");
            ColorConsole.WriteLine("--default-settings - do not load settings, instead load default and save");
            ColorConsole.WriteLine("--name X or --set-name X - Changes your name for current run, or saves it to settings");
            ColorConsole.WriteLine("--sort-type X - Changes how cards for current run are sorted, 1 is by rank and 2 is by suit.");
            ColorConsole.WriteLine("--players-amount X - Changes players amount, default value is 3. (tbh not really recommended to use)");
            ColorConsole.WriteLine("--seed X - Sets random seed");
            ColorConsole.WriteLine("--text-delay X - Text is split by characters and show after delay, useful if you're confused when too many text appears at once.");
            ColorConsole.WriteLine("--text-delay-interval X - Text delay must be int, but if it's 1, then output is pretty slow, so I made this. Each --text-delay-interval charcaters --text-delay is applied.");
            ColorConsole.WriteLine("--no-decrease - By default 1 is decreased from each input, so actually -1 is doing nothing, and picking card starts from 0. You can use it, if you want.");
            ColorConsole.WriteLine("{#000000}There are also some easter eggs... or candies.. doesn't matter..{#}");


            Console.WriteLine();
            ColorConsole.WriteLine("This message is shown once, later you can use --help to see it again, or --default-settings.");

            Settings["wasRun"] = "true";
            SaveSettings(false, false);
            play = false;
        }

        if (ShowRules)
        {
            ColorConsole.WriteLine("Rules are simple:\nEverybody gets 6 <cards>. After one, next card in deck becomes trump card. It's suit is trump suit, which beats all other suits. Trump card goes to the bottom of deck, visible for everyone.");
            ColorConsole.WriteLine("Then, someone <attacks>. <Defender> needs to use a <card> of same suit and higher value or trump.");
            ColorConsole.WriteLine("Or he can take all <cards> from the table, if he can't or doesn't want to <defend>.");
            ColorConsole.WriteLine("If he <defended>, everybody can add <cards> to the table, but only <cards> of same value as already on the table.");
            ColorConsole.WriteLine("<Defender> needs to <defend> from each added <card> too, or take all <cards>.");
            ColorConsole.WriteLine("Total <attacking> <cards> amount can't be higher than the amount of <cards> the <defender> had at the start of turn, because otherwise he can't beat all of them.");
            ColorConsole.WriteLine("When turns ends, everybody takes up to 6 <cards>. If the deck is empty, then people stop takings <cards>, and next turn starts.");
            ColorConsole.WriteLine("First who loses all his <cards>, while deck is empty, wins.");
            ColorConsole.WriteLine("Usually the game doesn't end when someone wins. Instead people play until someone stays, and everybody else wins. Person who lost is called \"Fool\". (But in this game only one winner {-}because I'm lazy{-})");
            play = false;
        }

        if (play)
        {
                
            //Game.Run(PlayersAmount);
        }
    }

    public static void SaveSettings(bool notify = true, bool create = false)
    {
        string text = "";
        foreach (KeyValuePair<string, string> kvp in Settings)
        {
            text += $"{kvp.Key},{kvp.Value}\n";
        }

        FileInfo file = new FileInfo(SettingsLocation);
        if (create)
        {
            file.Directory?.Create();
        }
        File.WriteAllText(SettingsLocation, text);
        if (notify)
        {
            ColorConsole.WriteLine($"{{#green}}Settings {(create ? "created" : "saved")} successfully!{{#}}");
        }
    }

}