using AngleSharp.Css.Dom;
using AntDesign.Extensions.Samples.Demos;
using Moq;
using System;

namespace AntDesign.Extensions.Components.Dialog.Test;

public class ContentTest : DialogTestBase
{
    
    public ContentTest() : base()
    {
    }



    [Fact]
    public void ContentStyle()
    {
        var cut = RenderComponent<ContentStyle>();
        cut.Find("button").Click();

        var mainDiv = cut.Find("body div.antd-ext-dialog-root .antd-ext-dialog-main");
        var dom = mainDiv.QuerySelector(".antd-ext-dialog-content");
        Assert.NotNull(dom);

        Assert.True(dom.ClassList.Contains("custom-content-class"));

        var cssText = dom.GetStyle().CssText;
        Assert.Contains("color: rgba(255, 255, 255, 1)", cssText);
    }
}
