using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using OpenUtau.Core;
using OpenUtauMobile.ViewModels.Controls;

namespace OpenUtauMobile.Views.Controls;

public partial class InsertTempoOrTimeSignaturePopup : Popup
{
    public InsertTempoOrTimeSignaturePopup(int position)
	{
		InitializeComponent();
		EntryPosition.Text = position.ToString();
        DocManager.Inst.Project.timeAxis.TickPosToBarBeat(position, out int bar, out _, out _);
        EntryBar.Text = bar.ToString();
    }
	private void OnCancelClicked(object sender, EventArgs e)
	{
		CloseAsync(null);
    }

    private void ButtonInsertTempoSignature_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(EntryPosition.Text, out int position) && double.TryParse(EntryBpm.Text, out double bpm))
        {
            if (position < 0)
            {
                Toast.Make("λ�ò���Ϊ��", CommunityToolkit.Maui.Core.ToastDuration.Short, 16).Show();
                return;
            }
            if (bpm <= 0)
            {
                Toast.Make("BPM����С�ڵ���0", CommunityToolkit.Maui.Core.ToastDuration.Short, 16).Show();
                return;
            }
            CloseAsync(new Tuple<int, double>(position, bpm));
        }
        else
        {
            Toast.Make("��������Ч������", CommunityToolkit.Maui.Core.ToastDuration.Short, 16).Show();
        }
    }

    private void ButtonInsertTimeSignature_Clicked(object sender, EventArgs e)
    {
        if (int.TryParse(EntryBar.Text, out int bar) && int.TryParse(EntryBeatPerBar.Text, out int beatPerBar) && int.TryParse(EntryBeatUnit.Text, out int beatUnit))
        {
            if (bar < 0)
            {
                Toast.Make("С�ڲ���Ϊ��", CommunityToolkit.Maui.Core.ToastDuration.Short, 16).Show();
                return;
            }
            if (beatPerBar <= 0)
            {
                Toast.Make("ÿС����������С�ڵ���0", CommunityToolkit.Maui.Core.ToastDuration.Short, 16).Show();
                return;
            }
            if (beatUnit <= 0)
            {
                Toast.Make("�Լ�������Ϊһ�Ĳ���С�ڵ���0", CommunityToolkit.Maui.Core.ToastDuration.Short, 16).Show();
                return;
            }
            CloseAsync(new Tuple<int, int, int>(bar, beatPerBar, beatUnit));
        }
        else
        {
            Toast.Make("��������Ч������", CommunityToolkit.Maui.Core.ToastDuration.Short, 16).Show();
        }
    }
}