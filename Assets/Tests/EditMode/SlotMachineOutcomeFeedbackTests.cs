using System.IO;
using NUnit.Framework;

public class SlotMachineOutcomeFeedbackTests
{
    [Test]
    public void SlotMachineUI_UsesSpinOutcomeAdvisorForResultCopy()
    {
        string path = Path.Combine(UnityEngine.Application.dataPath, "Scripts", "UI", "SlotMachineUI.cs");
        string source = File.ReadAllText(path);

        StringAssert.Contains("SpinOutcomeAdvisor.Describe", source);
        StringAssert.Contains("advice.Headline", source);
        StringAssert.Contains("advice.Change", source);
        StringAssert.Contains("advice.NextDecision", source);
        StringAssert.Contains("SpinOutcomeTone.Jackpot", source);
    }
}
