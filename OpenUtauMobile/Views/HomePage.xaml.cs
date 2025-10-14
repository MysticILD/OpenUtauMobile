using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using DynamicData.Binding;
using OpenUtauMobile.Views.Controls;
using OpenUtauMobile.Resources.Strings;
using OpenUtauMobile.Utils.Permission;
using OpenUtauMobile.ViewModels;
using System.Threading.Tasks;
using OpenUtau.Core;
using OpenUtauMobile.Utils;

namespace OpenUtauMobile.Views;

public partial class HomePage : ContentPage
{
    private HomePageViewModel _viewModel;
    private bool _isExit = false; // �˳���־
    public HomePage()
	{
		InitializeComponent();
        _viewModel = (HomePageViewModel)BindingContext;
    }

    /// <summary>
    /// ��д���ؼ������¼���ʵ���ٴΰ��·��ؼ��˳�Ӧ��
    /// </summary>
    /// <returns></returns>
    protected override bool OnBackButtonPressed()
    {
        if (_isExit)
        {
            Application.Current?.Quit();
        }
        else
        {
            _isExit = true;

            Task.Run(async () =>
            {
                await Task.Delay(2000); // 2��
                _isExit = false;
            });

            Toast.Make(AppResources.StringPressBackAgainToExit, CommunityToolkit.Maui.Core.ToastDuration.Short, 16).Show(); // toast��ʾ��ʾ
        }

        return true;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.RecentProjectsPaths = new ObservableCollectionExtended<string>(OpenUtau.Core.Util.Preferences.Default.RecentFiles);
    }


    private async void ButtonNewProject_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new EditPage(string.Empty));
    }

    private async void ButtonOpenProject_Clicked(object sender, EventArgs e)
    {
        string projectPath = await ObjectProvider.PickFile([".ustx"], this);
        if (!string.IsNullOrEmpty(projectPath))
        {
            await Navigation.PushModalAsync(new EditPage(projectPath));
        }
    }

    private void ButtonOpenSingerManage_Clicked(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new SingerManagePage()); // ��ת�����ֹ���ҳ��
    }

    private void ButtonOpenOptions_Clicked(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new OptionsPage()); // ��ת��ѡ��ҳ��
    }

    /// <summary>
    /// ������Ĺ���
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ButtonOpenRecent_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is string projectPath)
        {
            if (string.IsNullOrEmpty(projectPath))
            {
                return;
            }
            else if (!projectPath.EndsWith(".ustx"))
            {
                Toast.Make("��ѡ����ȷ�Ĺ����ļ�", CommunityToolkit.Maui.Core.ToastDuration.Short, 16).Show();
                return;
            }
            else
            {
                Navigation.PushModalAsync(new EditPage(projectPath));
            }
        }
    }
}