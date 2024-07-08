using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntDesign.Extensions;
public abstract class AntxDomComponentBase: AntxComponentBase
{
    protected Lazy<string> Id = new(IdUtil.GenId);

    [Parameter]
    public ClassBuilder? Class { get; set; }

    [Parameter]
    public StyleBuilder? Style { get; set; }

    private readonly ClassBuilder _class = new();
    private readonly StyleBuilder _stye = new();

    protected ClassBuilder _classBuilder => _class;
    protected StyleBuilder _styleBuilder => _stye;
}
