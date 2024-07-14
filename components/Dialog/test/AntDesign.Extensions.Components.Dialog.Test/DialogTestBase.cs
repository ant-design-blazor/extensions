using Microsoft.JSInterop;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntDesign.Extensions.Components.Dialog.Test;
public abstract class DialogTestBase : TestContext
{
    protected readonly Mock<IPortalService> _portalService = new();
    protected readonly Mock<IComponentIdGenerator> _componentIdGeneratorService = new();

    protected DialogTestBase()
    {
        Services.AddScoped(provider => _portalService.Object);
        Services.AddScoped(provider => _componentIdGeneratorService.Object);
    }
}
