using System;
using UnityEngine;

public class TankUIPage
{
    public static readonly EUIPage ingameUi = new EUIPage(1, "IngameUI");
}

public class CustomUIPage
{
    public static readonly EUIPage customPage = new EUIPage(2, "UIAnimUI");
}

public class TankEFxTag
{
    public static readonly EFxTag FireFx = new EFxTag(1, "FireExplosion");
    public static readonly EFxTag HitWallFx = new EFxTag(2, "ShellExplosion");
    public static readonly EFxTag HitTankFx = new EFxTag(3, "TankExplosion");
}

public enum ActorState
{
    Idle,
    Attack,
    Move,
    Dead,
    Stun,
    Float,
    Sink,
    Motionless,
    NoWalkable,
    SpecialAction,
    VictoryAction,
}

public enum AttackState
{
    Normal,
    PrecastLoop,
    CastInstant,
    CastLoop,
    Finished,
}

public enum AnimId
{
    None,
    Stand,
    CombatStand,
    Run,
    Attack0,
    Attack1,
    Attack2,
    Precast0,
    Precast1,
    Precast2,
    Precast3,
    Precast4,
    Precast5,
    Precast6,
    Precast7,
    Precast8,
    Precast9,
    Cast0,
    Cast1,
    Cast2,
    Cast3,
    Cast4,
    Cast5,
    Cast6,
    Cast7,
    Cast8,
    Cast9,
    CastLoop0,
    CastLoop1,
    CastLoop2,
    CastLoop3,
    CastLoop4,
    CastLoop5,
    CastLoop6,
    CastLoop7,
    CastLoop8,
    CastLoop9,
    Hit,
    Stun,
    Dying,
    Dead,
    Reserve1,
    Reserve2,
    Reserve3,
    Reserve4,
    Victory,
    SpecialAction0,
    SpecialAction1,
    SpecialAction2,
    SpecialAction3,
    SpecialAction4,
    SpecialAction5,
    SpecialAction6,
    SpecialAction7,
    SpecialAction8,
    SpecialAction9,
}

public class AnimatorStateId
{
    public static readonly int CombatStand = Animator.StringToHash("Base Layer.CombatStand");
    public static readonly int Idle = Animator.StringToHash("Base Layer.Idle");
    public static readonly int Run = Animator.StringToHash("Base Layer.Run");
    public static readonly int Dying = Animator.StringToHash("Base Layer.Dying");
    public static readonly int Attack0 = Animator.StringToHash("Base Layer.Attack0");
    public static readonly int Attack1 = Animator.StringToHash("Base Layer.Attack1");
    public static readonly int Attack2 = Animator.StringToHash("Base Layer.Attack2");
    public static readonly int Attack3 = Animator.StringToHash("Base Layer.Attack3");
}