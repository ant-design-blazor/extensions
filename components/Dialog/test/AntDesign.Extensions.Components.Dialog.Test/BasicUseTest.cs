using AngleSharp.Css.Dom;
using AntDesign.Extensions.Samples.Demos;
using Microsoft.JSInterop;
using Moq;
using System;

namespace AntDesign.Extensions.Components.Dialog.Test;

public class BasicUseTest : DialogTestBase
{
    public BasicUseTest() : base()
    {
    }


    [Fact]
    public void Visible()
    {
        var cut = RenderComponent<ModeDialog>();
        Assert.Throws<ElementNotFoundException>(() =>
        {
            cut.Find("body > div:has(div.antd-ext-dialog-root)");
        });

        // open button
        cut.Find("button").Click();

        var root = cut.Find("body div.antd-ext-dialog-root");
        Assert.NotNull(root);

        var positionDiv = root!.QuerySelector(".antd-ext-dialog-position");
        Assert.True(positionDiv.GetStyle().GetWidth() == "520px");


        // <button blazor:onclick="6" class="ant-btn ant-btn-primary">OK</button>
        // antd-ext-dialog-main antd-ext-dialog-mode-normal
        var okBtn = cut.Find(".antd-ext-dialog-footer button.ant-btn.ant-btn-primary");
        okBtn.Click();
        var rootDiv = cut.Find("body > div:has(div.antd-ext-dialog-root)");
        var result = rootDiv.GetStyle().CssText.Contains("display: none");
        Assert.True(result);
    }

    [Fact]
    public void DestroyOnClose()
    {
        var cut = RenderComponent<DestroyOnClose>();
        // open button
        cut.Find("button").Click();
        // <button blazor:onclick="6" class="ant-btn ant-btn-primary">OK</button>
        // antd-ext-dialog-main antd-ext-dialog-mode-normal
        var okBtn = cut.Find(".antd-ext-dialog-footer button.ant-btn.ant-btn-primary");
        okBtn.Click();

        Assert.Throws<ElementNotFoundException>(() =>
        {
            cut.Find("body > div:has(div.antd-ext-dialog-root)");
        });
    }

}
