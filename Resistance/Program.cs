using BusinessLogic;
using Microsoft.Extensions.Configuration;
using Parsing;
using System.Speech.Synthesis;

var roleAssignmentGenerator = new RoleAssignmentGenerator();

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var parser = new Parser(config);

//var roleAssignments = roleAssignmentGenerator.GenerateRoleAssignments(
//    players,
//    gameMode);

var speech = new SpeechSynthesizer();

speech.SpeakAsync("Everyone put your hands in the middle and go to sleep.");

await Task.Delay(2000);

speech.SpeakAsync("Spies, wake up and put your thumbs up.");

// TODO literally everything TBH

