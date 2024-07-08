using Microsoft.AspNetCore.Components;
using OneOf;

namespace AntDesign.Extensions;
public interface IPortalService
{
    Task<IAsyncDisposable> CreateAsync(ElementReference eleRef, OneOf<string, ElementReference> container);
    Task DeleteAsync(ElementReference eleRef);
}