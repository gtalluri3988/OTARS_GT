using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.Services.Interfaces;
using System.Data;
using MTAoarsGeneral.Repositories.Reports;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.ViewModels.Maintenance;
using MTAoarsGeneral.Repositories.Operations;

namespace MTAoarsGeneral.Services.Reports
{

    public class ReportService : IReportService
    {

        InvoiceReportRepository invoiceRepository;
        AsciiReportRepository asciiRepository;
        RenewalAsciiReportRepository renewalAsciiRepository;
        UserLoginReportRepository userLoginRepository;
        NewRegistrationStatisticsReportRepository newRegistrationRepository;
        RenewalStatisticsReportRepository renewalRepository;
        ActiveAgentsStatisticsReportRepository activeAgentsRepository;
        CPDReportRepository cpdRepository;
        BulkRegistrationReportRepository bulkRegistraitonRepository;
        SummaryReportRepository summaryRepository;
        AgentsReportRepository agentsRepository;
        TBEReportResultRepository tbeRepository;
        TBESpecialAgentsReportResultRepository tbeSpecialRepository;
        TerminatedAgentsStatisticsReportRepository terminatedAgentStatisticsRepository;
        TrainingDetailReportRepository trainingDetailRepository;
        ALCReportRepository alcRepository;
        ReferredListingReportRepository referredListingRepository;
        AdministrativeAuditTrailReportRepository administrativeAuditTrailReportRepository;
        PhotoUploadSummaryReportRepository photoUploadSummaryReportRepository;
        TBEExaminationReportRepository _TBEExaminationReportRepository;
        ResignedActiveAgentReportRepository resignedActiveAgentReportRepository;
        IScopeDataProvider dataProvider;
        TbeExemptionReportRepository _TbeExemptionReportRepository;

        public ReportService(IScopeDataProvider dataProvider, InvoiceReportRepository invoiceRepository, AsciiReportRepository asciiRepository, RenewalAsciiReportRepository renewalAsciiRepository,
            UserLoginReportRepository userLoginRepository, NewRegistrationStatisticsReportRepository newRegistrationRepository, RenewalStatisticsReportRepository renewalRepository,
            ActiveAgentsStatisticsReportRepository activeAgentsRepository, CPDReportRepository cpdRepository, BulkRegistrationReportRepository bulkRegistraitonRepository,
            SummaryReportRepository summaryRepository, AgentsReportRepository agentsRepository, TBEReportResultRepository tbeRepository, TerminatedAgentsStatisticsReportRepository terminatedAgentStatisticsRepository,
            TrainingDetailReportRepository trainingDetailRepository, ALCReportRepository alcRepository, ReferredListingReportRepository referredListingRepository, TBESpecialAgentsReportResultRepository tbeSpecialRepository,
            AdministrativeAuditTrailReportRepository administrativeAuditTrailReportRepository, PhotoUploadSummaryReportRepository photoUploadSummaryReportRepository,
            TBEExaminationReportRepository _TBEExaminationReportRepository, ResignedActiveAgentReportRepository resignedActiveAgentReportRepository,
            TbeExemptionReportRepository _TbeExemptionReportRepository)
        {
            this.invoiceRepository = invoiceRepository;
            this.asciiRepository = asciiRepository;
            this.renewalAsciiRepository = renewalAsciiRepository;
            this.userLoginRepository = userLoginRepository;
            this.newRegistrationRepository = newRegistrationRepository;
            this.renewalRepository = renewalRepository;
            this.activeAgentsRepository = activeAgentsRepository;
            this.cpdRepository = cpdRepository;
            this.bulkRegistraitonRepository = bulkRegistraitonRepository;
            this.summaryRepository = summaryRepository;
            this.agentsRepository = agentsRepository;
            this.dataProvider = dataProvider;
            this.tbeRepository = tbeRepository;
            this.tbeSpecialRepository = tbeSpecialRepository;
            this.terminatedAgentStatisticsRepository = terminatedAgentStatisticsRepository;
            this.trainingDetailRepository = trainingDetailRepository;
            this.alcRepository = alcRepository;
            this.referredListingRepository = referredListingRepository;
            this.administrativeAuditTrailReportRepository = administrativeAuditTrailReportRepository;
            this.photoUploadSummaryReportRepository = photoUploadSummaryReportRepository;
            this._TBEExaminationReportRepository = _TBEExaminationReportRepository;
            this.resignedActiveAgentReportRepository = resignedActiveAgentReportRepository;
            this._TbeExemptionReportRepository = _TbeExemptionReportRepository;
        }

