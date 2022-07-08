using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using fanuc_group_exchange_desktop.Model;
using fanuc_group_exchange_desktop.Command;

namespace fanuc_group_exchange_desktop.ViewModel
{
    public class UsedGroupViewModel : GroupViewModel
    {
        public UsedGroupViewModel(RobotGroup group, ApplicationViewModel applicationVM) : base(group, applicationVM)
        {
            SetUsedAxes();
        }


        private int _Height;
        public int Height
        {
            set
            {
                _Height = value;
                OnPropertyChanged();
            }
            get
            {
                return _Height;
            }
        }

        private string _UnnableToChangeAxesText;
        public string UnnableToStringAxesText
        {
            set
            {
                _UnnableToChangeAxesText = value;
                OnPropertyChanged();
            }
            get { return _UnnableToChangeAxesText; }
        }

        private ObservableCollection<AxeViewModel> _UsedMainAxes;
        public ObservableCollection<AxeViewModel> UsedMainAxes
        {
            set
            {
                _UsedMainAxes = value;
                //List<Coordinate> mainAxes = new List<Coordinate>();
                //foreach (AxeViewModel mainAxeVM in value)
                //{
                //   mainAxes.Add(mainAxeVM.Coordinate);
                //}
                //Group.MainAxes = mainAxes;

                OnPropertyChanged("UsedMainAxes");
            }
            get 
            {
                //ObservableCollection<AxeViewModel> mainAxes = new ObservableCollection<AxeViewModel>();
                //foreach (Coordinate mainAxe in Group.MainAxes)
                //{
                //    mainAxes.Add(new AxeViewModel(mainAxe, this));
                //}
                //return mainAxes;
                return _UsedMainAxes;
            }
        }

        private ObservableCollection<AxeViewModel> _UsedExtendedAxes;
        public ObservableCollection<AxeViewModel> UsedExtendedAxes
        {
            set
            {
                _UsedExtendedAxes = value;
                //List<Coordinate> extendedAxes = new List<Coordinate>();
                //foreach (AxeViewModel mainAxeVM in value)
                //{
                //    extendedAxes.Add(mainAxeVM.Coordinate);
                //}
                //Group.ExtendedAxes = extendedAxes;
                OnPropertyChanged("UsedExtendedAxes");
            }
            get
            {
                //ObservableCollection<AxeViewModel> extendedAxes = new ObservableCollection<AxeViewModel>();
                //foreach (Coordinate extendedAxe in Group.ExtendedAxes)
                //{
                //    extendedAxes.Add(new AxeViewModel(extendedAxe, this));
                //}
                //return extendedAxes;
                return _UsedExtendedAxes;
            }
        }



        private void SetUsedAxes()
        {
            UsedMainAxes = new ObservableCollection<AxeViewModel>();
            UsedExtendedAxes = new ObservableCollection<AxeViewModel>();
            
            if (Group.Number == 0 || Group.IsCartesian)
            {
                Height = 40;
                UnnableToStringAxesText = "Cant change axes \nof cartesian group";
            } 
            else
            {
                Height = 0;
                foreach (Coordinate mainAxe in Group.MainAxes)
                {
                    UsedMainAxes.Add(new AxeViewModel(mainAxe, this));
                }
            }

            foreach (Coordinate extendedAxe in Group.ExtendedAxes)
            {
                UsedExtendedAxes.Add(new AxeViewModel(extendedAxe, this));
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
                        applicationViewModel.DeleteSelectedUsedGroupCommand.Execute(this);
                    }));
            }
        }

        private RelayCommand _AddAxeBlockCommand;
        public override RelayCommand AddAxeBlockCommand
        {
            get
            {
                return _AddAxeBlockCommand ?? (
                    _AddAxeBlockCommand = new RelayCommand(obj =>
                    {
                        if (obj.ToString() == "J" && Group.Number != 0 && !Group.IsCartesian)
                        {
                            Coordinate coordinate = new Coordinate("J", Group.MainAxes.Count + AddingMainAxes.Count + 1);
                            AddingMainAxes.Add(new AxeViewModel(coordinate, this));
                        }
                        if(obj.ToString() == "E")
                        {
                            Coordinate coordinate = new Coordinate("E", Group.ExtendedAxes.Count + AddingExtendedAxes.Count + 1);
                            if (Group.ExtendedAxes.Count + AddingExtendedAxes.Count < 3)
                            {
                                AddingExtendedAxes.Add(new AxeViewModel(coordinate, this));
                            }
                        }
                    }));
            }
        }


        private RelayCommand _DeleteUsedAxeCommand;
        public override RelayCommand DeleteUsedAxeCommand => _DeleteUsedAxeCommand ??= new RelayCommand(obj =>
        {
            AxeViewModel coordinate = obj as AxeViewModel;
            if (coordinate.Coordinate.CoordinateName == "E")
            {
                Group.ExtendedAxes.Remove(coordinate.Coordinate);
                UsedExtendedAxes.Remove(coordinate);
                
                for (int i = coordinate.Coordinate.Number - 1; i < UsedExtendedAxes.Count; i++)
                {
                    UsedExtendedAxes[i].CoordinateNumber = $"{i + 1}";
                }
                for (int i = 0; i < AddingExtendedAxes.Count; i++)
                {
                    int number = AddingExtendedAxes[i].Coordinate.Number;
                    AddingExtendedAxes[i].CoordinateNumber = $"{--number}";
                }
            }
            if (coordinate.Coordinate.CoordinateName == "J")
            {
                Group.MainAxes.Remove(coordinate.Coordinate);
                UsedMainAxes.Remove(coordinate);
                for (int i = coordinate.Coordinate.Number - 1; i < UsedMainAxes.Count; i++)
                {
                    UsedMainAxes[i].CoordinateNumber = $"{i + 1}";
                }
                for (int i = 0; i < AddingMainAxes.Count; i++)
                {
                    int number = AddingMainAxes[i].Coordinate.Number;
                    AddingMainAxes[i].CoordinateNumber = $"{--number}";
                }
            }

            if (UsedMainAxes.Count == 0)
            {
                applicationViewModel.DeleteSelectedUsedGroupCommand.Execute(this);
            }

            applicationViewModel.UpdateProgramText();
        });
    }
}
