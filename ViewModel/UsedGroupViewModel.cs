using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fanuc_group_exchange_desktop.Model;
using fanuc_group_exchange_desktop.Command;

namespace fanuc_group_exchange_desktop.ViewModel
{
    public class UsedGroupViewModel : BaseViewModel
    {

        public ApplicationViewModel applicationViewModel;
        public UsedGroupViewModel(int groupNumber, ApplicationViewModel applicationViewModel) 
        {
            UsedGroupNumber = groupNumber;
            this.applicationViewModel = applicationViewModel;
        }

        private int _UsedGroupNumber;

        public int UsedGroupNumber
        {
            set
            {
                _UsedGroupNumber = value;
                OnPropertyChanged("UsedGroupNumber");
            }
            get
            {
                return _UsedGroupNumber;
            }
        }

        private RelayCommand _DeleteUsedGroup;
        public RelayCommand DeleteUsedGroup
        {
            get
            {
                return _DeleteUsedGroup ??
                    (_DeleteUsedGroup = new RelayCommand(obj =>
                    {
                        applicationViewModel.DeleteSelectedUsedGroupCommand.Execute(this);

                    }));
            }
        }
    }
}
