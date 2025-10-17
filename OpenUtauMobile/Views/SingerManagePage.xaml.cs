using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using OpenUtauMobile.Views.Controls;
using OpenUtauMobile.Utils;
using OpenUtauMobile.Utils.Permission;
using OpenUtauMobile.ViewModels;
using OpenUtau.Core.Ustx;

namespace OpenUtauMobile.Views;

public partial class SingerManagePage : ContentPage
{
    private SingerManageViewModel ViewModel { get; }
    public SingerManagePage()
    {
        InitializeComponent();
        ViewModel = (SingerManageViewModel)BindingContext;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.RefreshSingers(); // ˢ�¸����б�
    }

    /// <summary>
    /// ��ť�¼�-����
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ButtonBack_Clicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync(); // ������һҳ
    }

    /// <summary>
    /// ��ť�¼�-��Ӹ���
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ButtonAddSinger_Clicked(object sender, EventArgs e)
    {
        string installPackagePath = await ObjectProvider.PickFile([".zip", ".rar", ".uar", ".vogeon"], this);
        if (!string.IsNullOrEmpty(installPackagePath))
        {
            if (installPackagePath.EndsWith(".vogeon"))
            {
                await Navigation.PushModalAsync(new InstallVogenSingerPage(installPackagePath));
            }
            else
            {
                await Navigation.PushModalAsync(new InstallSingerPage(installPackagePath));
            }
        }
    }

    private void ButtonOpenDetail_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is USinger singer)
        {
            Navigation.PushModalAsync(new SingerDetailPage(singer)); // ��ת����������ҳ��
        }
    }
}
