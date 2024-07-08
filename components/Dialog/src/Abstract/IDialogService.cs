namespace AntDesign.Extensions;

public interface IDialogService
{
    IDialogRef Create(DialogOptions dialogOptions);
    IDialogRef CreateAndOpen(DialogOptions dialogOptions);
}