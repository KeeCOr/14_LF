using NUnit.Framework;
using SlotDefense;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineUILayoutTests
{
    [Test]
    public void ReelStripViewport_FillsGeneratedFrameWindow()
    {
        var root = new GameObject("SlotMachineUITestRoot", typeof(RectTransform));
        try
        {
            var ui = root.AddComponent<SlotMachineUI>();
            ui.reelLabels = new Text[3];
            ui.reelIcons = new Image[3];
            ui.spinButton = MakeButton(root.transform);

            for (int i = 0; i < 3; i++)
                BuildReel(root.transform, ui, i);

            root.SendMessage("Start");

            var viewport = (RectTransform)ui.reelIcons[0].transform.parent.Find("ReelViewport");
            var symbol = (RectTransform)viewport.Find("ReelStrip/Symbol_4");

            Assert.GreaterOrEqual(viewport.sizeDelta.x, 108f);
            Assert.GreaterOrEqual(viewport.sizeDelta.y, 80f);
            Assert.GreaterOrEqual(symbol.sizeDelta.x, 60f);
            Assert.GreaterOrEqual(symbol.sizeDelta.y, 60f);
        }
        finally
        {
            Object.DestroyImmediate(root);
        }
    }

    private static void BuildReel(Transform root, SlotMachineUI ui, int index)
    {
        var reel = new GameObject($"Reel{index}", typeof(RectTransform), typeof(Image));
        reel.transform.SetParent(root, false);

        var label = new GameObject("Value", typeof(RectTransform), typeof(Text));
        label.transform.SetParent(reel.transform, false);
        ui.reelLabels[index] = label.GetComponent<Text>();

        var icon = new GameObject("Icon", typeof(RectTransform), typeof(Image));
        icon.transform.SetParent(reel.transform, false);
        ui.reelIcons[index] = icon.GetComponent<Image>();
    }

    private static Button MakeButton(Transform root)
    {
        var button = new GameObject("SpinButton", typeof(RectTransform), typeof(Image), typeof(Button));
        button.transform.SetParent(root, false);
        return button.GetComponent<Button>();
    }
}
