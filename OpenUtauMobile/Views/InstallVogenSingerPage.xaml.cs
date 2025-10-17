using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using OpenUtau.Core;
using OpenUtauMobile.ViewModels;
using OpenUtauMobile.ViewModels.Messages;
using OpenUtauMobile.Views.Controls;
using Serilog;

namespace OpenUtauMobile.Views;

public partial class InstallVogenSingerPage : ContentPage
{
    private InstallVogenSingerViewModel ViewModel { get; }

    private bool _isExit = false; // �˳���־
    private List<View> StepViews { get; set; } = [];
    private int _currentStep = 0;

    public InstallVogenSingerPage(string installPackagePath)
    {
        InitializeComponent();
        ViewModel = (InstallVogenSingerViewModel)BindingContext;
        ViewModel.InstallPackagePath = installPackagePath;

        InitializeStepViews();
        UpdateStepViews();

    }

    /// <summary>
    /// ���ذ�ť����¼�
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ButtonBack_Clicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }


    /// <summary>
    /// ������ͼ��ӵ��б�
    /// </summary>
    private void InitializeStepViews()
    {
        foreach (var child in MainContentLayout.Children)
        {
            if (child is View view)
            {
                StepViews.Add(view);
            }
        }
    }

    /// <summary>
    /// ���²�����ͼ�Ŀɼ���
    /// </summary>
    private void UpdateStepViews()
    {
        for (int i = 0; i < StepViews.Count; i++)
        {
            StepViews[i].IsVisible = (i == _currentStep);
            StepViews[i].InputTransparent = (i != _currentStep);
        }
    }

    /// <summary>
    /// ���ذ�ť�����¼�
    /// </summary>
    /// <returns></returns>
    protected override bool OnBackButtonPressed()
    {
        if (_currentStep >= 2) // ��װ��ɺ�ֱ�ӷ���
        {
            return base.OnBackButtonPressed();
        }
        if (_isExit)
        {
            Navigation.PopModalAsync();
            Toast.Make("��װ��ȡ����", CommunityToolkit.Maui.Core.ToastDuration.Short, 16).Show();
            return false;
        }
        else
        {
            _isExit = true;

            Task.Run(async () =>
            {
                await Task.Delay(2000); // 2��
                _isExit = false;
            });

            Toast.Make("�ٴβ�����ȡ�����ְ�װ", CommunityToolkit.Maui.Core.ToastDuration.Short, 16).Show(); // toast��ʾ��ʾ
        }

        return true;
    }

    private void ButtonFinish_Clicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }

    private void ButtonInstall_Clicked(object sender, EventArgs e)
    {
        _currentStep++;
        UpdateStepViews();

        // ��������ϵ㵽������ʱ��û�и���ҳ��ɼ��ԣ���װ�����޷�չʾ

        Task.Run(() =>
        {
            try
            {
                ViewModel.Install();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "���ⰲװʧ�ܣ�");
                DocManager.Inst.ExecuteCmd(new ErrorMessageNotification(ex));
            }
        }).ContinueWith(task => MainThread.InvokeOnMainThreadAsync(() =>
        {
            DocManager.Inst.ExecuteCmd(new SingersChangedNotification());
            Toast.Make("��װ��ɣ�", CommunityToolkit.Maui.Core.ToastDuration.Short, 16).Show();
            _currentStep++;
            UpdateStepViews();
        }));
    }
}
