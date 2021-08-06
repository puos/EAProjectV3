using System;

public enum EFxTag
{
    FireFx,
    HitWallFx,
    HitTankFx,
}

public enum EFSMState 
{ 
    None = 0, 
    Move, 
    Chasing, 
    Attack, 
    Dead 
}