        public Dictionary<string, IEnumerable<dynamic>> GetDataSource(string key, Dictionary<string, string> parameters)
        {
            switch (key)
            {
                case LookupConstants.Reports.Invoice:
                    return invoiceRepository.GetData(parameters);
                case LookupConstants.Reports.Ascii:
                    return asciiRepository.GetData(parameters);
                case LookupConstants.Reports.RenewalAscii:
                    return renewalAsciiRepository.GetData(parameters);
                case LookupConstants.Reports.UserLogin:
                    return userLoginRepository.GetData(parameters);
                case LookupConstants.Reports.AdminActivity:
                    return GetAdminActivityDataSource();
                case LookupConstants.Reports.NewRegistrationStatistics:
                    return newRegistrationRepository.GetData(parameters);
                case LookupConstants.Reports.RenewalStatistics:
                    return renewalRepository.GetData(parameters);
                case LookupConstants.Reports.ActiveAgentsStatistics:
                    return activeAgentsRepository.GetData(parameters);
                case LookupConstants.Reports.CPD:
                    return cpdRepository.GetData(parameters);
                case LookupConstants.Reports.BulkRegistration:
                    return bulkRegistraitonRepository.GetData(parameters);
                case LookupConstants.Reports.Summary:
                    return summaryRepository.GetData(parameters);
                case LookupConstants.Reports.Agents:
                    return agentsRepository.GetData(parameters);
                case LookupConstants.Reports.AutoTermination:
                    return agentsRepository.GetData(parameters);
                case LookupConstants.Reports.TerminationList:
                    return agentsRepository.GetData(parameters);
                case LookupConstants.Reports.TerminationSummary:
                    return agentsRepository.GetData(parameters);
                case LookupConstants.Reports.Consolidate:
                    return agentsRepository.GetData(parameters);
                case LookupConstants.Reports.ConsolidateSummary:
                    return agentsRepository.GetData(parameters);
                case LookupConstants.Reports.TBEResult:
                    return tbeRepository.GetData(parameters);
                case LookupConstants.Reports.InvoiceDetail:
                    return invoiceRepository.GetData(parameters);
                case LookupConstants.Reports.TerminatedAgentsStatistics:
                    return terminatedAgentStatisticsRepository.GetData(parameters);
                case LookupConstants.Reports.TrainingDetail:
                    return trainingDetailRepository.GetData(parameters);
                case LookupConstants.Reports.ALC:
                    return alcRepository.GetData(parameters);
                case LookupConstants.Reports.Referred:
                case LookupConstants.Reports.ReferredAdmin:
                    return referredListingRepository.GetData(parameters);
                case LookupConstants.Reports.TBESpecialAgents:
                    return tbeSpecialRepository.GetData(parameters);
                case LookupConstants.Reports.AdministrativeAuditTrail:
                    return administrativeAuditTrailReportRepository.GetData(parameters);
                case LookupConstants.Reports.PhotoUploadSummary:
                    return photoUploadSummaryReportRepository.GetData(parameters);
                case LookupConstants.Reports.TBEExamination:
                    return _TBEExaminationReportRepository.GetData(parameters);
                case LookupConstants.Reports.ResignedActiveAgents:
                    return resignedActiveAgentReportRepository.GetData(parameters);
                case LookupConstants.Reports.TBEExemption:
                    return _TbeExemptionReportRepository.GetData(parameters);
            }
            return null;
        }

