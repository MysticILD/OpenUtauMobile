using CommunityToolkit.Maui.Alerts;
using OpenUtau.Core;
using OpenUtau.Core.Ustx;
using OpenUtauMobile.ViewModels;

namespace OpenUtauMobile.Views;

public partial class SingerDetailPage : ContentPage
{
    private SingerDetailViewModel ViewModel { get; }
    public SingerDetailPage(USinger singer)
	{
		InitializeComponent();
        ViewModel = (SingerDetailViewModel)BindingContext;
        ViewModel.Init(singer);
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

    private async void ButtonRemoveSinger_Clicked(object sender, EventArgs e)
    {
        if (await this.DisplayAlert("ж�ظ���", $"ȷ��Ҫж�ظ��� {ViewModel.Singer.LocalizedName} ��", "��", "��"))
        {
            await Toast.Make($"����ж�ظ��� {ViewModel.Singer.LocalizedName}...").Show();
            if (await SingerManager.Inst.UninstallSingerAsync(ViewModel.Singer))
            {
                await Toast.Make($"�ɹ�ж�ظ��� {ViewModel.Singer.LocalizedName}").Show();
            }
            else
            {
                await Toast.Make($"ж�ظ��� {ViewModel.Singer.LocalizedName} ʧ��").Show();
            }
            await Navigation.PopModalAsync(); // ������һҳ
        }
    }
}