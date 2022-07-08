using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fanuc_group_exchange_desktop.Command;
using fanuc_group_exchange_desktop.Model;

namespace fanuc_group_exchange_desktop.ViewModel
{
    public abstract class GroupViewModel : BaseViewModel
    {
        public RobotGroup Group;
        public ApplicationViewModel applicationViewModel;

        protected GroupViewModel(RobotGroup group, ApplicationViewModel applicationViewModel)
        {
            Group = group;
            this.applicationViewModel = applicationViewModel;
            AddingExtendedAxes = new ObservableCollection<AxeViewModel>();
            AddingMainAxes = new ObservableCollection<AxeViewModel>();
        }

        public int GroupNumber
        {
            set
            {
                Group.Number = value;
                OnPropertyChanged("GroupNumber");
            }
            get
            {
                return Group.Number;
            }
        }

        public int GroupFrame
        {
            set
            {
                Group.UserFrame = value;
                OnPropertyChanged("GroupFrame");
            }
            get
            {
                return Group.UserFrame;
            }
        }

        public int GroupTool
        {
            set
            {
                Group.UserTool = value;
                OnPropertyChanged("GroupTool");
            }
            get
            {
                return Group.UserTool;
            }
        }

        private ObservableCollection<AxeViewModel> _AddingMainAxes;
        public ObservableCollection<AxeViewModel> AddingMainAxes
        {
            set
            {
                _AddingMainAxes = value;
                OnPropertyChanged("AddingMainAxes");
            }
            get
            {
                return _AddingMainAxes;
            }
        }


        private ObservableCollection<AxeViewModel> _AddingExtendedAxes;
        public ObservableCollection<AxeViewModel> AddingExtendedAxes
        {
            set
            {
                _AddingExtendedAxes = value;
                OnPropertyChanged("AddingMainAxes");
            }
            get
            {
                return _AddingExtendedAxes;
            }
        }


        public virtual RelayCommand AddAxeBlockCommand { set; get; }
        public virtual RelayCommand DeleteUsedAxeCommand { set; get; }

        private RelayCommand _DeleteAddingAxeCommand;
        public RelayCommand DeleteAddingAxeCommand => _DeleteAddingAxeCommand ??= new RelayCommand(obj =>
        {
            AxeViewModel coordinate = obj as AxeViewModel;
            int selectedNumber = coordinate.Coordinate.Number;
            if (coordinate.Coordinate.CoordinateName == "E")
            {
                int firstAxeNumber = AddingExtendedAxes[0].Coordinate.Number;
                AddingExtendedAxes.Remove(coordinate);
                if (AddingExtendedAxes.Count == 0)
                {
                    return;
                }
                
                for (int i = selectedNumber, j = selectedNumber - firstAxeNumber; j < AddingExtendedAxes.Count; i++, j++)
                {
                    AddingExtendedAxes[j].CoordinateNumber = $"{i}";
                }
            }
            if (coordinate.Coordinate.CoordinateName == "J")
            {
                int firstAxeNumber = AddingMainAxes[0].Coordinate.Number;
                AddingMainAxes.Remove(coordinate);
                if (AddingMainAxes.Count == 0)
                {
                    return;
                }
                
                for (int i = selectedNumber, j = selectedNumber - firstAxeNumber; j < AddingMainAxes.Count; i++, j++)
                {
                    AddingMainAxes[j].CoordinateNumber = $"{i}";
                }
            }
        });

        public void SaveCoordinates()
        {      
            foreach (AxeViewModel coord in AddingMainAxes)
            {
                Group.MainAxes.Add(coord.Coordinate);
            }
            foreach (AxeViewModel coord in AddingExtendedAxes)
            {
                Group.ExtendedAxes.Add(coord.Coordinate);
            }     
        }
    }
}
