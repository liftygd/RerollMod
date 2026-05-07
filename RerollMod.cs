using MewgenicsModSdk;
using MewgenicsModSdk.Api;
using MewgenicsModSdk.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

namespace RerollMod;

public class RerollMod : MewgenicsMod
{
    public override string Id => "reroll_mod";
    public override string Name => "Reroll";
    public override string Version => "1.0.0";
    public override string Description => "Does something cool.";
    public override string Category => "Gameplay";

    bool logging = true;
    public void log(string message) { if (logging) Log(message); }

    private Random random = new Random();
    private Server server = new Server();
    private bool _runActive;

    Dictionary<string, List<string>> abilities = new Dictionary<string, List<string>>();
    Dictionary<string, List<string>> passives = new Dictionary<string, List<string>>();

    private void loadGon()
    {
        abilities["butcher"] = [
            "HogRush", "Burp", "SelfMutilate", "ForceFeed", "Fartoom",
            "Mutilate", "SkullBash", "Shred", "Chomp", "Succ", "Consume",
            "Trudge", "BodySlam", "BloodMagic", "SmellBlood", "Vurp",
            "LunchTime", "Tromp", "LightenTheLoad", "Crushinator",
            "CannonBall", "Monch", "DeathWind", "Spoil", "Grill", "Roast", "BadGas",
            "ButcherPurge", "Binge", "MyTurn", "Gib", "Swallow", "Track", "Sharpen",
            "FireFart", "RoughToss", "Bowl", "BowlDash", "TaintedOffering",
            "DeliciousScent", "Cough", "Reflux", "Tryptophan", "HookBind", "Regurge",
            "Grapnel", "Rehook", "Contaminate", "LodgeHook", "Butcher", "Chonkwalk", "Indigestion_Fart"
        ];

        abilities["fighter"] = [
            "Dash", "Spin", "FirePunch", "IcePunch", "ThunderPunch", "FurySwipes", "SideSlash", "FighterLeap",
            "Uppercut", "Counter", "TailWhip", "Poke", "Nip", "Push", "FalconPunch", "Exert", "Enrage", "Tumble",
            "Confront", "Juiced", "CosmicPunch", "FighterTaunt", "GravitySlam", "Berserk", "BerserkDash",
            "Challenge", "Slap", "Stoopzerk", "LastThought", "SleeperHold", "Grapple", "ThinkTooHard", "Zoomzerk",
            "Reposition", "FighterBonusThrow", "Bloodzerk", "Lacerate", "ExhaustingBlow", "ChaosRampage", "MeteorSlam",
            "MuscleMemory", "Inhale", "OneTwoPunch", "TeamSpin", "TeamFlex", "Huddle", "RagePunch", "BreakingPoint",
            "AssertDominance", "DumbMove", "ReflexPunchJab", "SuckerPunch", "Stick", "Hurl", "BigPunch", "Ram"
        ];

        abilities["colorless"] = [
            "Block", "Rest", "Brace", "Roll", "SharpenClaws", "Reach",
            "ManaDrain", "SoothingGlow", "Ponder", "Focus", "Metabolize",
            "Brainstorm", "Momentum", "TapLand", "GainThorns", "HolyStep",
            "LoveTap", "PrepareToJump", "MarkJump", "BoostSpellRange", "PushMove",
            "Gamble", "SoulReap", "Hunt", "Flex", "HolyBlood", "Dart", "Smack",
            "Spit", "FetusSpit", "MiniHook", "MiniDistract", "ButtScoot",
            "Confusion", "PlayDead", "Reflect", "HealBolt", "SlipThrough",
            "Dump", "Snacks", "FeatherFeet", "Reduce", "Nerf", "Trip", "Copycat",
            "DollUp", "StackTheDeck", "Infiltrate", "Burst",
            "Suppress", "Endeavor", "LotteryShottery", "CatNap", "PissYourself",
            "FindARock", "BurgeoningBlast", "BurgeoningBarrier", "BurgeoningBattery",
            "HoseOff", "Taint", "PokeWound", "WasteTime", "Desecrate", "Contort",
            "RussianRoulette", "Step", "Interchange", "LookAtMe", "Rouse", "Shift",
            "Donate", "Magnet", "ScuffItOff", "BarfBall", "DexterousHit", "Till",
            "Itch", "Meow", "Swat", "LickHeal", "Purr", "Hiss", "Knead",
            "BuyCatnip", "VetVisit", "HireHitman", "SubwayRide", "GymMembership",
            "SuperCrateBox", "BBQ", "CPR", "Blow", "Toast", "Landscape", "Zap",
            "Sunburn", "ColdShoulder", "BlowKiss", "ForbiddenFart", "WetHairball"
        ];

        abilities["hunter"] = ["LineShot", "HailOfNails", "SpawnMaggotFriend", "SpawnPooterFriend",
            "Marked", "ScatterShot", "BrambleShot", "BearTrap", "Harpoon", "TwinShot", "CrossShot",
            "SpawnBaitTrap", "BombShot", "SummonBrambles", "FireShot", "TrailBlazer", "FocusShot",
            "Shards", "TerrainWalk", "Extend", "ChaosShot", "NeedleShot", "SpikeTrap", "FleaShot",
            "WebTrap", "LastHit", "CupidsArrow", "ArrowFlurry", "HeavyShot", "StakeOut",
            "Snipe", "Diversion", "ArrowSmith", "TacticalRetreat", "Infest", "CollectPelt", "SentryMode",
            "Pheromones", "SpawnTomTomFriend", "ScoutMe", "ShootHere", "CraftArrow", "CharmTrap",
            "BounceShot", "Picnic", "SoothingShot", "Vivisect", "PoisonLace", "SlopThePigs", "SpiderInjector",
            "PersistentHunt", "Bunker"
        ];

        abilities["mage"] = [
            "Surf", "Bolt", "Fireball", "FreezeRay", "Blast", "MagicMissile", "WallOfFire",
            "MeteorStorm", "MegaBlast", "Slow", "Enlarge", "WindSlash", "Warp", "MageTeleport", "MageSwap",
            "Absorb", "IceArmor", "FireArmor", "ManaMeld", "Inspire", "Telefrag", "ChaosTeleport", "CryoHeal",
            "Gust", "Blizzard", "Inferno", "Thunderburst", "DealWithTheDevil", "ForbiddenFlame",
            "ForbiddenFlood", "WaterSphere", "ChainLightning", "Shatter", "ForbiddenFulmination",
            "FireBolt", "IcicleTaser", "FreezerBurn", "Corrupt", "Jolt", "Smolder", "FireSurge",
            "IceSurge", "LightningSurge", "Creshendo", "Divide", "ForbiddenFrost", "BlackMagic",
            "Teach", "HomingBlasts", "Replicate", "Magnify", "TriAttack"
        ];

        abilities["tank"] = ["Taunt", "HeadButt", "ThrowShield", "ChewCud", "AssBlast", "Chew", "BatterUp",
            "BackBreaker", "Intimidate", "Toss", "BonusToss", "NubbyToss", "BellyFlop",
            "ToadJump_BasicMove", "BellyFlop_BasicMove", "TankTrample", "TankSwap", "ToTheRescue",
            "TankTantrum", "Earthquake", "RockToss", "BarbedWire", "DrawAttention", "BowlOver",
            "Clap", "TankRockSong", "RockCrusher", "BodyGuard", "Gore", "RockBlast", "RockTomb",
            "SwapPositions_WideLoad", "BearHug", "Fissure", "BigRock", "FlipFlop", "Lunge", "Nudge",
            "StoneGaze", "Medusa", "Anchor", "EatRock", "PlantFeet", "IronHead", "GangUp", "Aftershock",
            "SteelSkin", "FaultLine", "Demolish", "FollowUpDash", "CatapultJump", "PushThrough", "Spur",
            "Supper", "FullForce", "Sandstorm", "Thicken"
        ];

        abilities["medic"] = [
            "RangedHeal", "MeleeHeal", "Malaise", "OpenWounds", "Prayer", "Convert", "Cleanse", "Purge",
            "HereticMark", "Zealot", "Haste", "Rally", "BuddyUp", "HealingFall", "RallyCharge",
            "ReverseDamage", "Rebuke", "Wish", "WitchHunt", "FriendOrFoe", "Revive", "HolyLight",
            "BornAgain", "Benediction", "Crusade", "HallowedGround", "Enlighten",
            "Anoint", "EyeForAnEye", "WrathOfGod", "Adoubement", "DivineProtection", "ChosenWarrior",
            "SwiftSanctify", "DivineGift", "HolyWeapon", "GetDown", "MedicObey", "Awaken", "Baptism",
            "Pray", "Emergency", "GuardianAngel", "Booster", "Stimulants", "BlindingLight", "CircleOfProtection",
            "CallOver", "Grace", "TurnFoe", "HealingSalve", "Heathens"
        ];

        abilities["thief"] = [
            "Assassinate", "BoostBackstab", "PoisonGas", "PoisonNail", "WeakeningNail", "SharpNail",
            "Double", "CoinToss", "MoveAgain", "AttackAgain", "Camouflage", "Shadow", "TimeWalk",
            "DoubleLoot", "Distract", "Rebound", "CutPurse", "EagleEye", "PickPocket", "Backflip",
            "Blur", "GreedStep", "Stalk", "Declaw", "QuickRoll", "Slice", "PocketSand",
            "Nightshade", "Shadowshift", "SlingShade", "Caltrops", "PierceShot", "Cheat", "VenomBarrage",
            "LootCorpse", "SeverArtery", "Fade", "SharpenNail", "SneakUp", "Shank", "StealKidney", "StealLuck",
            "ThiefSwap", "Pierce", "WindUp", "TripleNails", "SkinDisguise", "Chakram", "Jitter", "StealTime",
            "Outskirts", "PoisonDip", "LuckyPenny"
        ];

        abilities["necromancer"] = [
            "MaggotArmy", "Reanimate", "Rebirth", "Pestilence", "Weakness", "SoulSuck", "EvilIncarnate",
            "SoulLink", "WeAreOne", "BloodRain", "AnimateDead", "DeathBloom", "Scare", "SoulTransfer",
            "RandomReap", "SlitWrists", "Whisper", "DarkStep", "Leeches", "Shriek", "LastGasp",
            "Seppuku", "RaiseTheDead", "FullMoon", "Unearth", "BloodGeyser", "Flatline", "Replace",
            "SummonBones", "GigaDrain", "Bloodletting", "MassPsychosis", "Debone", "Reap", "Haunt", "Spook",
            "CarrionShot", "LifeDrain", "CoffinFlop", "DonateBlood", "Seance", "GoLimp", "DemonicPact",
            "RandomSoulLink", "RandomDualSoulLink", "Curse", "LeechSwarm", "Feed", "Hush", "ReaperStep",
            "ForbiddenFamine", "FleshGolem", "TradeLife", "AbsorbSoul", "Gravecrawl", "DigUpTheDead", "SpiderEgg", "ClewOfLeeches"
        ];

        abilities["druid"] = [
            "CrowFlutter", "CrowFlap", "ManaBomb", "SongOfSpring", "GrantLife", "SquirrelSquad",
            "SquirrelFurySwipes", "SummonSquirrel", "SummonSnake", "SummonTurtle", "SummonToad",
            "DruidSwap", "BattleCry", "PullToSafety", "BrambleBurst", "FlowerFeet",
            "ThornyFeet", "Encourage", "Protection", "Promote", "SafetyDance", "WarCry", "TigerForm",
            "MonkeyForm", "RhinoForm", "SummonCatepillar", "SleepPowder", "CallTheWind", "InspirationalSong",
            "DeathMetal", "ChaChaSlide", "BestowWisdom", "RaccoonForm", "Scavenge", "SummonCrow",
            "WeWillRockYou", "TreeForm", "HydroPump", "ControlPlants", "ControlWater", "ControlAir",
            "FamiliarSelfDestruct", "FeralMelee", "Entangle", "Lullaby", "WeAreTheChampions", "Cheerlead",
            "NaturesBlessing", "ThrowEgg", "SquirrelForm", "PlantMushroom", "Serenade", "WindyStep",
            "ElkForm", "MockingbirdForm", "FromTheTrees"
        ];

        abilities["tinkerer"] = [
            "Research", "Discharge", "Repair", "ShoddyJetpack", "SpawnDecoy", "Switcheroo", "SpringShoes",
            "Flamethrower", "TurretShot", "RocketTurretShot", "AutoPilot", "Recycle", "BuildTurret",
            "RocketSkates", "DrillDown", "ArmorUp", "FreshOffTheForge", "ElectricNail", "Craft", "Shockwave",
            "Math", "Reprogram", "Improve", "Catbot", "Bombchu", "RemoteDetonator", "ShortCircuit",
            "Electrolyze", "EjectButton", "Firecrackers", "Upgrade", "Eureka", "PunchBot", "FastHands",
            "MechSuitEject", "MechSuitBarrage", "MechSuitDash", "UnreliableShield",
            "UnreliableMissile", "SpareParts", "BatteryNuke", "ExperimentalTeleporter", "ShockTherapy",
            "BuildNuke", "InstantBarrier", "VoltTackle", "Smash", "ShedScrap", "RepairArmor", "RocketRide",
            "Roomba_Bump", "RoboVac", "NurseBot", "TeslaCoil", "RefineMaterials", "Fabricate", "Sparks", "Hone"
        ];

        abilities["psychic"] = [
            "Telekinesis", "Suggestion", "MindControl", "MegaGrav", "PsyFlutter", "MagnetPull", "MindBlast",
            "PsychicChoke", "SkyShatter", "ReadMind", "AlterDNA", "Flicker", "MindMeld", "Vaccuum",
            "GrowHead", "Ping", "FlashForward", "Order", "TemporalShards", "RealityScramble", "Glare", "BlindingFlash",
            "Snatch", "FutureSight", "MassManaLeech", "BecomeEntropy", "FastForward", "AncestralRecall",
            "CumulativeBlast", "Hallucinate", "MassHysteria", "ExtraTurnQuestion", "MindCrack", "Reset", "Mimic",
            "ChaosSwap", "Asteroid", "Stasis", "Pass", "ThinkDeep", "Puppet", "YouSeeNothing", "ForceBlast",
            "IncreaseGravity", "Manifest", "Flip", "Withdraw", "ForceCone", "Inversion", "Echo", "Slipstream",
            "MindCrack_EldritchVisage"
        ];

        abilities["monk"] = [
            "Propell", "Hadouken", "Cartwheel", "StoneFists", "Transcend", "HipToss", "Bruise", "Slapback",
            "Finisher", "Reverberate", "ComboThrow", "ComboPull", "OneWithTheWind", "Pogo", "TrainArms",
            "Porcupine", "Anneal", "DeepDive", "HopAndBlock", "TrainMind", "Meditate", "DoomPunch",
            "KiBurst", "DragonPunch", "TrainLegs", "ReallyFastRun", "DetectWeakness",
            "KineticCharge", "AirBurst", "TrainBody", "ReleaseEnergy", "Pummel", "QuickAttack", "PerfectForm",
            "WarmupStretch", "FlyingFist", "RapidFlowSpin", "SpiritBomb", "OnePunch", "UnbridledHits",
            "Kamehameha", "SideStep", "UnimpededLunge", "DoubleDragon", "FistOfFate", "Nirvana", "EmptyMind",
            "Position", "ChargeFists", "Apprentice"
        ];

        passives["colorless"] = [
            "SelfAssured", "LuckDrain", "Infested", "Worms", "Amped", "Furious", "Deathless", "MetalDetector",
            "DeathProof", "Leader", "Mange", "ETank", "Careful", "DirtyClaws", "LateBloomer", "Study",
            "NaturalHealing", "LongShot", "FastFooted", "Slugger", "Pulp", "Amplify", "DeathBoon",
            "SantaSangre", "Untouched", "Daunt", "AnimalHandler", "WhipCracker", "PressurePoints", "Gassy",
            "Dealer", "Patience", "Wiggly", "MiniMe", "BareMinimum", "Unrestricted", "DeathsDoor", "OverConfident",
            "SerialKiller", "StrengthInNumbers", "FightersSoul", "HuntersSoul", "MagesSoul", "ClericsSoul",
            "TanksSoul", "ThiefsSoul", "MonksSoul", "ButchersSoul", "DruidsSoul", "TinkerersSoul", "NecromancersSoul",
            "PsychicsSoul", "Charming", "FirstImpression", "Scavenger", "ZenkaiBoost", "Protection", "Rockin",
            "Mania", "Lucky", "OneEighty", "JestersSoul", "HotBlooded", "ToxicBlooded", "BloodBlooded", "VoidSoul"
        ];

        passives["fighter"] = [
            "BloodLust", "Avenger", "Scars", "FasterWhenHit", "KillsHeal", "Vengeful", "HamsterStyle",
            "WeaponMaster", "ShoulderCheck", "SkullSmash", "TurtleStyle", "Overpowered", "FightMe",
            "HighAsYouCanCount", "DumbMuscle", "ThickSkull", "MostValuableCat", "RatStyle", "Boned",
            "ReflexPunch", "HitMe", "Smash", "PunchFace", "Recoil"
        ];

        passives["hunter"] = [
            "TakeAim", "TowerDefense", "HuntersBoon", "BroodMother", "TaintedMother", "Quiver",
            "SplitShot", "Hazardous", "ThornArrows", "Traps", "CatchProjectiles", "TrickyTraps",
            "GravityFalls", "HawkEye", "Spotters", "LuckSwing", "Host", "Sniper", "RubberArrows",
            "TalkToAnimals", "AnimalControl", "SleepDarts", "Survivalist", "Fleabag"
        ];

        passives["mage"] = [
            "Micronaps", "HolyMantel", "Shrapnel", "BurningPaws", "LightningPaws", "IcePaws", "PawMissile",
            "Overload", "ChargeUp", "DeathChill", "Recharged", "EnergyStorm", "FireArmor", "IceArmor",
            "Resonance", "LearnFromMe", "LightningArmor", "LongCast", "LightUpTheStage", "ElementalAttunement",
            "LatentEnergy", "Five", "MagicGuru", "One", "Two", "Four"
        ];

        passives["tank"] = [
            "Thorns", "HeavyHanded", "SlackOff", "Scabs", "EyeCatchin", "ThunderThighs", "Plow",
            "PetRocks", "ToadStyle", "ChainKnockback", "ProtectiveAura", "Wrestlemaniac", "MountainForm",
            "HomeRun", "RockAspect", "WideLoad", "HardHead", "MyLeg", "Hardy", "SlowAndSteady", "FollowUp",
            "CatAPult", "ShovingMatch", "Stoic", "PriorityTarget"
        ];

        passives["medic"] = [
            "HealingAura", "NaturalHealer", "Eternal", "Blessed", "AngelicInspiration",
            "TopOff", "SharingIsCaring", "Caretaker", "MoraleBoost", "RangedMedic", "Godspeed",
            "GodWarrior", "BreathOfLife", "ThouShaltNotKill", "ThouShaltNotCovet", "BlessingOfHolyFire",
            "AlmsForThePoor", "Purifier", "VeneratedTouch", "ProtectTheWeak", "ThouShaltObey", "EnchantedRelic",
            "BlessingOfSpirit", "Heathens"
        ];

        passives["thief"] = [
            "Backstabber", "GoldenClaws", "Shadow", "PoisonTips", "Burgle", "SwiftKiller", "LongStrider",
            "DoubleThrow", "BountyHunter", "RazorClaws", "Looter", "Zip", "WeakSpot",
            "Penetrate", "AfterImage", "Shiv", "Critical", "LootHoarder", "Cripple", "Agile", "Shank",
            "FlipACoin", "ShakeDown", "SweetSpot", "Pinpoint"
        ];

        passives["necromancer"] = [
            "Vampirism", "OneWithNothing", "BedBugs", "WormLord", "InfiniteRebirth", "SacrificialLamb",
            "OffloadPain", "CambionConception", "Leechmother", "Infected", "LastGasp",
            "RelentlessDead", "ChainsOfGuilt", "DarkPriest", "Undeath", "NumbingLeeches", "EternalHealth",
            "Torpor", "SoulBound", "Superstition", "ImmortalLeeches", "CorpseConnoisseur", "Parasitic",
            "SpreadSorrow"
        ];

        passives["druid"] = [
            "SuperCrow", "NaturesGuidance", "PoisonIvy", "Pathfinder", "EmptyVessels", "WildAnimals", "BarkSkin",
            "SoothingSong", "Teamwork", "Bouquet", "GoodVibrations", "VersatileVocalist", "LikeAFish", "Encore",
            "SpecialFriends", "SneakAttack", "WildStyle", "BuddySystem", "FlowerPower", "Feral",
            "RapGod", "Animalistic", "Maestro", "MegaMinions"
        ];

        passives["tinkerer"] = [
            "VersionTwo", "WeaponProficiency", "LivingBattery", "FuzzyFeet", "EMP",
            "MrMega", "EscapeSequence", "ItemProxy", "LightningRod", "ItsAlive", "Energizer", "ReactiveArmor",
            "Nanobots", "Scrapper", "DemoMan", "DuctTape", "ArmoredPlating", "BoobyTrap", "RobotArms",
            "Conductor", "Napalm", "Ingenuity", "Shrapnel_Tinkerer", "Blacksmith"
        ];

        passives["butcher"] = [
            "Putrefy", "NeverFull", "MainCourse", "FreshMeat", "Masochist", "Glutton", "Hooked", "Stompy",
            "Barbed", "GrapplingHook", "PainGain", "WideSwing", "Confrontational", "HeaveHook", "Harpooner",
            "LordOfTheFlies", "Schadenfreude", "Gurgitator", "LooseMeat", "Hack", "BowlingBall", "Testy",
            "Incubator", "DukeOfFlies"
        ];

        passives["psychic"] = [
            "Flying", "SoulShatter", "Glow", "Blink", "FullPower", "RealityShatter", "MentalStorm",
            "Wither", "Flourish", "PsySmack", "Beckon", "MindTempest", "Overflow", "Omniscience",
            "PsionicRepel", "Enlightened", "MadVisage", "PowerUp", "TrueSight", "Radiation",
            "Drag", "Twiddle", "RepressedMemories", "EldritchVisage"
        ];

        passives["monk"] = [
            "SafeSwitching", "Mixup", "Turnabout", "MonkeyStyle", "BrickSkin", "JaggedEdges", "MindBreaker",
            "CobraStyle", "Tenderize", "LongArms", "SpreadThePain", "Harden", "IronSkin", "JetFists",
            "EnergyFists", "UnburdenedMotion", "UnburdenedStrikes", "UnburdenedThoughts",
            "RunningJab", "PerfectTechnique", "RapidFlow", "CounterBarrage", "FlowState", "DancingLights"
        ];
    }

