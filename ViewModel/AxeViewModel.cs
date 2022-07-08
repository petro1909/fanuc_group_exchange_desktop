using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fanuc_group_exchange_desktop.Model;
using fanuc_group_exchange_desktop.Command;
using System.Collections.ObjectModel;

namespace fanuc_group_exchange_desktop.ViewModel
{
    public class AxeViewModel : BaseViewModel
    {
        private GroupViewModel GroupVM;
        public Coordinate Coordinate;

        public AxeViewModel(Coordinate coordinate, GroupViewModel groupVM)
        {
            this.Coordinate = coordinate;
            GroupVM = groupVM;
        }

        public string CoordinateName
        {
            set
            {
                Coordinate.CoordinateName = value;
                OnPropertyChanged("CoordinateName");
            }
            get { return Coordinate.CoordinateName; }
        }

        public string CoordinateNumber
        {
            set
            { 

                Coordinate.Number = int.Parse(value);
                OnPropertyChanged("CoordinateNumber");
            }
            get
            {
                string numberStr = Coordinate.Number == 0 ? string.Empty : Coordinate.Number.ToString(); 
                return  numberStr;
            }
        }

        public double CoordinatePosition 
        {
            set
            {
                Coordinate.CoordinatePosition = value;
                OnPropertyChanged("CoordinatePosition");
            }
            get { return Coordinate.CoordinatePosition; }
        }

        public string CoordinateUnit
        {
            set
            {
                Coordinate.CoordinateUnit = value;
                OnPropertyChanged("CoordinateUnit");
            }
            get
            {
                return Coordinate.CoordinateUnit;
            }
        }

        private ObservableCollection<string> _Units;

        public ObservableCollection<string> Units
        {
            set
            {
                _Units = value;
                OnPropertyChanged("Units");
            }
            get
            {
                return new ObservableCollection<string> { "mm", "deg" };
            }
        }
        private RelayCommand _DeleteAxeCommand;
        public RelayCommand DeleteAxeCommand
        {
            get
            {
                return _DeleteAxeCommand ??
                    (_DeleteAxeCommand = new RelayCommand(obj =>
                    {
                        bool isUsedAxe = bool.Parse(obj.ToString());
                        if (isUsedAxe)
                        {
                            GroupVM.DeleteUsedAxeCommand.Execute(this);
                        }
                        else
                        {
                            GroupVM.DeleteAddingAxeCommand.Execute(this);
                        }
                    }));
            }
        }
    }
}
