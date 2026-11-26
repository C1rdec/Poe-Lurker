---
description: Refresh the item filter completion data (Class names and BaseType names) from repoe-fork/poe2.
---

# Update item filter completion data

Refresh the embedded lists used by the item filter editor completion:

- `src/PoeLurker.UI/Assets/Filter/ItemClasses.txt` (values for `Class`)
- `src/PoeLurker.UI/Assets/Filter/BaseTypes.txt` (values for `BaseType`)

Both files are embedded resources read by `src/PoeLurker.UI/Controls/ItemFilterCompletionProvider.cs`. Nothing is downloaded at runtime.

## Source

Raw files from https://github.com/repoe-fork/poe2 (branch `master`):

- `data/item_classes.json`: object keyed by class id. Use each entry's `name` (for example "Life Flasks").
- `data/base_items.json`: object keyed by metadata path. Use each entry's `name`, only when `release_state` is `released`.

## Steps

1. Download both JSON files with `Invoke-WebRequest` from `https://raw.githubusercontent.com/repoe-fork/poe2/master/data/` into the temp folder.
2. Build `ItemClasses.txt`: all non-empty `name` values, sorted and unique, one per line.
3. Build `BaseTypes.txt`: all non-empty `name` values where `release_state` is `released`, sorted and unique, one per line.
4. Remove placeholder entries from `BaseTypes.txt`: any line starting with `[` (for example `[DNT] ...` and `[DNT-UNUSED] ...`).
5. Write both files as UTF-8 without BOM, overwriting the existing ones. Do not change `PoeLurker.UI.csproj`; the files are already embedded.
6. Report the new line counts and a short diff summary (entries added and removed compared to the previous files, using `git diff --stat`).
7. Build `src/PoeLurker.UI/PoeLurker.UI.csproj` and confirm it succeeds.

## Also check (only if asked)

The condition, action and block names in `ItemFilterCompletionProvider.cs` (`Blocks`, `Conditions`, `Actions`) come from https://www.pathofexile.com/item-filter/about. Compare them with that page and report any differences without editing unless asked. Ignore the "New" and "Updated" badges shown on the page.
