# Dialogs

`Shared/Dialogs/` wraps SampSharp's dialogs, so every menu looks the same: `BetterListDialog`, `BetterTablistDialog`, `BetterInputDialog` and `BetterMessageDialog`.

## Showing a menu

```csharp
BetterListDialog dialog = new BetterListDialog("Choose", "Back");
dialog.SetTitle(TitleType.Children, "Properties", property.GetName());
int renameRow = dialog.AddRow("Rename");

dialogService.Show(player.Entity, dialog, response =>
{
    if (response.Response != DialogResponse.LeftButton)
        onBack();
    else if (response.ItemIndex == renameRow)
        OpenRename(player, property, reopen);
});
```

## Rules

- **Rows:** match responses by the index `AddRow` returns. **Why:** counting rows by hand breaks as soon as a row is optional.
- **Back buttons:** give every menu a callback that reopens the menu it came from.
- **Flows:** a dialog service with several flows is a `partial class`, with one file per flow.
- **Plain characters only:** `->`, not `→`. **Why:** SA-MP dialogs use the Windows-1252 encoding.

## Deep dive

- **Titles** always start with "Open Roleplay | ".
  - `TitleType.Children` joins the parts with ` -> `: a breadcrumb.
  - `TitleType.Parents` joins them with ` | `.
- **Colours:** every row and column starts in white, so one row's colour can't bleed into the next.
- **`DialogHelper`** has the standard button texts (`Yes`, `No`, `Cancel`, …) and `GetBooleanAsOnOrOff`.
- **Examples:** `FactionDialogService.cs` and `PropertyDialogService.cs` show the shared helpers (`ShowMessage`, `ShowConfirmation`, `OpenNameInput`).

Want more? → [Architecture](../architecture.md)
