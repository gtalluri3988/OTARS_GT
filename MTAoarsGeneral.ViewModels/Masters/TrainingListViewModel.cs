using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTAoarsGeneral.ViewModels.Masters {
    
    public class TrainingListViewModel {

        public TrainingListViewModel() {
            TrainingModel = new TrainingViewModel();
        }

        public TrainingViewModel TrainingModel { get; set; }

        public int Year { get; set; }
    }// class

}// namespace
