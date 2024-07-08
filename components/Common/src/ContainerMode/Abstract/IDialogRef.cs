
namespace AntDesign.Extensions;

public interface IDialogRef
{
    IDialogRef Open();
    IDialogRef AutoRemoveOnLocationChanged();

    void Close();
    void Destroy();
}
