---
name: caliburn-micro-wpf
description: Use when creating or modifying WPF Views, ViewModels, bindings, actions, or bootstrapper code with Caliburn.Micro. Applies Caliburn.Micro naming and binding conventions instead of manual bindings or code-behind.
---

# Caliburn.Micro WPF Conventions

Source: https://caliburnmicro.com/documentation/conventions

## 1. View / ViewModel resolution (ViewModel-First)
- Name ViewModels `XxxViewModel` and Views `XxxView`. The locator removes "Model" from the full type name.
  - `ViewModels.CustomerViewModel` -> `Views.CustomerView`
  - `CustomerManagement.CustomerViewModel` -> `CustomerManagement.CustomerView`
- Keep the View in a namespace that matches the transformation (ViewModels -> Views, or same feature namespace).
- Multiple views for one ViewModel use a context (`View.Context` / `WindowManager` context). `ViewModels.CustomerViewModel` with context "Master" -> `Views.Customer.Master`.
- `ViewLocator` calls `InitializeComponent` on located views. Code-behind is not required.
- Views and assemblies must be discoverable: override `SelectAssemblies()` in the bootstrapper when Views live in other assemblies.
- If a ViewModel implements `IViewAware`, its cached view is used before locating a new one.
- Locators are replaceable `Func`s (`ViewLocator.LocateForModelType`, `NameTransformer` rules); customize by replacing or wrapping them.
- Used by: the Bootstrapper (shell), `WindowManager` (dialogs/windows), and the `View.Model` attached property (composition).
- View-First: `ViewModelLocator` changes "View" -> "ViewModel" (also tries the `IXxxViewModel` interface name) and resolves from IoC. Only used when the View's DataContext is null.

## 2. ViewModelBinder
`ViewModelBinder.Bind` runs for every View/ViewModel pair:
1. Sets `Action.Target` and `DataContext` to the ViewModel.
2. Passes the View to the ViewModel if it is `IViewAware`.
3. Creates conventional property bindings and actions by matching **named** elements (`x:Name`) to ViewModel members.

Do not add a manual `{Binding}` or `Click` handler when a convention covers it.

## 3. Action conventions
- Element `x:Name` equal to a ViewModel method name -> the element's default event calls that method.
  ```xml
  <Button x:Name="Save" Content="Save" />
  ```
  ```csharp
  public void Save() { }
  public Task SaveAsync() { ... }   // async methods are supported (Task return)
  ```
- Method parameters can be bound by name from other elements: `<Button x:Name="Login" />` with `Login(string username, string password)` binds to elements named `Username` and `Password`.
- Guard methods: a `CanSave` property (or `bool CanSave(...)` method) enables/disables the control. Call `NotifyOfPropertyChange(() => CanSave)` when its inputs change.
- Explicit syntax when needed: `cal:Message.Attach="[Event Click] = [Action Save($dataContext)]"`.
  - Special values: `$eventArgs`, `$dataContext`, `$source`, `$view`, `$executionContext`, `$this`.
- Default events: Button -> Click, TextBox -> TextChanged, CheckBox -> Click, Selector -> SelectionChanged, etc.

## 4. Property binding conventions
- Element `x:Name` equal to a ViewModel property name -> binding on the element's default property.
- Deep paths use underscores: `x:Name="Customer_FirstName"` binds `Customer.FirstName`.
- Existing explicit bindings are never overwritten.
- Common defaults (ElementConvention):
  - `TextBox` -> `Text`
  - `TextBlock`, `Label`, `ContentControl` -> `Text` / `Content`
  - `CheckBox` / `ToggleButton` -> `IsChecked`
  - `Slider`, `ProgressBar` -> `Value`
  - `ItemsControl`, `ListBox`, `ComboBox`, `ListView`, `DataGrid` -> `ItemsSource`
  - `Selector` -> `SelectedItem` by convention (see below)
  - `Image` -> `Source`
  - `Expander` -> `IsExpanded`
- Visibility: a `bool` property bound to the `Visibility` of an element is converted via `BooleanToVisibilityConverter`.
  Use `Visibility` conventions by naming the control and having a bool property (for example `IsVisible` bound through `x:Name`) or a `Visibility` binding with `{Binding ..., Converter=...}` when more control is needed.
- Validation is surfaced when the ViewModel implements `IDataErrorInfo` / `INotifyDataErrorInfo`.
- Brings `cal:Bind.Model`-style composition only through `View.Model`.