        public Dictionary<string, string> GetReportParameters(string key, Dictionary<string, string> parameters)
        {
            var output = new Dictionary<string, string>();
            switch (key)
            {
                case LookupConstants.Reports.Invoice:
                    return invoiceRepository.GetParameters(parameters);
                case LookupConstants.Reports.Ascii:
                    return asciiRepository.GetParameters(parameters);
                case LookupConstants.Reports.RenewalAscii:
                    return renewalAsciiRepository.GetParameters(parameters);
                case LookupConstants.Reports.UserLogin:
                    return userLoginRepository.GetParameters(parameters);
                case LookupConstants.Reports.AdminActivity:
                    return GetAdminActivityParameters();
                case LookupConstants.Reports.NewRegistrationStatistics:
                    return newRegistrationRepository.GetParameters(parameters);
                case LookupConstants.Reports.RenewalStatistics:
                    return renewalRepository.GetParameters(parameters);
                case LookupConstants.Reports.ActiveAgentsStatistics:
                    return activeAgentsRepository.GetParameters(parameters);
                case LookupConstants.Reports.CPD:
                    return cpdRepository.GetParameters(parameters);
                case LookupConstants.Reports.BulkRegistration:
                    return bulkRegistraitonRepository.GetParameters(parameters);
                case LookupConstants.Reports.Summary:
                    return summaryRepository.GetParameters(parameters);
                case LookupConstants.Reports.Agents:
                    return agentsRepository.GetParameters(parameters);
                case LookupConstants.Reports.AutoTermination:
                    return agentsRepository.GetParameters(parameters);
                case LookupConstants.Reports.TerminationList:
                    return agentsRepository.GetParameters(parameters);
                case LookupConstants.Reports.TerminationSummary:
                    return agentsRepository.GetParameters(parameters);
                case LookupConstants.Reports.Consolidate:
                    return agentsRepository.GetParameters(parameters);
                case LookupConstants.Reports.ConsolidateSummary:
                    return agentsRepository.GetParameters(parameters);
                case LookupConstants.Reports.TBEResult:
                    return tbeRepository.GetParameters(parameters);
                case LookupConstants.Reports.InvoiceDetail:
                    return invoiceRepository.GetParameters(parameters);
                case LookupConstants.Reports.TerminatedAgentsStatistics:
                    return terminatedAgentStatisticsRepository.GetParameters(parameters);
                case LookupConstants.Reports.TrainingDetail:
                    return trainingDetailRepository.GetParameters(parameters);
                case LookupConstants.Reports.ALC:
                    return alcRepository.GetParameters(parameters);
                case LookupConstants.Reports.Referred:
                case LookupConstants.Reports.ReferredAdmin:
                    return referredListingRepository.GetParameters(parameters);
                case LookupConstants.Reports.TBESpecialAgents:
                    return tbeSpecialRepository.GetParameters(parameters);
                case LookupConstants.Reports.AdministrativeAuditTrail:
                    return administrativeAuditTrailReportRepository.GetParameters(parameters);
                case LookupConstants.Reports.PhotoUploadSummary:
                    return photoUploadSummaryReportRepository.GetParameters(parameters);
                case LookupConstants.Reports.TBEExamination:
                    return _TBEExaminationReportRepository.GetParameters(parameters);
                case LookupConstants.Reports.ResignedActiveAgents:
                    return resignedActiveAgentReportRepository.GetParameters(parameters);
                case LookupConstants.Reports.TBEExemption:
                    return _TbeExemptionReportRepository.GetParameters(parameters);
            }
            return output;
        }

        Dictionary<string, IEnumerable<dynamic>> GetAdminActivityDataSource()
        {
            var activity = dataProvider.Get<AdminActivityViewModel>(GlobalConstants.CurrentAdminActivity);
            var output = new Dictionary<string, IEnumerable<dynamic>>();
            output.Add(LookupConstants.Reports.AdminActivity, activity.GetDetails());
            return output;
        }

        Dictionary<string, string> GetAdminActivityParameters()
        {
            var activity = dataProvider.Get<AdminActivityViewModel>(GlobalConstants.CurrentAdminActivity);
            var output = new Dictionary<string, string>();
            output.Add("Activity", activity.Activity.Description);
            return output;
        }
    }// class

}// namespace
