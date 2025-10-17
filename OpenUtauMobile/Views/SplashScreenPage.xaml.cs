using CommunityToolkit.Maui.Views;
using OpenUtau.Audio;
using OpenUtau.Classic;
using OpenUtau.Core;
using OpenUtau.Core.Util;
using OpenUtauMobile.Utils;
using OpenUtauMobile.Views.Controls;
using OpenUtauMobile.Views.Utils;
using Serilog;
using System.Diagnostics;
using System.Globalization;
using Preferences = OpenUtau.Core.Util.Preferences;

namespace OpenUtauMobile.Views;

public partial class SplashScreenPage : ContentPage, ICmdSubscriber
{
    public SplashScreenPage()
	{
		InitializeComponent();
        string version = AppInfo.VersionString;
        LabelDisplayVersion.Text = $"Version {version}";
        DocManager.Inst.AddSubscriber(this); // ����DocManager������
        //CheckPermission(); // ���Ȩ��
        InitApp(); // ��ʼ��OpenUtau���
    }


    /// <summary>
    /// ��ʼ��OpenUtau���
    /// </summary>
    public void InitApp()
    {
        var mainThread = Thread.CurrentThread;
        var mainScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        Task.Run(async () => // �첽��ʼ��
        {
            Log.Information("==========��ʼ��ʼ��OpenUtau���==========");
            try
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    LabelInitDetail.Text = "��ʼ�������ṩ��";
                });
                // throw new Exception("�����쳣");
                ObjectProvider.Initialize(); // ��ʼ�������ṩ��
                Log.Information("�����ṩ����ʼ�����");

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    LabelInitDetail.Text = "��ʼ��ToolsManager";
                });
                ToolsManager.Inst.Initialize(); // ��ʼ��ToolsManager
                Log.Information("ToolsManager��ʼ�����");

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    LabelInitDetail.Text = "��ʼ��SingerManager";
                });
                SingerManager.Inst.Initialize(); // ��ʼ��SingerManager
                Log.Information("SingerManager��ʼ�����");

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    LabelInitDetail.Text = "��ʼ��DocManager";
                });
                DocManager.Inst.Initialize(mainThread, mainScheduler); // ��ʼ��DocManager
                DocManager.Inst.PostOnUIThread = action => MainThread.BeginInvokeOnMainThread(action); // ����DocManager��PostOnUIThread
                Log.Information("DocManager��ʼ�����");

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    LabelInitDetail.Text = "��ʼ��PlaybackManager";
                });
                PlaybackManager.Inst.AudioOutput = ObjectProvider.AudioOutput?? new DummyAudioOutput(); // ����PlaybackManager��AudioOutput
                Log.Information("PlaybackManager��ʼ�����");

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    LabelInitDetail.Text = "��ʼ�����پ���";
                });
                ViewConstants.PianoKeys = [.. Enumerable.Range(0, ViewConstants.TotalPianoKeys).Reverse().Select(n => new PianoKey(n))];
                Log.Information("���پ�����ʼ�����");

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    LabelInitDetail.Text = "��ʼ�����Ա��ػ�";
                });
                string lang = Preferences.Default.Language;
                if (string.IsNullOrEmpty(lang))
                {
                    lang = CultureInfo.CurrentCulture.Name; // ��ȡϵͳ����
                }
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(Preferences.Default.Language);
                CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(Preferences.Default.Language);
                Log.Information($"����ƫ�ã�{Preferences.Default.Language}");
                Log.Information("���Ա��ػ���ʼ�����");

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    LabelInitDetail.Text = "��ʼ����ϣ�";
                });
            }
            catch (Exception e)
            {
                Log.Error($"OpenUtau��˳�ʼ��ʧ��: {e}");
                // ��ʼ��ʧ�ܵ���
                var popup = new ErrorPopup(e, "OpenUtau��˳�ʼ��ʧ��");
                object? result = await MainThread.InvokeOnMainThreadAsync(() => this.ShowPopupAsync(popup));

                if (result is string id)
                {
                    if (id == "exit")
                    {
                        Log.Information("�˳�Ӧ��");
                        Application.Current?.Quit();
                    }
                }
            }
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Navigation.PushModalAsync(new HomePage(), false); // ģʽ��������ҳ
            });
            Log.Information("==========OpenUtau��˳�ʼ�����==========");
        });
    }

    public async void CheckPermission()
    {
        if (DeviceInfo.Current.Platform == DevicePlatform.Android) // Android
        {
            PermissionStatus storage_read_status = await Permissions.CheckStatusAsync<Permissions.StorageRead>(); // ���洢��ȡȨ��
            if (true)
            {
                storage_read_status = await Permissions.RequestAsync<Permissions.StorageRead>(); // ����洢��ȡȨ��
                if (storage_read_status != PermissionStatus.Granted)
                {
                    Log.Error("û�д洢Ȩ��");
                    throw new Exception("û�д洢Ȩ��");
                }
            }
        }
    }

    public void OnNext(UCommand cmd, bool isUndo)
    {
        if (cmd is ErrorMessageNotification errorCmd)
        {
            Debug.WriteLine(errorCmd.e?.ToString());
            Popup popup = new ErrorPopup(errorCmd.e, errorCmd.message);
            MainThread.BeginInvokeOnMainThread(async () => await this.ShowPopupAsync(popup));
        }
    }
}