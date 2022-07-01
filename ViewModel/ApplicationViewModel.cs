using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using fanuc_group_exchange_desktop.Model;
using fanuc_group_exchange_desktop.Command;
using fanuc_group_exchange_desktop.Services;

namespace fanuc_group_exchange_desktop.ViewModel
{
    public class ApplicationViewModel : BaseViewModel
    {
        private readonly FileService FileService;
        private readonly RobotProgramService _RobotProgramService;
        private string filePath;
        private RobotProgram Program;

        public ApplicationViewModel(Window window)
        {
            FileService = new FileService();
            _RobotProgramService = new RobotProgramService();

            ProgramText = App.placeholderText;
            AddingGroupViewModels = new ObservableCollection<GroupViewModel>
            {
                new GroupViewModel(new RobotGroup(0,1), this)
            };
        }


        private string _ProgramText;
        public string ProgramText
        {
            set
            {
                _ProgramText = value;
                OnPropertyChanged("ProgramText");
            }
            get => _ProgramText;
        }


        private ObservableCollection<UsedGroupViewModel> _UsedGroupsViewModels;

        public ObservableCollection<UsedGroupViewModel> UsedGroupViewModels
        {
            set
            {
                _UsedGroupsViewModels = value;
                OnPropertyChanged("UsedGroupViewModels");
            }
            get => _UsedGroupsViewModels;
        }


        private ObservableCollection<GroupViewModel> _AddingGroupViewModels;
        public ObservableCollection<GroupViewModel> AddingGroupViewModels
        {
            set
            {
                _AddingGroupViewModels = value;
                OnPropertyChanged("AddingGroupViewModels");
            }
            get => _AddingGroupViewModels;
        }


        private RelayCommand _GetFileCodeCommand;
        public RelayCommand GetFileCodeCommand => _GetFileCodeCommand ??= new RelayCommand(obj =>
        {
            FileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == false) return;

            filePath = dialog.FileName;
            string fileText = FileService.ReadAllText(filePath);

            this.Program = _RobotProgramService.GetFanucLSProgram(fileText);
            ProgramText = _RobotProgramService.CombineFileParts(this.Program);
            List<bool> groupList = this.Program.UsedGroupsList;
                        
            UsedGroupViewModels = new ObservableCollection<UsedGroupViewModel>();
            for (int i = 0; i < groupList.Count; i++)
            {
                if (groupList[i])
                {
                    UsedGroupViewModels.Add(new UsedGroupViewModel(i + 1, this));
                }
            }
        });

        private RelayCommand _SaveFileCommand;
        public RelayCommand SaveFileCommand => _SaveFileCommand ??= new RelayCommand(obj =>
        {
            if (Program == null) return;
            string programText = _RobotProgramService.CombineFileParts(this.Program);
            FileService.WriteToFile(filePath, programText);
        });

        private RelayCommand _SaveFileAsCommand;
        public RelayCommand SaveFileAsCommand => _SaveFileAsCommand ??= new RelayCommand(obj =>
        {
            if (Program == null) return;

            SaveFileDialog dialog = new();
            dialog.Filter = "Файлы программ (*.LS)|*.ls";
            if (dialog.ShowDialog() == false) return;

            string programName = dialog.SafeFileName[0..dialog.SafeFileName.LastIndexOf(".")];
            _RobotProgramService.SetFanucLSFileName(this.Program, programName);

            string programText = _RobotProgramService.CombineFileParts(this.Program);
            FileService.WriteToFile(dialog.FileName, programText);
        });


        private RelayCommand _DeleteSelectedUsedGroupCommand;
        public RelayCommand DeleteSelectedUsedGroupCommand => _DeleteSelectedUsedGroupCommand ??= new RelayCommand(obj =>
        {
            UsedGroupViewModel usedGroup = obj as UsedGroupViewModel;
            _RobotProgramService.DeleteGroup(this.Program, usedGroup.UsedGroupNumber);
            UsedGroupViewModels.Remove(usedGroup);
            ProgramText = _RobotProgramService.CombineFileParts(this.Program);
        });

        private RelayCommand _SaveAddedGroupsCommand;
        public RelayCommand SaveAddedGroupsCommand => _SaveAddedGroupsCommand ??= new RelayCommand(obj =>
        {
            if (this.Program == null) return;
            if (AddingGroupViewModels == null || AddingGroupViewModels.Count == 0) return;
            
            List<RobotGroup> AddedGroups = new();
            foreach (GroupViewModel groupViewModel in AddingGroupViewModels)
            {
                if (groupViewModel.GroupNumber <= 1)
                {
                    MessageBox.Show($"You can't add or change this group : {groupViewModel.GroupNumber}");
                    return;
                }
                groupViewModel.SaveCoordinateBlockCommand.Execute(null);
                RobotGroup rGroup = groupViewModel.robotGroup;
                AddedGroups.Add(rGroup);
            }
            _RobotProgramService.AddGroups(this.Program, AddedGroups);
            ProgramText = _RobotProgramService.CombineFileParts(this.Program);

            List<bool> usedGroups = Program.UsedGroupsList;
            
            UsedGroupViewModels.Clear();
            for(int i = 0; i < usedGroups.Count; i++)
            {
                if (usedGroups[i])
                {
                    UsedGroupViewModels.Add(new UsedGroupViewModel(i + 1, this));
                }
            }
            AddingGroupViewModels.Clear();
            AddingGroupViewModels.Add(new GroupViewModel(new RobotGroup(0, 1), this));
        });


        private RelayCommand _AddGroupBlockCommand;
        public RelayCommand AddGroupBlockCommand => _AddGroupBlockCommand ??= new RelayCommand(obj =>
        {
            RobotGroup robotGroup = new(0, 1);
            AddingGroupViewModels.Add(new GroupViewModel(robotGroup, this));

        });

        private RelayCommand _DeleteGroupBlockCommand;
        public RelayCommand DeleteGroupBlockCommand => _DeleteGroupBlockCommand ??= new RelayCommand(obj =>
        {
            GroupViewModel group = obj as GroupViewModel;
            AddingGroupViewModels.Remove(group);
        });
    }
}
