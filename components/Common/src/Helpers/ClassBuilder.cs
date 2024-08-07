﻿
namespace AntDesign.Extensions;

public record BuilderItem(string Value, bool Show);


public class ClassBuilder : BuilderBase
{
    private string _name = "";

    internal ClassBuilder(string name)
    {
        _name = name;
    }

    public ClassBuilder() : base()
    {

    }
    public ClassBuilder(ClassBuilder builder) : base(builder)
    {

    }

    public override string ToString()
    {
        var res = TransitionBuilder?.GetContentArr().ToList() ?? new List<BuilderItem>();
        res.AddRange(base.GetContentArr());
        var result = string.Join(" ", res.Select(x => x.Value).Distinct());
        return result;
    }

    public override string Build() => ToString();

    public static implicit operator ClassBuilder(string clsStr)
    {
        var builder = new ClassBuilder();
        if (!string.IsNullOrWhiteSpace(clsStr))
        {
            builder.Add(clsStr.Trim());
        }

        return builder;
    }
}