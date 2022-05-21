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
        public FileWorker fileWorker;
        private string filePath;

        private StringBuilder _ProgramText;
        public StringBuilder ProgramText
        {
            set
            {
                _ProgramText = value;
                OnPropertyChanged("ProgramText");
            }
            get
            {
                return _ProgramText;
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
            get
            {
                return _UsedGroupsViewModels;
            }
        }


        private ObservableCollection<GroupViewModel> _AddingGroupViewModels;
        public ObservableCollection<GroupViewModel> AddingGroupViewModels
        {
            set
            {
                _AddingGroupViewModels = value;
                OnPropertyChanged("AddingGroupViewModels");
            }
            get
            {
                return _AddingGroupViewModels;
            }
        }


        public ApplicationViewModel(Window window)
        {
            fileWorker = new FileWorker();
            ProgramText = new StringBuilder(App.placeholderText);
            AddingGroupViewModels = new ObservableCollection<GroupViewModel>
            {
                new GroupViewModel(new RobotGroup(0,1), this)
            };
        }


        private RelayCommand _GetFileCodeCommand;
        public RelayCommand GetFileCodeCommand
        {
            get
            {
               return _GetFileCodeCommand ??
                    (_GetFileCodeCommand = new RelayCommand(obj =>
                    {
                        FileDialog dialog = new OpenFileDialog();
                        if (dialog.ShowDialog() == false) return;

                        filePath = dialog.FileName;

                        fileWorker.ReadFromFile(filePath);
                        ProgramText = fileWorker.CombineFileParts();


                        List<string> groupList = fileWorker.UsedGroupsList;
                        
                        UsedGroupViewModels = new ObservableCollection<UsedGroupViewModel>();
                        for (int i = 0; i < groupList.Count; i++)
                        {
                            if(groupList[i] == "1")
                            {
                                UsedGroupViewModels.Add(new UsedGroupViewModel(i+1,this));
                            }
                        }
                    }));
            }
        }

        public RelayCommand _SaveFileCommand;
        public RelayCommand SaveFileCommand
        {
            get
            {
                return _SaveFileCommand ??
                    (_SaveFileCommand = new RelayCommand(obj => {
                        if (_ProgramText ==  new StringBuilder(App.placeholderText)) return;

                        string file = fileWorker.CombineFileParts().ToString();
                        fileWorker.WriteToFile(filePath, file);
                    }));
            }
        }

        public RelayCommand _SaveFileAsCommand;
        public RelayCommand SaveFileAsCommand
        {
            get
            {
                return _SaveFileAsCommand ??
                    (_SaveFileAsCommand = new RelayCommand(obj =>
                    {
                        if (_ProgramText == new StringBuilder(App.placeholderText)) return;


                        SaveFileDialog dialog = new SaveFileDialog();
                        dialog.Filter = "Файлы программ (*.LS)|*.ls";
                        if (dialog.ShowDialog() == false) return;

                        string fileName = dialog.SafeFileName;
                        fileWorker.FileName = fileName.Substring(0, fileName.LastIndexOf("."));

                        string file = fileWorker.CombineFileParts().ToString();
                        fileWorker.WriteToFile(dialog.FileName, file);
                    }));
            }
        }


        private RelayCommand _DeleteSelectedUsedGroupCommand;

        public RelayCommand DeleteSelectedUsedGroupCommand
        {
            get
            {
                return _DeleteSelectedUsedGroupCommand ??
                    (_DeleteSelectedUsedGroupCommand = new RelayCommand(obj =>
                    {
                        UsedGroupViewModel usedGroup = obj as UsedGroupViewModel;
                        fileWorker.DeleteGroup(usedGroup.UsedGroupNumber);
                        UsedGroupViewModels.Remove(usedGroup);
                        ProgramText = fileWorker.CombineFileParts();
                    }));
            }
        }

        private RelayCommand _AddGroupBlockCommand;

        public RelayCommand AddGroupBlockCommand
        {
            get
            {
                return _AddGroupBlockCommand ??
                    (_AddGroupBlockCommand = new RelayCommand(obj =>
                    {
                        RobotGroup robotGroup = new RobotGroup(0,1);
                        AddingGroupViewModels.Add(new GroupViewModel(robotGroup,this));

                    }));
            }
        }

        private RelayCommand _DeleteGroupBlockCommand;

        public RelayCommand DeleteGroupBlockCommand
        {
            get
            {
                return _DeleteGroupBlockCommand ??
                    (_DeleteGroupBlockCommand = new RelayCommand(obj =>
                    {
                        GroupViewModel group = obj as GroupViewModel;
                        AddingGroupViewModels.Remove(group);
                    }));
            }
        }

        private RelayCommand _SaveAddedGroupsCommand;

        public RelayCommand SaveAddedGroupsCommand
        {
            get
            {
                return _SaveAddedGroupsCommand ??
                    (_SaveAddedGroupsCommand = new RelayCommand(obj =>
                    {
                        if (SaveAddedGroupsCommand != null)
                        {
                            if (_ProgramText == new StringBuilder(App.placeholderText)) return;

                            List<RobotGroup> groups = new List<RobotGroup>();
                            
                            foreach(GroupViewModel groupViewModel in AddingGroupViewModels)
                            {

                                if (groupViewModel.GroupNumber == 0 || groupViewModel.GroupNumber == 1)
                                {
                                    MessageBox.Show("You can't add or change this group : " + groupViewModel.GroupNumber.ToString());
                                    return;
                                }
                                groupViewModel.SaveCoordinateBlockCommand.Execute(null);
                                RobotGroup robotNotFirstGroup = groupViewModel.robotGroup;


                                groups.Add(robotNotFirstGroup);
                                
                                UsedGroupViewModel usedGroupViewModel = new UsedGroupViewModel(robotNotFirstGroup.Number, this);
                                
                                bool isContain = false;
                                foreach (UsedGroupViewModel usedGroup in UsedGroupViewModels)
                                {
                                    if (usedGroup.UsedGroupNumber == usedGroupViewModel.UsedGroupNumber)
                                    {
                                        isContain = true;
                                        break;
                                    }
                                }
                                if (isContain == false)
                                {
                                    if (usedGroupViewModel.UsedGroupNumber < UsedGroupViewModels[UsedGroupViewModels.Count - 1].UsedGroupNumber)
                                    {
                                        for (int i = 0; i < UsedGroupViewModels.Count; i++)
                                        {
                                            if (UsedGroupViewModels[i].UsedGroupNumber > usedGroupViewModel.UsedGroupNumber)
                                            {
                                                UsedGroupViewModels.Insert(i, usedGroupViewModel);
                                                break;
                                            }
                                        }
                                    } else
                                    {
                                        UsedGroupViewModels.Add(usedGroupViewModel);
                                    }
                                }

                                
                            }
                            fileWorker.SetFanucLSFilePositions(groups);

                            ProgramText = fileWorker.CombineFileParts();
                        }
                    }));
            }
        }
    }
}
