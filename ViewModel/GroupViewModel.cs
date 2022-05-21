using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fanuc_group_exchange_desktop.Model;
using fanuc_group_exchange_desktop.Command;

namespace fanuc_group_exchange_desktop.ViewModel
{
    public class GroupViewModel : BaseViewModel
    {

        public RobotGroup robotGroup;

        private ApplicationViewModel applicationViewModel;


        public GroupViewModel() { }

        public GroupViewModel(RobotGroup robotGroup, ApplicationViewModel applicationViewModel)
        {
            this.applicationViewModel = applicationViewModel;
            this.robotGroup = robotGroup;
            Coordinates = new ObservableCollection<CoordinateViewModel>()
            {

                new CoordinateViewModel(new Coordinate("J1="), this)
            };
        }

        public int GroupNumber
        {
            set
            {
                robotGroup.Number = value;
                OnPropertyChanged("GroupNumber");
            }
            get
            {
                return robotGroup.Number;
            }
        }

        public int GroupFrame
        {
            set
            {
                robotGroup.UserFrame = value;
                OnPropertyChanged("GroupFrame");
            }
            get
            {
                return robotGroup.UserFrame;
            }
        }

        public int GroupTool
        {
            set
            {
                robotGroup.UserTool = value;
                OnPropertyChanged("GroupTool");
            }
            get
            {
                return robotGroup.UserTool;
            }
        }

        private ObservableCollection<CoordinateViewModel> _Coordinates;
        public ObservableCollection<CoordinateViewModel> Coordinates
        {
            set
            {
                _Coordinates = value;
               
                
                OnPropertyChanged("RobotCoordiantes");
            }
            get
            {   
                return _Coordinates;
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
                        applicationViewModel.DeleteGroupBlockCommand.Execute(this);
                    }));
            }
        }

        private RelayCommand _AddCoordinateBlockCommand;
        public RelayCommand AddCoordinateBlockCommand
        {
            get
            {
                return _AddCoordinateBlockCommand ?? (
                    _AddCoordinateBlockCommand = new RelayCommand(obj =>
                    {
                        Coordinate coordinate = new Coordinate($"J{Coordinates.Count + 1}=");
                        Coordinates.Add(new CoordinateViewModel(coordinate,this));
                    }));
            }
        }


        private RelayCommand _DeleteCoordinateBlockCommand;

        public RelayCommand DeleteCoordinateBlockCommand
        {
            get
            {
                return _DeleteCoordinateBlockCommand ??
                    (_DeleteCoordinateBlockCommand = new RelayCommand(obj =>
                    {

                        CoordinateViewModel coordinate = obj as CoordinateViewModel;
                        int number = int.Parse(coordinate.CoordinateName[1..coordinate.CoordinateName.IndexOf('=')]);
                        Coordinates.Remove(coordinate);

                        for(int i = number - 1; i < Coordinates.Count; i++)
                        {
                            Coordinates[i].CoordinateName = $"J{i+1}=";
                        }
                    }));
            }
        }

        private RelayCommand _SaveCoordinateBlockCommand;

        public RelayCommand SaveCoordinateBlockCommand
        {
            get
            {
                return _SaveCoordinateBlockCommand ??
                    (_SaveCoordinateBlockCommand = new RelayCommand(obj =>
                    {
                        List<Coordinate> coordinates = new List<Coordinate>();
                        foreach (CoordinateViewModel coord in Coordinates)
                        {
                            coordinates.Add(coord.Coordinate);
                        }

                        robotGroup.Coordinates = coordinates;
                    }));
            }
        }








    }
}
