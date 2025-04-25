using System;


namespace MyFrameWork.AppTool;
[AttributeUsage(AttributeTargets.Property)]
public class SelectSourceAttribute : Attribute
{
    public string SourceName { get; }

    public SelectSourceAttribute(string sourceName)
    {
        SourceName = sourceName;
    }
}
