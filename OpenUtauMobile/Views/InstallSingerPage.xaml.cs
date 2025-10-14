using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using OpenUtau.Core;
using OpenUtauMobile.ViewModels;
using OpenUtauMobile.ViewModels.Messages;
using OpenUtauMobile.Views.Controls;
using Serilog;

namespace OpenUtauMobile.Views;

public partial class InstallSingerPage : ContentPage
{
    private InstallSingerViewModel _viewModel;

    private bool _isExit = false; // �˳���־
    private List<View> StepViews { get; set; } = [];
    private int _currentStep = 0;

    public InstallSingerPage(string installPackagePath)
    {
        InitializeComponent();
        _viewModel = (InstallSingerViewModel)BindingContext;
        _viewModel.InstallPackagePath = installPackagePath;

        InitializeStepViews();
        UpdateStepViews();
        UpdateStepButton();

        //WeakReferenceMessenger.Default.Register<SimpleMessage>(this, async (sender, message) =>
        //{
        //    var popup = new SimplePopup("����", message.Value, new Dictionary<string, string> { { "ok", "ȷ��" } });
        //    object? result = await MainThread.InvokeOnMainThreadAsync(() => this.ShowPopupAsync(popup));

        //    if (result is string action && action == "ok")
        //    {
        //        await Navigation.PopModalAsync();
        //    }
        //});
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Task.Run(() => { _viewModel.Init(); }); // �����
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
    /// ��һ����ť����¼�
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ButtonBackStep_Clicked(object sender, EventArgs e)
    {
        if (_currentStep > 0)
        {
            _currentStep--;
            if (_currentStep == 2 && !_viewModel.MissingInfo)
            {
                _currentStep--;
            }
            UpdateStepViews();
            UpdateStepButton();
        }
    }

    /// <summary>
    /// ��һ����ť����¼�
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ButtonNextStep_Clicked(object sender, EventArgs e)
    {
        if (_currentStep < StepViews.Count - 1)
        {
            _currentStep++;
            if (_currentStep == 2 && !_viewModel.MissingInfo)
            {
                _currentStep++;
            }
            UpdateStepViews();
            UpdateStepButton();
        }
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

    private void UpdateStepButton()
    {
        // ��һ����ť�ɼ���
        if (_currentStep >= 1 && _currentStep <= 3)
        {
            ButtonBackStep.IsVisible = true;
        }
        else
        {
            ButtonBackStep.IsVisible = false;
        }
        // ��һ����ť�ɼ���
        if (_currentStep <= 2)
        {
            ButtonNextStep.IsVisible = true;
        }
        else
        {
            ButtonNextStep.IsVisible = false;
        }
    }

    /// <summary>
    /// ���ذ�ť�����¼�
    /// </summary>
    /// <returns></returns>
    protected override bool OnBackButtonPressed()
    {
        if (_currentStep >= 5) // ��װ��ɺ�ֱ�ӷ���
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

    private void ArchiveEncodingListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_viewModel == null)
        {
            return;
        }
        Task.Run(() => { _viewModel.RefreshArchiveItems(); });
    }

    private void TextEncodingListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_viewModel == null)
        {
            return;
        }
        Task.Run(() => { _viewModel.RefreshTextItems(); });
    }

    /// <summary>
    /// ��ʼ��װ��ť����¼�
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ButtonBeginInstall_Clicked(object sender, EventArgs e)
    {
        _currentStep++;
        UpdateStepViews();
        UpdateStepButton();

        Task.Run(() =>
        {
            try
            {
                _viewModel.Install();
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
            UpdateStepButton();
        }));
    }

    private void ButtonFinish_Clicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }
}
