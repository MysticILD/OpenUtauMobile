using CommunityToolkit.Maui.Views;
using OpenUtauMobile.ViewModels.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace OpenUtauMobile.Views.Controls
{
    public partial class FileSaverPopup : Popup
    {
        private readonly FileSaverPopupViewModel _viewModel;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="types">�����ļ����ͣ�����[*.ustx, *.wav]</param>
        /// <param name="initialDirectory">��ʼ����Ŀ¼</param>
        /// <param name="initialFileName">Ĭ���ļ���</param>
        public FileSaverPopup(string[] types, string initialDirectory = "", string initialFileName = "")
        {
            InitializeComponent();
            _viewModel = (FileSaverPopupViewModel)BindingContext;
            _viewModel.Types = types;
            _viewModel.CurrentDirectory = initialDirectory;
            _viewModel.FileName = initialFileName;
            _viewModel.Initialize();
        }

        /// <summary>
        /// UI�����Ŀ�¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnItemSelected(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel.SelectedItem is DirectoryInfo directory) // ѡ�е���Ŀ¼
            {
                _viewModel.CurrentDirectory = directory.FullName;
            }
            else if (_viewModel.SelectedItem is FileInfo file) // ѡ�е����ļ�
            {
                _viewModel.FileName = file.Name;
            }
        }

        /// <summary>
        /// ������һ��Ŀ¼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGoUpClicked(object sender, EventArgs e)
        {
            DirectoryInfo? parentDirectory = Directory.GetParent(_viewModel.CurrentDirectory);
            if (parentDirectory != null)
            {
                _viewModel.CurrentDirectory = parentDirectory.FullName;
            }
        }

        /// <summary>
        /// ȡ����ť����¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCancelClicked(object sender, EventArgs e)
        {
            CloseAsync("");
        }

        /// <summary>
        /// ˢ�µ�ǰĿ¼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRefresh_Clicked(object sender, EventArgs e)
        {
            _viewModel.LoadDirectory(_viewModel.CurrentDirectory);
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSave_Clicked(object sender, EventArgs e)
        {
            // ���ж��ļ����Ƿ����types
            if (_viewModel.Types.Length > 0 && _viewModel.Types[0] != "*")
            {
                bool isValid = false;
                foreach (string type in _viewModel.Types)
                {
                    if (_viewModel.FileName.EndsWith(type.TrimStart('*')))
                    {
                        isValid = true;
                        break;
                    }
                }
                // �������������Ҫ���Զ���ӵ�һ�����ͺ�׺
                if (!isValid)
                {
                    _viewModel.FileName = _viewModel.FileName + (_viewModel.Types[0].StartsWith('*') ? _viewModel.Types[0].TrimStart('*') : _viewModel.Types[0]);
                }
            }
            CloseAsync(Path.Combine(_viewModel.CurrentDirectory, _viewModel.FileName));
        }
    }
}