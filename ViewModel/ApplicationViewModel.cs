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
            AddingGroupViewModels = new ObservableCollection<AddingGroupViewModel>
            {
                new AddingGroupViewModel(new RobotGroup(0,1), this)
            };
            ProgramVA = VerticalAlignment.Center;
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

        private string _ProgramNameText;
        public string ProgramNameText
        {
            set
            {
                _ProgramNameText = value;
                OnPropertyChanged("ProgramNameText");
            } 
            get
            {
                return _ProgramNameText;
            }
        }

        private VerticalAlignment _ProgramVA;
        public VerticalAlignment ProgramVA
        {
            set
            {
                _ProgramVA = value;
                OnPropertyChanged();
            }
            get
            {
                return _ProgramVA;
            }
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


        private ObservableCollection<AddingGroupViewModel> _AddingGroupViewModels;
        public ObservableCollection<AddingGroupViewModel> AddingGroupViewModels
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
            ProgramNameText = Program.Name;
            UpdateProgramText();
            ProgramVA = VerticalAlignment.Top;

            List<bool> groupList = this.Program.UsedGroupsList;

            UsedGroupViewModels = new ObservableCollection<UsedGroupViewModel>();

            IEnumerable<RobotGroup> usedGroups = this.Program.RobotPositions[0].RobotGroupsList.Values;
            foreach(RobotGroup group in usedGroups)
            {
                UsedGroupViewModels.Add(new UsedGroupViewModel(group, this));
            }

        });

        private RelayCommand _SaveFileNameCommand;
        public RelayCommand SaveFileNameCommand => _SaveFileNameCommand ??= new RelayCommand(obj =>
        {
            if (Program == null) return;
            _RobotProgramService.SetFanucLSFileName(this.Program, ProgramNameText);
            UpdateProgramText();
        });

        private RelayCommand _SaveFileCommand;
        public RelayCommand SaveFileCommand => _SaveFileCommand ??= new RelayCommand(obj =>
        {
            if (Program == null) return;
            string programText = _RobotProgramService.CombineFileParts(this.Program);

            string newFilePath = $"{filePath[0..(filePath.LastIndexOf("\\")+1)]}{Program.Name}.LS";
            FileService.DeleteFile(filePath);
            FileService.WriteToFile(newFilePath, programText);
            
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


        private RelayCommand _SaveChangesCommand;
        public RelayCommand SaveChangesCommand => _SaveChangesCommand ??= new RelayCommand(obj =>
        {
            if (this.Program == null) return;
            if (AddingGroupViewModels == null) return;
            
            List<RobotGroup> updatedGroups = new();

            ObservableCollection<GroupViewModel> commonGroups = new ObservableCollection<GroupViewModel>(UsedGroupViewModels);
            foreach (var addingGroupVM in AddingGroupViewModels)
            {
                if (addingGroupVM.Group.Number == 0 || addingGroupVM.AddingMainAxes.Count == 0) continue;
                commonGroups.Add(addingGroupVM);

            }
            foreach (GroupViewModel groupViewModel in commonGroups)
            {
                groupViewModel.SaveCoordinates();
                updatedGroups.Add(groupViewModel.Group);
            }
            _RobotProgramService.SetGroups(this.Program, updatedGroups);
            UpdateProgramText();

            UsedGroupViewModels.Clear();
            
            IEnumerable<RobotGroup> usedGroups = this.Program.RobotPositions[0].RobotGroupsList.Values;
            foreach (RobotGroup group in usedGroups)
            {
                UsedGroupViewModels.Add(new UsedGroupViewModel(group, this));
            }
            AddingGroupViewModels.Clear();
            AddingGroupViewModels.Add(new AddingGroupViewModel(new RobotGroup(0, 1), this));
        });


        private RelayCommand _AddGroupBlockCommand;
        public RelayCommand AddGroupBlockCommand => _AddGroupBlockCommand ??= new RelayCommand(obj =>
        {
            RobotGroup robotGroup = new(0, 1);
            AddingGroupViewModels.Add(new AddingGroupViewModel(robotGroup, this));

        });


        //---Delete selected used group in current program---//
        private RelayCommand _DeleteSelectedUsedGroupCommand;
        public RelayCommand DeleteSelectedUsedGroupCommand => _DeleteSelectedUsedGroupCommand ??= new RelayCommand(obj =>
        {
            UsedGroupViewModel usedGroup = obj as UsedGroupViewModel;
            _RobotProgramService.DeleteGroup(this.Program, usedGroup.GroupNumber);
            UsedGroupViewModels.Remove(usedGroup);
            ProgramText = _RobotProgramService.CombineFileParts(this.Program);
        });

        //---Delete selected group block in adding group list---//
        private RelayCommand _DeleteAddingGroupBlockCommand;
        public RelayCommand DeleteAddingGroupBlockCommand => _DeleteAddingGroupBlockCommand ??= new RelayCommand(obj =>
        {
            AddingGroupViewModel group = obj as AddingGroupViewModel;
            AddingGroupViewModels.Remove(group);
        });


        public void UpdateProgramText()
        {
            ProgramText = _RobotProgramService.CombineFileParts(this.Program);
        }
    }
}
