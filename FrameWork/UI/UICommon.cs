using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct EUIPage
{
    private int id;
    private string name;

    public EUIPage(EUIPage e)
    {
        id = e.id;
        name = e.name;
    }

    public EUIPage(int id, string name)
    {
        this.id = id;
        this.name = name;
    }
    public int Id { get { return id; } }
    public override string ToString() { return name; }
    public static bool operator !=(EUIPage lhs,EUIPage rhs) {  return lhs.id != rhs.id; }
    public static bool operator ==(EUIPage lhs, EUIPage rhs) { return lhs.id == rhs.id; }
}

public struct EUIPopup
{
    private int id;
    private string name;

    public EUIPopup(EUIPopup e)
    {
        id = e.id;
        name = e.name;
    }

    public EUIPopup(int id, string name)
    {
        this.id = id;
        this.name = name;
    }
    public int Id { get { return id; } }
    public override string ToString() { return name; }

    public static bool operator !=(EUIPopup lhs, EUIPopup rhs) { return lhs.id != rhs.id; }
    public static bool operator ==(EUIPopup lhs, EUIPopup rhs) { return lhs.id == rhs.id; }
}


public struct EUIComponent
{
    private int id;
    private string name;

    public EUIComponent(EUIComponent e)
    {
        id = e.id;
        name = e.name;
    }

    public EUIComponent(int id, string name)
    {
        this.id = id;
        this.name = name;
    }
    public int Id { get { return id; } }
    public override string ToString() { return name; }
}

public class EUIDefault
{
    public static readonly EUIPage PageEnd = new EUIPage(0, "End");
    public static readonly EUIPopup PoupEnd = new EUIPopup(0, "End");
}