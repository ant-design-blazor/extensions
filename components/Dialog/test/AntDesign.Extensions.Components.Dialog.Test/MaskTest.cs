using AngleSharp.Css.Dom;
using AntDesign.Extensions.Samples.Demos;
using Moq;
using System;

namespace AntDesign.Extensions.Components.Dialog.Test;

public class MaskTest : DialogTestBase
{

    public MaskTest() : base()
    {
    }

    [Fact]
    public void MaskClosable()
    {
        var cut = RenderComponent<ModeModal>();
        cut.Find("button").Click();
        var dom = cut.Find("body > div > div.antd-ext-dialog-root");
        dom = dom.QuerySelector(".antd-ext-dialog-mask");
        dom!.Click();

        var rootDiv = cut.Find("body > div:has(div.antd-ext-dialog-root)");
        var result = rootDiv.GetStyle().CssText.Contains("display: none");
        Assert.True(result);
    }


    [Fact]
    public void DisableMaskClosable()
    {
        var cut = RenderComponent<MaskClosable>();
        cut.Find("button").Click();
        var dom = cut.Find("body > div > div.antd-ext-dialog-root");
        dom = dom.QuerySelector(".antd-ext-dialog-mask");

        var rootDiv = cut.Find("body > div:has(div.antd-ext-dialog-root)");
        var result = rootDiv.GetStyle().CssText.Contains("display: none");
        Assert.False(result);
        // <div class="antd-ext-dialog-mask custom-mask-class" style="background-color: #ffffff;"></div>
    }


    [Fact]
    public void CustomeStyle()
    {
        var cut = RenderComponent<MaskStyle>();
        cut.Find("button").Click();
        var dom = cut.Find("body > div > div.antd-ext-dialog-root");
        dom = dom.QuerySelector(".antd-ext-dialog-mask");

        var cls = dom!.ClassName!.Contains("custom-mask-class");
        Assert.True(cls);

        var result = dom.GetStyle().CssText.Contains("background-color: rgba(255, 255, 255, 1)");
        Assert.True(result);
    }


    [Fact]
    public void DisableMask()
    {
        var cut = RenderComponent<MaskShowMask>();
        cut.Find("button").Click();
        var dom = cut.Find("body > div > div.antd-ext-dialog-root");
        dom = dom.QuerySelector(".antd-ext-dialog-mask");
        Assert.Null(dom);
    }
}
