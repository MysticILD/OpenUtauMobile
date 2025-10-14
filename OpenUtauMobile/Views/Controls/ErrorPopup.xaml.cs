using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using OpenUtauMobile.Utils;

namespace OpenUtauMobile.Views.Controls;

public partial class ErrorPopup : Popup
{
    public ErrorPopup(string errorDetail)
	{
		InitializeComponent();
        LabelErrorDetail.Text = "�ܱ�Ǹ�� OpenUtau Mobile �������쳣�����ֹ��ܿ����޷�����ʹ�á�\n";
        LabelErrorDetail.Text += "�����������´�����Ϣ���Ʋ������������ߣ�������Ӧ�ó���\n\n";
        LabelErrorDetail.Text += errorDetail;
    }

    private async void ButtonCopy_Clicked(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(LabelErrorDetail.Text);
#if !ANDROID33_0_OR_GREATER
        await Toast.Make("�Ѹ��Ƶ�������", CommunityToolkit.Maui.Core.ToastDuration.Short).Show();
#endif
    }

    private async void ButtonClose_Clicked(object sender, EventArgs e)
    {
        await CloseAsync("close");
    }

    private void ButtonRelaunch_Clicked(object sender, EventArgs e)
    {
        ObjectProvider.AppLifeCycleHelper.Restart();
    }
}