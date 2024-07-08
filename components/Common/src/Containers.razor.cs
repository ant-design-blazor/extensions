using Microsoft.AspNetCore.Components;

namespace AntDesign.Extensions;
public partial class Containers
{
    [Parameter]
    public List<Type> Components { get; set; } = new List<Type>();

    public void Accept<T>() where T : ComponentBase
    {
        this.Components.Add(typeof(T));
        StateHasChanged();
    }
}
