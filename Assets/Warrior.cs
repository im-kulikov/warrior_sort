using System;
using System.Collections.Generic;
using Debug = System.Diagnostics.Debug;

public enum Force
{
    Red,
    Blue,
}

public class WarriorComparer : IComparer<Warrior>
{
    private readonly int _round;

    public WarriorComparer(int round)
    {
        _round = round;
    }

    public int Compare(Warrior a, Warrior b)
    {
        Debug.Assert(a != null, nameof(a) + " != null");
        Debug.Assert(b != null, nameof(b) + " != null");

        return a.Coefficient(_round).CompareTo(b.Coefficient(_round)) * -1;
    }
}

[Serializable]
public class WarriorData
{
    public List<Warrior> warriors;

    // test data
    public List<Warrior> first;
    public List<Warrior> second;
}

[Serializable]
public class Warrior
{
    public Force team;
    public int initiative;
    public int speed;
    public int number;

    public int Coefficient(int round)
    {
        var bonus = 0;

        // добавляем бонус по условию:
        // - в чётном приоритет у синих
        // - в нечётном приоритет у красных
        if (team == Force.Blue && round % 2 == 0 || team == Force.Red && round % 2 != 0) bonus = 10;

        return initiative * 1000 + speed * 100 + bonus + (10 - number);
    }

    public bool IsSame(Warrior elem)
    {
        return team == elem.team &&
               initiative == elem.initiative &&
               speed == elem.speed &&
               number == elem.number;
    }

    public Warrior(Force team, int initiative, int speed, int number)
    {
        this.team = team;
        this.speed = speed;
        this.number = number;
        this.initiative = initiative;
    }
}