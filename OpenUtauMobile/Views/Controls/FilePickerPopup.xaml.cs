using CommunityToolkit.Maui.Views;
using OpenUtauMobile.ViewModels.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace OpenUtauMobile.Views.Controls
{
    public partial class FilePickerPopup : Popup
    {
        private FilePickerPopupViewModel _viewModel;

        public FilePickerPopup(string[] types, string initialDirectory = "", string initialFileName = "")
        {
            InitializeComponent();
            _viewModel = (FilePickerPopupViewModel)BindingContext;
            _viewModel.Types = types;
            _viewModel.CurrentDirectory = initialDirectory;
            _viewModel.FileName = initialFileName;
            _viewModel.Initialize();
        }


        /// <summary>
        /// ѡ���ļ���Ŀ¼
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
                CloseAsync(file.FullName);
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

        private void OnCancelClicked(object sender, EventArgs e)
        {
            CloseAsync("");
        }

        private void ButtonRefresh_Clicked(object sender, EventArgs e)
        {
            _viewModel.LoadDirectory(_viewModel.CurrentDirectory);
        }
    }
}