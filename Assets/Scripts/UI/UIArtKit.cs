using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SlotDefense
{
    public enum UIArtSheet
    {
        Buttons,
        Frames,
        CardIcons,
        UiAtlas,
        BattlefieldProps
    }

    public enum UIArtSprite
    {
        RedButton,
        BlueButton,
        GreenButton,
        GoldButton,
        DarkButton,
        BlueSquareButton,
        DarkSquareButton,
        RedCircleButton,
        ToggleOn,
        ToggleOff,
        BlueTab,
        RedTab,
        SlotMachineFrame,
        CardTrayFrame,
        TitleBannerFrame,
        PlayerHpFrame,
        EnemyHpFrame,
        ModalPanelFrame,
        ResourceCounterFrame,
        TooltipFrame,
        PortraitFrame,
        SquareIconFrame,
        CardFrameBronze,
        CardFrameSilver,
        CardFrameGold,
        CardFramePurple,
        FireSymbol,
        IronSymbol,
        LifeSymbol,
        LuckOrb,
        GoldCoin,
        Chest,
        CardBack,
        BlueShield,
        RedShield,
        PlayerTower,
        EnemyTower,
        PortalGate,
        StoneWall,
        Campfire,
        WoodenFence,
        PineTree,
        FlowerBush,
        RockCluster,
        GroundPatch,
        CrownPlaque
    }

    public static class UIArtKit
    {
        private const string ResourcePrefix = "GeneratedArt/";
        private static readonly Dictionary<UIArtSheet, Texture2D> Sheets = new Dictionary<UIArtSheet, Texture2D>();
        private static readonly Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();

        public static Texture2D LoadSheet(UIArtSheet sheet)
        {
            Texture2D texture;
            if (Sheets.TryGetValue(sheet, out texture)) return texture;

            texture = Resources.Load<Texture2D>(ResourcePrefix + SheetPath(sheet));
            Sheets[sheet] = texture;
            return texture;
        }

        public static Sprite Sprite(UIArtSprite spriteId)
        {
            var def = Definition(spriteId);
            return CreateSprite(spriteId.ToString(), def.sheet, def.topRect, def.border);
        }

        public static Sprite CardIcon(string cardName)
        {
            if (string.IsNullOrEmpty(cardName))
                return Sprite(UIArtSprite.CardBack);

            var lower = cardName.ToLowerInvariant();
            if (ContainsAny(lower, "궁수", "폭풍", "archer", "ranger"))
                return CreateCardIcon("ForestRanger", 0, 1);
            if (ContainsAny(lower, "마법", "화염술", "번개", "포탈", "mage", "magic"))
                return CreateCardIcon("ArcaneMage", 2, 1);
            if (ContainsAny(lower, "힐", "생명", "성기사", "행운", "healer", "life", "luck"))
                return CreateCardIcon("LifeCleric", 2, 0);
            if (ContainsAny(lower, "거인", "골렘", "스켈레톤", "기사", "giant", "golem", "knight"))
                return CreateCardIcon("IronGolem", 1, 0);
            if (ContainsAny(lower, "드워프", "폭격", "탑", "제철", "병영", "dwarf", "tower", "forge"))
                return CreateCardIcon("Bombardier", 1, 1);
            if (ContainsAny(lower, "용", "검사", "팔라딘", "fire", "dragon", "sword"))
                return CreateCardIcon("FireKnight", 0, 0);

            return CreateCardIcon("FireKnight", 0, 0);
        }

        public static Sprite CardIcon(CardData card)
        {
            if (card == null) return Sprite(UIArtSprite.CardBack);
            switch (card.cardType)
            {
                case CardType.Skill:
                    if (card.ironCost > 0 && card.fireCost > 0) return Sprite(UIArtSprite.LuckOrb);
                    if (card.lifeCost > 0) return Sprite(UIArtSprite.LifeSymbol);
                    return Sprite(UIArtSprite.FireSymbol);
                case CardType.Building:
                    if (card.cardName != null && card.cardName.Contains("탑")) return Sprite(UIArtSprite.StoneWall);
                    return Sprite(UIArtSprite.Chest);
                case CardType.Buff:
                    return Sprite(UIArtSprite.GoldCoin);
                default:
                    return CardIcon(card.cardName);
            }
        }

        public static Sprite CardFrame(CardType type, int slotIndex)
        {
            switch (type)
            {
                case CardType.Skill:
                    return Sprite(UIArtSprite.CardFramePurple);
                case CardType.Building:
                    return Sprite(UIArtSprite.CardFrameSilver);
                case CardType.Buff:
                    return Sprite(UIArtSprite.CardFrameGold);
                default:
                    switch (Mathf.Abs(slotIndex) % 3)
                    {
                        case 1:
                            return Sprite(UIArtSprite.CardFrameSilver);
                        case 2:
                            return Sprite(UIArtSprite.CardFrameGold);
                        default:
                            return Sprite(UIArtSprite.CardFrameBronze);
                    }
            }
        }

        public static Sprite ElementIcon(ElementType element)
        {
            switch (element)
            {
                case ElementType.Fire:
                    return Sprite(UIArtSprite.FireSymbol);
                case ElementType.Iron:
                    return Sprite(UIArtSprite.IronSymbol);
                case ElementType.Life:
                    return Sprite(UIArtSprite.LifeSymbol);
                default:
                    return Sprite(UIArtSprite.LuckOrb);
            }
        }

        public static void Apply(Image image, UIArtSprite spriteId)
        {
            if (image == null) return;
            var sprite = Sprite(spriteId);
            if (sprite == null) return;
            image.sprite = sprite;
            image.color = Color.white;
            image.type = HasBorder(spriteId) ? Image.Type.Sliced : Image.Type.Simple;
            image.preserveAspect = !HasBorder(spriteId);
        }

        public static void Apply(Button button, UIArtSprite spriteId)
        {
            if (button == null) return;
            var image = button.targetGraphic as Image;
            if (image == null) image = button.GetComponent<Image>();
            Apply(image, spriteId);
        }

        private static Sprite CreateCardIcon(string key, int col, int row)
        {
            var rect = new Rect(512f * col, 512f * row, 512f, 512f);
            return CreateSprite("CardIcon_" + key, UIArtSheet.CardIcons, rect, Vector4.zero, false);
        }

        private static Sprite CreateSprite(string key, UIArtSheet sheet, Rect topRect, Vector4 border, bool fromTop = true)
        {
            var cacheKey = sheet + "_" + key;
            Sprite sprite;
            if (Sprites.TryGetValue(cacheKey, out sprite)) return sprite;

            var texture = LoadSheet(sheet);
            if (texture == null) return null;

            var rect = fromTop
                ? new Rect(topRect.x, texture.height - topRect.y - topRect.height, topRect.width, topRect.height)
                : topRect;
            sprite = UnityEngine.Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, border);
            Sprites[cacheKey] = sprite;
            return sprite;
        }

        private static string SheetPath(UIArtSheet sheet)
        {
            switch (sheet)
            {
                case UIArtSheet.Buttons:
                    return "lotteryfantasy-buttons-sheet";
                case UIArtSheet.Frames:
                    return "lotteryfantasy-frames-sheet";
                case UIArtSheet.CardIcons:
                    return "lotteryfantasy-card-unit-icons-sheet";
                case UIArtSheet.UiAtlas:
                    return "lotteryfantasy-ui-atlas-sheet";
                case UIArtSheet.BattlefieldProps:
                    return "lotteryfantasy-battlefield-props-sheet";
                default:
                    return "lotteryfantasy-ui-atlas-sheet";
            }
        }

        private static ArtDefinition Definition(UIArtSprite spriteId)
        {
            switch (spriteId)
            {
                case UIArtSprite.RedButton:
                    return new ArtDefinition(UIArtSheet.Buttons, new Rect(70, 50, 650, 175), new Vector4(70, 55, 70, 55));
                case UIArtSprite.BlueButton:
                    return new ArtDefinition(UIArtSheet.Buttons, new Rect(815, 50, 640, 175), new Vector4(70, 55, 70, 55));
                case UIArtSprite.GreenButton:
                    return new ArtDefinition(UIArtSheet.Buttons, new Rect(70, 292, 650, 175), new Vector4(70, 55, 70, 55));
                case UIArtSprite.GoldButton:
                    return new ArtDefinition(UIArtSheet.Buttons, new Rect(815, 292, 650, 175), new Vector4(70, 55, 70, 55));
                case UIArtSprite.DarkButton:
                    return new ArtDefinition(UIArtSheet.Buttons, new Rect(70, 550, 650, 170), new Vector4(70, 55, 70, 55));
                case UIArtSprite.BlueSquareButton:
                    return new ArtDefinition(UIArtSheet.Buttons, new Rect(775, 550, 185, 180), new Vector4(42, 42, 42, 42));
                case UIArtSprite.DarkSquareButton:
                    return new ArtDefinition(UIArtSheet.Buttons, new Rect(1000, 550, 185, 180), new Vector4(42, 42, 42, 42));
                case UIArtSprite.RedCircleButton:
                    return new ArtDefinition(UIArtSheet.Buttons, new Rect(1268, 520, 205, 220), Vector4.zero);
                case UIArtSprite.ToggleOn:
                    return new ArtDefinition(UIArtSheet.Buttons, new Rect(72, 790, 315, 120), new Vector4(52, 42, 52, 42));
                case UIArtSprite.ToggleOff:
                    return new ArtDefinition(UIArtSheet.Buttons, new Rect(424, 790, 315, 120), new Vector4(52, 42, 52, 42));
                case UIArtSprite.BlueTab:
                    return new ArtDefinition(UIArtSheet.Buttons, new Rect(780, 825, 315, 120), new Vector4(46, 38, 46, 38));
                case UIArtSprite.RedTab:
                    return new ArtDefinition(UIArtSheet.Buttons, new Rect(1150, 825, 315, 120), new Vector4(46, 38, 46, 38));
                case UIArtSprite.PlayerHpFrame:
                    return new ArtDefinition(UIArtSheet.Frames, new Rect(28, 62, 430, 110), new Vector4(48, 36, 48, 36));
                case UIArtSprite.TitleBannerFrame:
                    return new ArtDefinition(UIArtSheet.Frames, new Rect(500, 20, 520, 135), new Vector4(70, 45, 70, 45));
                case UIArtSprite.EnemyHpFrame:
                    return new ArtDefinition(UIArtSheet.Frames, new Rect(1070, 60, 430, 110), new Vector4(48, 36, 48, 36));
                case UIArtSprite.PortraitFrame:
                    return new ArtDefinition(UIArtSheet.Frames, new Rect(52, 220, 240, 240), new Vector4(50, 50, 50, 50));
                case UIArtSprite.SlotMachineFrame:
                    return new ArtDefinition(UIArtSheet.Frames, new Rect(350, 210, 725, 420), new Vector4(85, 85, 85, 85));
                case UIArtSprite.ResourceCounterFrame:
                    return new ArtDefinition(UIArtSheet.Frames, new Rect(1150, 245, 335, 78), new Vector4(46, 28, 46, 28));
                case UIArtSprite.TooltipFrame:
                    return new ArtDefinition(UIArtSheet.Frames, new Rect(1210, 365, 230, 120), new Vector4(42, 38, 42, 38));
                case UIArtSprite.SquareIconFrame:
                    return new ArtDefinition(UIArtSheet.Frames, new Rect(58, 505, 205, 205), new Vector4(42, 42, 42, 42));
                case UIArtSprite.CardTrayFrame:
                    return new ArtDefinition(UIArtSheet.Frames, new Rect(28, 720, 520, 215), new Vector4(65, 48, 65, 48));
                case UIArtSprite.CardFrameBronze:
                    return new ArtDefinition(UIArtSheet.Frames, new Rect(600, 750, 115, 190), new Vector4(24, 34, 24, 34));
                case UIArtSprite.CardFrameSilver:
                    return new ArtDefinition(UIArtSheet.Frames, new Rect(735, 750, 115, 190), new Vector4(24, 34, 24, 34));
                case UIArtSprite.CardFrameGold:
                    return new ArtDefinition(UIArtSheet.Frames, new Rect(870, 750, 115, 190), new Vector4(24, 34, 24, 34));
                case UIArtSprite.CardFramePurple:
                    return new ArtDefinition(UIArtSheet.Frames, new Rect(1005, 750, 115, 190), new Vector4(24, 34, 24, 34));
                case UIArtSprite.ModalPanelFrame:
                    return new ArtDefinition(UIArtSheet.Frames, new Rect(1185, 520, 310, 420), new Vector4(55, 70, 55, 70));
                case UIArtSprite.FireSymbol:
                    return new ArtDefinition(UIArtSheet.UiAtlas, new Rect(510, 350, 170, 180), Vector4.zero);
                case UIArtSprite.IronSymbol:
                    return new ArtDefinition(UIArtSheet.UiAtlas, new Rect(690, 350, 170, 180), Vector4.zero);
                case UIArtSprite.LifeSymbol:
                    return new ArtDefinition(UIArtSheet.UiAtlas, new Rect(870, 350, 170, 180), Vector4.zero);
                case UIArtSprite.LuckOrb:
                    return new ArtDefinition(UIArtSheet.UiAtlas, new Rect(1210, 250, 220, 250), Vector4.zero);
                case UIArtSprite.GoldCoin:
                    return new ArtDefinition(UIArtSheet.UiAtlas, new Rect(80, 350, 180, 180), Vector4.zero);
                case UIArtSprite.Chest:
                    return new ArtDefinition(UIArtSheet.UiAtlas, new Rect(280, 320, 190, 210), Vector4.zero);
                case UIArtSprite.CardBack:
                    return new ArtDefinition(UIArtSheet.UiAtlas, new Rect(1080, 560, 260, 360), new Vector4(38, 48, 38, 48));
                case UIArtSprite.BlueShield:
                    return new ArtDefinition(UIArtSheet.UiAtlas, new Rect(110, 610, 190, 210), Vector4.zero);
                case UIArtSprite.RedShield:
                    return new ArtDefinition(UIArtSheet.UiAtlas, new Rect(330, 610, 190, 210), Vector4.zero);
                case UIArtSprite.PlayerTower:
                    return new ArtDefinition(UIArtSheet.BattlefieldProps, new Rect(30, 0, 385, 455), Vector4.zero);
                case UIArtSprite.EnemyTower:
                    return new ArtDefinition(UIArtSheet.BattlefieldProps, new Rect(430, 0, 385, 455), Vector4.zero);
                case UIArtSprite.PortalGate:
                    return new ArtDefinition(UIArtSheet.BattlefieldProps, new Rect(760, 110, 350, 365), Vector4.zero);
                case UIArtSprite.StoneWall:
                    return new ArtDefinition(UIArtSheet.BattlefieldProps, new Rect(1100, 200, 405, 210), Vector4.zero);
                case UIArtSprite.Campfire:
                    return new ArtDefinition(UIArtSheet.BattlefieldProps, new Rect(35, 485, 170, 305), Vector4.zero);
                case UIArtSprite.WoodenFence:
                    return new ArtDefinition(UIArtSheet.BattlefieldProps, new Rect(265, 565, 360, 180), Vector4.zero);
                case UIArtSprite.PineTree:
                    return new ArtDefinition(UIArtSheet.BattlefieldProps, new Rect(690, 455, 210, 330), Vector4.zero);
                case UIArtSprite.FlowerBush:
                    return new ArtDefinition(UIArtSheet.BattlefieldProps, new Rect(935, 565, 230, 185), Vector4.zero);
                case UIArtSprite.RockCluster:
                    return new ArtDefinition(UIArtSheet.BattlefieldProps, new Rect(1165, 510, 315, 230), Vector4.zero);
                case UIArtSprite.GroundPatch:
                    return new ArtDefinition(UIArtSheet.BattlefieldProps, new Rect(220, 805, 505, 155), Vector4.zero);
                case UIArtSprite.CrownPlaque:
                    return new ArtDefinition(UIArtSheet.BattlefieldProps, new Rect(785, 780, 345, 210), Vector4.zero);
                default:
                    return new ArtDefinition(UIArtSheet.Frames, new Rect(58, 505, 205, 205), new Vector4(42, 42, 42, 42));
            }
        }

        private static bool HasBorder(UIArtSprite spriteId)
        {
            return Definition(spriteId).border != Vector4.zero;
        }

        private static bool ContainsAny(string value, params string[] needles)
        {
            for (int i = 0; i < needles.Length; i++)
            {
                if (value.Contains(needles[i])) return true;
            }
            return false;
        }

        private struct ArtDefinition
        {
            public readonly UIArtSheet sheet;
            public readonly Rect topRect;
            public readonly Vector4 border;

            public ArtDefinition(UIArtSheet sheet, Rect topRect, Vector4 border)
            {
                this.sheet = sheet;
                this.topRect = topRect;
                this.border = border;
            }
        }
    }
}
