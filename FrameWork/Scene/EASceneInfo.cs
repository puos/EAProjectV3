using System;


[AttributeUsage(AttributeTargets.Class,AllowMultiple = false, Inherited = true)]
public class EASceneInfoAttribute : Attribute
{
    public Type classType { get; private set; }
    public string className { get; private set; }

    public EASceneInfoAttribute(Type classType)
    {
        this.classType = classType;
        this.className = classType.Name;
    }
}