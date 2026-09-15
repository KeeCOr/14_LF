# LotteryFantasy — Steam Achievements

## Planned Achievements

| ID | Name | Description | Hidden |
|----|------|-------------|--------|
| ACH_001 | First Victory | Win your first battle | No |
| ACH_002 | Perfect | Complete without taking damage | No |
| ACH_003 | Master | Complete all stages | No |

## Steamworks.NET Integration
- Install Steamworks.NET SDK to Assets/Plugins/Steamworks.NET/
- steam_appid.txt must be in Unity project root with real App ID
- Call SteamUserStats.SetAchievement() and SteamUserStats.StoreStats() on unlock