    private string RandomSpell(string catClass)
    {
        log("RandomSpell triggered");
        log($"CatClass: {catClass}");
        string abil = abilities[catClass][random.Next(abilities[catClass].Count)];
        log(abil);
        return abil;
    }

    private string RandomPassive(string catClass)
    {
        log("RandomPassive triggered");
        log($"CatClass: {catClass}");
        string passive = passives[catClass][random.Next(passives[catClass].Count)];
        log(passive);
        return passive;
    }
    
    private string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    protected override void OnLoad()
    {
        log(Name + " loaded");
        loadGon();
        
        GameEvents.OnKeyDown += OnKeyDown;
        GameEvents.OnAdventureStart += OnAdventureStart;
        GameEvents.OnAdventureReturn += OnAdventureReturn;
        GameEvents.OnFightStart += OnFightStart;
        GameEvents.OnFightEnd += OnFightEnd;
        GameEvents.OnHouseUpdate += OnHouseUpdate;
        
        _runActive = Config.GetBool("runActive");
        Config.GetString("playerId", Guid.NewGuid().ToString());
        Config.GetString("playerName", RandomString(5));
        Config.GetString("server", string.Empty);
        Config.GetString("key", string.Empty);
        
        server.ActivateClient(Config);
    }

    private void OnHouseUpdate(HouseUpdateEvent @event)
    {
        if (!IsEnabled) return;
        if (!_runActive) return;

        EndRun();
    }

