namespace Zick.GameScheduler.Backend.Data.Models.ServerVariables;

public enum RaceFlags
{
    AbsSystemAllowed = 32,
    ScSystemAllowed = 64,
    TcsSystemAllowed = 128,
    OnlyRealisticAssists = 16 ,
    MechanicalFailuresEnabled = 262144,
    FillWithAi = 131072,
    TimedRace = 1048576,
    AntiBlatantCollisions = 1073741824
}

public enum AllowedView
{
    HelmetOnly=0,
    All=2
}

public enum DamageType
{
    None = 0,
    VisualOnly = 1,
    PerformanceImpacting = 2,
    Full = 3
}

public enum DamageScale
{
    Low = 0,
    Medium = 1,
    High = 2,
    Max = 3
}

public enum PitAssist
{
    On = 0,
    Off = 1
}

public enum TireWearRate
{
    X5 = 2,
    X4 = 3,
    X3 = 4,
    X2 = 5,
    X1 = 6,
    Off = 8
}

public enum FuelRate
{
    X5 = 3,
    X4 = 4,
    X3 = 5,
    X2 = 6,
    X1 = 0,
    Off = 2
}

public enum CutStrictness
{
    
}