using AngleSharp.Css.Dom;
using AntDesign.Extensions.Samples.Demos;
using Moq;
using System;

namespace AntDesign.Extensions.Components.Dialog.Test;

public class DialogModeTest : DialogTestBase
{

    public DialogModeTest():base()
    {

    }

    [Fact]
    public void Modal()
    {
        var cut = RenderComponent<ModeModal>();
        // open button
        cut.Find("button").Click();

        var dom = cut.Find("body > div > div.antd-ext-dialog-root");
        dom.QuerySelector(".antd-ext-dialog-mask");
        Assert.NotNull(dom);
    }

    [Fact]
    public void Dilaog()
    {
        var cut = RenderComponent<ModeDialog>();
        // open button
        cut.Find("button").Click();

        var dom = cut.Find("body > div > div.antd-ext-dialog-root");
        dom = dom.QuerySelector(".antd-ext-dialog-mask");
        Assert.Null(dom);
    }

    [Fact]
    public void Normal()
    {
        var cut = RenderComponent<ModeNormal>();
        // open button
        cut.Find("button").Click();

        Assert.Throws<ElementNotFoundException>(() =>
        {
            cut.Find("body > div:has(div.antd-ext-dialog-root)");
        });
        Assert.Throws<ElementNotFoundException>(() =>
        {
            cut.Find(".antd-ext-dialog-mask");
        });

        cut.Find("div.antd-ext-dialog-main");
    }
}
