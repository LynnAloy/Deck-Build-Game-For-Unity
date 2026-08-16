using XLua;

[CSharpCallLua]
public delegate double CalculateDamageDelegate(
    double baseDamage,
    double targetHpPercent
);