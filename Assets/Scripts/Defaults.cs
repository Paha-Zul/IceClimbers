using UnityEngine;
using System.Collections;

public class Defaults  {
    public readonly static string CoinPrefString = "Coins";
    public readonly static string RopeLengthPrefString = "RopeLength";
    public readonly static string RopeSpeedPrefString = "RopeSpeed";
    public readonly static string BouncinessPrefString = "Boundiness";

    public readonly static float RopeLength = 4;
    public readonly static float RopeSpeed = 0.05f;
    public readonly static float Bounciness = 0.3f;
    public readonly static float RopeLengthDecrease = -0.1f, RopeSpeedIncrease = 0.1f, BouncinessIncrease = -0.01f;
    public readonly static float WallSpeed = 0.02f;

    public readonly static float InitialShardSpawnSpeed = 3f; //Initial spawn rate of ice shards
    public readonly static float ShardSpawnIncreaseIncreaseInterval = 8f; //Increases the spawn interval every x amount of time (in seconds)
    public readonly static float ShardSpawnTimeIncreaseAmount = 0.1f; //Amount of time to decrease the spawn speed.

    public readonly static int ShardsDodgedBeforeBirdSpawn = 15;
    public readonly static float InitialBirdSpawnSpeed = 5;
    public readonly static float NumShardsPerBirdSpawnIncrease = 5;
    public readonly static float BirdSpawnIncreaseSpeed = 0.2f;

    public readonly static int RopeSpeedCost = 10, RopeLengthCost = 10, BouncinessCost = 10, HookSpeedCost = 10, HardHatCost = 50;
    public readonly static int RopeSpeedCostIncr = 1, RopeLengthCostIncr = 1, BouncinessCostIncr = 1, HookSpeedCostIncr = 1, HardHatCostIncr = 10;
    public readonly static int maxRopeSpeedUpgrades = 10, MaxRopeLengthUpgrades = 20, maxBouncinessUpgrade = 10, maxHookSpeedUpgrade = 10, maxHardHatUpgrade = 1;
}
