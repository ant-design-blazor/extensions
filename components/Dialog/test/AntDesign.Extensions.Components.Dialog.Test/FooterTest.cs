using AngleSharp.Css.Dom;
using AntDesign.Extensions.Samples.Demos;
using Moq;
using System;
using System.Linq;

namespace AntDesign.Extensions.Components.Dialog.Test;

public class FooterTest : DialogTestBase
{
    
    public FooterTest() : base()
    {
    }


    [Fact]
    public void FooterNull()
    {
        var cut = RenderComponent<FooterNull>();
        cut.Find("button").Click();

        var mainDiv = cut.Find("body div.antd-ext-dialog-root .antd-ext-dialog-main");
        var dom = mainDiv.QuerySelector(".antd-ext-dialog-footer");
        Assert.Null(dom);
    }

    [Fact]
    public void FooterCustom1()
    {
        var cut = RenderComponent<FooterCustom>();
        var btns = cut.FindAll("button").ToList();
        btns[0].Click();

        var mainDiv = cut.Find("body div.antd-ext-dialog-root .antd-ext-dialog-main");
        var dom = mainDiv.QuerySelector(".antd-ext-dialog-footer > div");
        Assert.NotNull(dom);

        var buttons = dom.QuerySelectorAll("button").ToList();
        Assert.True(buttons.Count == 1);

        buttons[0].Click();
        var rootDiv = cut.Find("body > div:has(div.antd-ext-dialog-root)");
        var result = rootDiv.GetStyle().CssText.Contains("display: none");
        Assert.True(result);
    }


    [Fact]
    public void FooterCustom2()
    {
        var cut = RenderComponent<FooterCustom>();
        var btns = cut.FindAll("button").ToList();
        btns[1].Click();

        var mainDiv = cut.Find("body div.antd-ext-dialog-root .antd-ext-dialog-main");
        var dom = mainDiv.QuerySelector(".antd-ext-dialog-footer > div");
        Assert.NotNull(dom);

        var buttons = dom.QuerySelectorAll("button").ToList();
        Assert.True(buttons.Count == 3);

        foreach (var btn in buttons)
        {
            btn.Click();
            var rootDiv = cut.Find("body > div:has(div.antd-ext-dialog-root)");
            var result = rootDiv.GetStyle().CssText.Contains("display: none");
            Assert.True(result);
            btns[1].Click();
        }
    }


    [Fact]
    public void FooterStyle()
    {
        var cut = RenderComponent<FooterStyle>();
        cut.Find("button").Click();

        var mainDiv = cut.Find("body div.antd-ext-dialog-root .antd-ext-dialog-main");
        var dom = mainDiv.QuerySelector(".antd-ext-dialog-footer");
        Assert.NotNull(dom);

        Assert.True(dom.ClassList.Contains("custom-footer-class"));

        var cssText = dom.GetStyle().CssText;
        Assert.Contains("color: rgba(255, 255, 255, 1)", cssText);
    }
}
