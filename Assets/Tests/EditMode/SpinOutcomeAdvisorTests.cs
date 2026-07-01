using NUnit.Framework;
using SlotDefense;

public class SpinOutcomeAdvisorTests
{
    [Test]
    public void Describe_TripleFire_ShowsStrategicChangeAndNextAttackDecision()
    {
        var advice = SpinOutcomeAdvisor.Describe(
            new[] { ElementType.Fire, ElementType.Fire, ElementType.Fire },
            (6, 0, 0));

        Assert.AreEqual("JACKPOT - FIRE x6", advice.Headline);
        Assert.AreEqual("변환 화염 에너지가 크게 올라 광역 공격 선택지가 강해졌습니다.", advice.Change);
        Assert.AreEqual("공격 선택: 다음 웨이브에서 몰려드는 적에게 화염 스킬을 먼저 쓰세요.", advice.NextDecision);
        Assert.AreEqual(SpinOutcomeTone.Jackpot, advice.Tone);
    }

    [Test]
    public void Describe_DoubleIron_ShowsDefenseDecisionBecomesSafer()
    {
        var advice = SpinOutcomeAdvisor.Describe(
            new[] { ElementType.Iron, ElementType.Iron, ElementType.Life },
            (0, 3, 1));

        Assert.AreEqual("IRON PAIR - IRON x3 / LIFE x1", advice.Headline);
        Assert.AreEqual("변환 철 에너지가 쌓여 방어형 카드 사용이 안정적입니다.", advice.Change);
        Assert.AreEqual("방어 선택: 마을 체력이 흔들리면 방어 카드와 전열 보강을 우선하세요.", advice.NextDecision);
        Assert.AreEqual(SpinOutcomeTone.Strong, advice.Tone);
    }

    [Test]
    public void Describe_AllDifferent_ShowsLowRollRiskAndRecoveryDecision()
    {
        var advice = SpinOutcomeAdvisor.Describe(
            new[] { ElementType.Fire, ElementType.Iron, ElementType.Life },
            (1, 1, 1));

        Assert.AreEqual("MIXED ROLL - FIRE x1 / IRON x1 / LIFE x1", advice.Headline);
        Assert.AreEqual("변환 에너지가 고르게 쌓였지만 즉시 폭발력은 없습니다.", advice.Change);
        Assert.AreEqual("위험 관리: 공격/방어를 크게 바꾸기보다 부족한 속성을 보완하세요.", advice.NextDecision);
        Assert.AreEqual(SpinOutcomeTone.Risky, advice.Tone);
    }

    [Test]
    public void Describe_DoubleLife_ShowsRecoveryDecisionInsteadOfAttackDecision()
    {
        var advice = SpinOutcomeAdvisor.Describe(
            new[] { ElementType.Life, ElementType.Fire, ElementType.Life },
            (1, 0, 4));

        Assert.AreEqual("LIFE PAIR - FIRE x1 / LIFE x4", advice.Headline);
        Assert.AreEqual("변환 생명 에너지가 쌓여 회복과 유지 선택이 안전해졌습니다.", advice.Change);
        Assert.AreEqual("회복 선택: 피해 누적 전 회복 카드로 전선을 유지하세요.", advice.NextDecision);
        Assert.AreEqual(SpinOutcomeTone.Strong, advice.Tone);
    }
}