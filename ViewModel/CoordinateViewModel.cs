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
    public class CoordinateViewModel : BaseViewModel
    {
        public Coordinate Coordinate;
        private GroupViewModel groupViewModel;


        public CoordinateViewModel() { 
        }

        public CoordinateViewModel(Coordinate coordinate, GroupViewModel groupViewModel)
        {
            this.Coordinate = coordinate;
            this.groupViewModel = groupViewModel;
        }

        public int CoordinateNumber
        {
            set 
            { 
                Coordinate.Number = value;
                OnPropertyChanged("CoordinateNumber");
            }
            get { return Coordinate.Number; }
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


        private RelayCommand _DeleteCoordinateCommand;
        public RelayCommand DeleteCoordinateCommand
        {
            get
            {
                return _DeleteCoordinateCommand ??
                    (_DeleteCoordinateCommand = new RelayCommand(obj => 
                    {
                        groupViewModel.DeleteCoordinateBlockCommand.Execute(this);
                    } ));
            }
        }
    }
}
