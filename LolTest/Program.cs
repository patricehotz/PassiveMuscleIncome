using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.LolStatusV3;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;

[DllImport("Kernel32.dll")]
static extern IntPtr GetConsoleWindow();
[DllImport("User32.dll")]
static extern bool ShowWindow(IntPtr hWnd, int cmdShow);

IntPtr hWnd = GetConsoleWindow();
if (hWnd != IntPtr.Zero)
{
    ShowWindow(hWnd, 0);//hide
}

var LolCheckIntervall = new System.Timers.Timer();
var CheckIfLoseIntervall = new System.Timers.Timer();

LolCheckIntervall.Elapsed += new ElapsedEventHandler(CheckIfLolIsStarted);
LolCheckIntervall.Interval = 10000;
LolCheckIntervall.Enabled = true;

CheckIfLoseIntervall.Elapsed += new ElapsedEventHandler(CheckIfLose);
CheckIfLoseIntervall.Interval = 5000;
CheckIfLoseIntervall.Enabled = false;

void CheckIfLolIsStarted(object source, ElapsedEventArgs e)
{
    CheckIfLoseIntervall.Enabled = Process.GetProcessesByName("LeagueClient").Length > 0;
}

async void CheckIfLose(object source, ElapsedEventArgs e)
{
    var saveFile = @"save.txt";
    var logFile = @"log.txt";
    var apiKey = "RGAPI-6df4a86d-bf96-4650-9da5-9b12d25dd5ec";

    try
    {
        var cool = Process.GetProcessesByName("LeagueClient").Length > 0;

        var riotApi = RiotApi.NewInstance(apiKey);

        var puuid = (await riotApi.SummonerV4.GetBySummonerNameAsync(Region.EUW, "CakeMaster6969")).Puuid;
        var matchId = (await riotApi.MatchV5.GetMatchIdsByPUUIDAsync(Region.Europe, puuid, 2, 1))[0];

        if (File.Exists(saveFile) && File.ReadAllText(saveFile) == matchId)
        {
            return;
        }

        File.WriteAllText(saveFile, matchId);

        var matchDetails = await riotApi.MatchV5.GetMatchAsync(Region.Europe, matchId);

        var participants = (JArray)JObject.Parse(matchDetails._AdditionalProperties["info"].ToString())["participants"];

        foreach (var participant in participants)
        {
            if ((string)participant["puuid"] == puuid && !(bool)participant["win"])
            {
                MessageBox.Show("Do some Exercise");
            }
        };

    }
    catch (Exception ee)
    {
        File.WriteAllText(logFile, ee.Message);
    }
}
Console.ReadKey();