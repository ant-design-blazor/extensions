using AngleSharp.Css.Dom;
using AntDesign.Extensions.Samples.Demos;
using Moq;
using System;

namespace AntDesign.Extensions.Components.Dialog.Test;

public class HeaderTest : DialogTestBase
{
    
    public HeaderTest() : base()
    {
    }


    [Fact]
    public void DiableDisplay()
    {
        var cut = RenderComponent<HeaderTitle>();
        cut.Find("button").Click();

        var mainDiv = cut.Find("body > div.antd-ext-dialog-root .antd-ext-dialog-main");
        var dom = mainDiv.QuerySelector(".antd-ext-dialog-header");
        Assert.Null(dom);
    }

    [Fact]
    public void HeaderStyle()
    {
        var cut = RenderComponent<HeaderStyle>();
        cut.Find("button").Click();

        var mainDiv = cut.Find("body div.antd-ext-dialog-root .antd-ext-dialog-main");
        var dom = mainDiv.QuerySelector(".antd-ext-dialog-header");
        Assert.NotNull(dom);

        Assert.True(dom.ClassList.Contains("custom-header-class"));

        var cssText = dom.GetStyle().CssText;
        Assert.Contains("color: rgba(255, 255, 255, 1)", cssText);
    }


    [Fact]
    public void TitleTemplate()
    {
        var cut = RenderComponent<HeaderTemplate>();
        cut.Find("button").Click();

        var mainDiv = cut.Find("body div.antd-ext-dialog-root .antd-ext-dialog-main");
        var dom = mainDiv.QuerySelector(".antd-ext-dialog-header .antd-ext-dialog-title");
        Assert.NotNull(dom);

        // <div class="antd-ext-dialog-title"></div>

        var innerHtml = dom.InnerHtml;
        Assert.Contains("<b>TitleTempalte</b>", innerHtml);
    }
}
