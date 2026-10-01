using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Services.Shared;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Validators;
using MTAoarsGeneral.Repositories.Interfaces;
using AutoMapper;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.Utilities.Extensions;
using MTAoarsGeneral.Utilities.Constants;
using System.Transactions;
using MTAoarsGeneral.ViewModels.Notifications;
using MTAoarsGeneral.Mappers.Operations;
using MTAoarsGeneral.ViewModels.Operations;
using System.Data;

namespace MTAoarsGeneral.Services.Operations {
    public class ALCService : BaseService, IALCService {

        IALCRepository alcRepository;
        public ALCService(IValidationProvider validationProvider, IALCRepository alcRepository)
            : base(validationProvider) {
                this.alcRepository = alcRepository;
                ALCHeader h;
        }

        public void Save(ALCHeaderViewModel input) {
            var header = Mapper.Map<ALCHeaderViewModel, ALCHeader>(input);
            alcRepository.SaveHeader(header);
        }
        
    }// class
}// namespace
