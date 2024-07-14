using AngleSharp.Css.Dom;
using AntDesign.Extensions.Samples.Demos;
using Moq;
using System;

namespace AntDesign.Extensions.Components.Dialog.Test;

public class PositionTest : DialogTestBase
{
    public PositionTest() :base()
    {
    }


    [Fact]
    public void Top()
    {
        var cut = RenderComponent<PositionTop>();
        cut.Find("button").Click();

        var positionDiv = cut.Find("body div.antd-ext-dialog-root .antd-ext-dialog-position");

        var top = positionDiv.GetStyle().GetTop();
        Assert.True(positionDiv.GetStyle().GetTop() == "200px");
        Assert.True(positionDiv.GetStyle().GetWidth() == "520px");
    }

    [Fact]
    public void Width()
    {
        var cut = RenderComponent<PositionWidth>();
        cut.Find("button").Click();

        var positionDiv = cut.Find("body div.antd-ext-dialog-root .antd-ext-dialog-position");

        var top = positionDiv.GetStyle().GetTop();
        Assert.True(positionDiv.GetStyle().GetTop() == "100px");
        Assert.True(positionDiv.GetStyle().GetWidth() == "800px");
    }

    [Fact]
    public void Center()
    {
        var cut = RenderComponent<PositionCenter>();
        cut.Find("button").Click();

        var positionDiv = cut.Find("body div.antd-ext-dialog-root .antd-ext-dialog-position");

        Assert.Contains("antd-ext-dialog-center", positionDiv.ClassList);
    }
}