    private void OnFightStart(FightStartEvent @event)
    {
        if (!IsEnabled) return;
        log("OnFightStart");
        
        UpdateCats();
    }
    
    private void OnFightEnd(FightEndEvent @event)
    {
        if (!IsEnabled) return;
        log("OnFightEnd");

        if (@event.Result != FightResult.Lose) 
            return;
        
        EndRun();
    }

    private void OnAdventureReturn(AdventureReturnEvent @event)
    {
        if (!IsEnabled) return;
        log("OnAdventureReturn");
        
        EndRun();
    }

    private void OnAdventureStart(AdventureStartEvent @event)
    {
        if (!IsEnabled) return;
        log("OnAdventureStart");

        var cats = GetAdventureCats();
        foreach (var cat in cats)
            cat.Name = $"{cat.Name} | L";
        
        UpdateCats();
    }

    private void StartRun()
    {
        if (_runActive) return;
        
        log("Started run");
        
        _runActive = true;
        Config.Set("runActive", _runActive);
    }
    
    private void EndRun()
    {
        if (!_runActive) return;
        
        log("Ended run");
        
        _runActive = false;
        Config.Set("runActive", _runActive);
        
        server.ActivateClient(Config);
        server.EndRun(Guid.Parse(Config.GetString("playerId")));
    }