### ItemsControl
- Binding `ItemsSource` also sets a default `ItemTemplate` if none of `ItemTemplate`, `DisplayMemberPath`, `ItemTemplateSelector` is set:
  ```xml
  <ContentControl cal:View.Model="{Binding}" VerticalContentAlignment="Stretch" HorizontalContentAlignment="Stretch" />
  ```
  Each item is rendered through ViewModel-First composition (an `ItemViewModel` needs an `ItemView`).

### Selector (ListBox, ComboBox...)
- With `x:Name="Items"` the singularized name `Item` is used to look for, in order: `ActiveItem`, `SelectedItem`, `CurrentItem`. The first one found is bound to `SelectedItem`.

### TabControl
- Same as Selector, but sets `ContentTemplate` to the default template.
- If the ViewModel implements `IHaveDisplayName` and `DisplayMemberPath` is not set, the tab header uses `{Binding DisplayName}`.
- One line is enough: `<TabControl x:Name="Items" />` on a `Conductor<IScreen>.Collection.OneActive`.

### ContentControl
- With no `ContentTemplate` / `ContentTemplateSelector`, the convention binds `View.Model`, so the bound ViewModel is rendered using its View.
  ```xml
  <ContentControl x:Name="ActiveItem" />
  ```

## 5. Screens and composition
- Derive from `Screen` (has `DisplayName`, lifecycle `OnActivateAsync`, `OnDeactivateAsync`, `CanCloseAsync`), `PropertyChangedBase` for plain models, `Conductor<T>` for hosting.
- Use `Set(ref field, value)` and `NotifyOfPropertyChange(() => Prop)` for property changes.
- Use `View.Model="{Binding SomeVm}"` or `<ContentControl x:Name="SomeVm" />` for composition.
- Show windows with `IWindowManager.ShowWindowAsync / ShowDialogAsync / ShowPopupAsync`.

## 6. Bootstrapper (WPF, Caliburn.Micro 4.x)
```csharp
public class AppBootstrapper : BootstrapperBase
{
	public AppBootstrapper() => Initialize();

	protected override async void OnStartup(object sender, StartupEventArgs e)
		=> await DisplayRootViewForAsync<ShellViewModel>();
}
```
- Register in `App.xaml` resources:
  ```xml
  <Application.Resources>
	  <ResourceDictionary>
		  <ResourceDictionary.MergedDictionaries>
			  <ResourceDictionary>
				  <local:AppBootstrapper x:Key="bootstrapper" />
			  </ResourceDictionary>
		  </ResourceDictionary.MergedDictionaries>
	  </ResourceDictionary>
  </Application.Resources>
  ```
- Remove `StartupUri` from `App.xaml`.
- Override `Configure`, `GetInstance`, `GetAllInstances`, `BuildUp` to integrate an IoC container (e.g. Microsoft.Extensions.DependencyInjection) and `SelectAssemblies` for multi-assembly Views.

## 7. ConventionManager customization
- Add a convention for a third-party control:
  ```csharp
  ConventionManager.AddElementConvention<MyControl>(MyControl.ValueProperty, "Value", "ValueChanged");
  ```
- Customize: `ConventionManager.GetElementConvention(typeof(T))`, then change `ApplyBinding`, `GetBindableProperty`, `CreateTrigger`, `ParameterProperty`.
- Replace `ViewModelBinder.BindProperties` / `BindActions` (Funcs) or set `ViewModelBinder.ApplyConventionsByDefault = false` to disable conventions. Bind a single View explicitly with `Bind.Model` / `Bind.ModelWithoutContext`.
- Naming rules can be extended through `ViewLocator.NameTransformer.AddRule(...)` (regex based). `ViewLocator.ConfigureTypeMappings(...)` handles custom suffixes.
- Use `ConventionManager.Singularize` and `DerivePotentialSelectionNames` to change the selection property naming.

## Rules for the assistant
1. Prefer conventions: name elements with `x:Name` matching ViewModel members instead of writing `{Binding}`, `Click=` handlers, or code-behind.
2. Respect the `XxxViewModel` / `XxxView` pairing and keep them in matching namespaces.
3. Use `CanXxx` for enabling actions, never manual `IsEnabled` handling.
4. Do not fight the framework: only add explicit bindings when a convention doesn't apply, since explicit bindings are preserved.
5. When a View is not found, the "{type} not found." TextBlock shows the expected name; fix the naming or namespace.
6. Keep ViewModels free of View/UI types; use `IViewAware` only when truly needed.
