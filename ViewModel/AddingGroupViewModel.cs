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
    public class AddingGroupViewModel : GroupViewModel
    {
        public AddingGroupViewModel(RobotGroup group, ApplicationViewModel applicationVM) : base(group, applicationVM)
        {
            group.MainAxes = new List<Coordinate>();
            group.ExtendedAxes = new List<Coordinate>();
        }

        private RelayCommand _DeleteAddingGroupBlockCommand;
        public RelayCommand DeleteAddingGroupBlockCommand
        {
            get
            {
                return _DeleteAddingGroupBlockCommand ??
                    (_DeleteAddingGroupBlockCommand = new RelayCommand(obj =>
                    {
                        applicationViewModel.DeleteAddingGroupBlockCommand.Execute(this);
                    }));
            }
        }

        //private RelayCommand _AddMainAxeBlockCommand;
        //public RelayCommand AddMainAxeBlockCommand
        //{
        //    get
        //    {
        //        return _AddMainAxeBlockCommand ?? (
        //            _AddMainAxeBlockCommand = new RelayCommand(obj =>
        //            {
        //                Coordinate coordinate = new Coordinate("J", AddingMainAxes.Count + 1);
        //                AddingMainAxes.Add(new AxeViewModel(coordinate, this));
        //            }));
        //    }
        //}

        //private RelayCommand _AddExtendedAxeBlockCommand;
        //public RelayCommand AddExtendedAxeBlockCommand
        //{
        //    get
        //    {
        //        return _AddExtendedAxeBlockCommand ?? (
        //            _AddExtendedAxeBlockCommand = new RelayCommand(obj =>
        //            {
        //                Coordinate coordinate = new Coordinate("E", AddingExtendedAxes.Count + 1);
        //                AddingExtendedAxes.Add(new AxeViewModel(coordinate, this));
        //            }));
        //    }
        //}





        private RelayCommand _AddAxeBlockCommand;
        public override RelayCommand AddAxeBlockCommand
        {
            get
            {
                return _AddAxeBlockCommand ?? (
                    _AddAxeBlockCommand = new RelayCommand(obj =>
                    {
                        if (obj.ToString() == "J")
                        {
                            Coordinate coordinate = new Coordinate("J", AddingMainAxes.Count + 1);
                            AddingMainAxes.Add(new AxeViewModel(coordinate, this));
                        }
                        if (obj.ToString() == "E")
                        {
                            Coordinate coordinate = new Coordinate("E", AddingExtendedAxes.Count + 1);
                            if (AddingExtendedAxes.Count < 3)
                            {
                                AddingExtendedAxes.Add(new AxeViewModel(coordinate, this));
                            }
                        }
                    }));
            }
        }


        //private RelayCommand _DeleteAxeCommand;
        //public override RelayCommand DeleteAxeCommand => _DeleteAxeCommand ??= new RelayCommand(obj =>
        //{
        //    AxeViewModel coordinate = obj as AxeViewModel;
        //    if (coordinate.Coordinate.CoordinateName == "E")
        //    {
        //        AddingExtendedAxes.Remove(coordinate);

        //        for (int i = coordinate.Coordinate.Number - 1; i < AddingExtendedAxes.Count; i++)
        //        {
        //            AddingExtendedAxes[i].Coordinate.Number = i + 1;
        //        }
        //    }
        //    else
        //    {
        //        AddingMainAxes.Remove(coordinate);

        //        for (int i = coordinate.Coordinate.Number - 1; i < AddingMainAxes.Count; i++)
        //        {
        //            AddingMainAxes[i].Coordinate.Number = i + 1;
        //        }
        //    }
        //});


        //private RelayCommand _DeleteExtendedAxeBlockCommand;
        //public RelayCommand DeleteExtendedAxeBlockCommand => _DeleteExtendedAxeBlockCommand ??= new RelayCommand(obj =>
        //{
        //    AxeViewModel coordinate = obj as AxeViewModel;
        //    AddingExtendedAxes.Remove(coordinate);

        //    for (int i = coordinate.Coordinate.Number - 1; i < AddingExtendedAxes.Count; i++)
        //    {
        //        AddingExtendedAxes[i].CoordinateName = $"E{i + 1}=";
        //    }
        //});










    }
}