    private void UpdateCats()
    {
        StartRun();
        List<GameChar> cats = GetAdventureCats();
        
        for (int i = 0; i < cats.Count; i++)
        {
            var cat = cats[i];
            var catNameComposite = cat.Name.Split(" | ");
            StringBuilder catName = new StringBuilder();
            
            int rollCount = 0;
            if (catNameComposite.Length >= 2)
                rollCount = Convert.ToInt32(catNameComposite[1]);

            catName.Append($"{Config.GetString("playerName")} | {rollCount}");

            if (catNameComposite.Length >= 3)
                catName.Append($" | {catNameComposite[2]}");
            
            cat.Name = catName.ToString();
            UpdateCatOnServer(cat);
        }
    }

    private List<GameChar> GetAdventureCats()
    {
        List<GameChar> cats = GameWorld.Current.GetCats();
        for (int i = cats.Count - 1; i >= 0; i--)
            if (!cats[i].IsInAdventureParty) cats.RemoveAt(i);

        return cats;
    }

    protected override void OnEnable()
    {
        log(Name + " enabled");
    }
    
    protected override void OnDisable()
    {
        log(Name + " disabled");
    }

    private void RollCat(GameChar cat)
    {
        var catNameComposite = cat.Name.Split(" | ");
        int rollCount = 0;
        bool rollLock = catNameComposite is [_, _, "L"];
            
        if (rollLock)
            return;
        
        string sp = cat.Spell1;
        string pa = cat.Passive0;

        string sp_n = RandomSpell(cat.ClassName.ToLower());
        string pa_n = RandomPassive(cat.ClassName.ToLower());

        while (sp == sp_n) sp_n = RandomSpell(cat.ClassName.ToLower());
        while (pa == pa_n) pa_n = RandomPassive(cat.ClassName.ToLower());

        cat.Spell1 = sp_n;
        cat.Passive0 = pa_n;
        
        if (catNameComposite.Length >= 2)
            rollCount = Convert.ToInt32(catNameComposite[1]) + 1;
        
        cat.Name = $"{Config.GetString("playerName")} | {rollCount}";
        
        server.ActivateClient(Config);
        server.RollCat(
            Guid.Parse(Config.GetString("playerId")),
            cat);
    }

