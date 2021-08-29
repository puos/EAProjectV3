using System;

public class TankUIPage
{
    public static readonly EUIPage ingameUi = new EUIPage(1, "IngameUI");
}

public class TankEFxTag
{
    public static readonly EFxTag FireFx = new EFxTag(1, "FireExplosion");
    public static readonly EFxTag HitWallFx = new EFxTag(2, "ShellExplosion");
    public static readonly EFxTag HitTankFx = new EFxTag(3, "TankExplosion");
}