    private void UpdateCatOnServer(GameChar cat)
    {
        string call = server.CreateCatState(
            Guid.Parse(Config.GetString("playerId")),
            cat);
        
        log(call);
        
        server.ActivateClient(Config);
        server.UpdateCat(call);
    }

    protected void OnKeyDown(KeyEventArgs e)
    {
        if (!IsEnabled) return;
        if ((e.Scancode == SDL_Scancode.P || e.Scancode == SDL_Scancode.O) && !e.IsRepeat)
        {
            log($"🔵 [Reroll] Key {e.Key} pressed! (111 = O, 112 = P)");
            List<GameChar> cats = GetAdventureCats();

            if (e.Scancode == SDL_Scancode.O) RollCat(cats[0]);
            else if (e.Scancode == SDL_Scancode.P) RollCat(cats[1]);
        }
    }

    internal static unsafe class Exports
    {
        private static readonly RerollMod _mod = new();

        [UnmanagedCallersOnly(EntryPoint = "MewMod_GetInfo")]
        public static ModInfo* GetInfo() { try { return ModInfoHelper.GetInfo(_mod); } catch { return null; } }

        [UnmanagedCallersOnly(EntryPoint = "MewMod_Init")]
        public static void Init(MewgenicsApi* api) { try { _mod.InternalLoad(api); } catch { } }

        [UnmanagedCallersOnly(EntryPoint = "MewMod_Enable")]
        public static void Enable() { try { _mod.InternalEnable(); } catch { } }

        [UnmanagedCallersOnly(EntryPoint = "MewMod_Disable")]
        public static void Disable() { try { _mod.InternalDisable(); } catch { } }

        [UnmanagedCallersOnly(EntryPoint = "MewMod_ConfigReload")]
        public static void ConfigReload() { try { _mod.InternalConfigReload(); } catch { } }
    }
}