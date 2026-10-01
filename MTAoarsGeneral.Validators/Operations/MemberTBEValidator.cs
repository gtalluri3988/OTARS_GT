using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAoarsGeneral.ViewModels.Shared;
using MTAoarsGeneral.Repositories.Interfaces;
using MTAoarsGeneral.DomainModels;
using MTAoarsGeneral.ViewModels.Interfaces;
using MTAoarsGeneral.ViewModels.Operations;
using MTAoarsGeneral.Utilities.Constants;

namespace MTAoarsGeneral.Validators.Operations
{
    public class MemberTBEValidator : BaseValidator, IValidator<IExistableTbeDetails>
    {
        IIbfimResultRepository ibfimRepository;
        IMiiResultRepository miiRepository;
        ITbeExemptionRepository tbeExemptionRepository;
        ILookupRepository lookupRepository;
        IMemberRepository memberRepository;

        //private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public MemberTBEValidator(IIbfimResultRepository ibfimRepository, IMiiResultRepository miiRepository, ITbeExemptionRepository tbeExemptionRepository, ILookupRepository lookupRepository, IMemberRepository memberRepository)
        {
            this.ibfimRepository = ibfimRepository;
            this.miiRepository = miiRepository;
            this.tbeExemptionRepository = tbeExemptionRepository;
            this.lookupRepository = lookupRepository;
            this.memberRepository = memberRepository;
        }

        public IEnumerable<ValidationMessage> Validate(IExistableTbeDetails item)
        {
            if (!item.IsOld)
            {
                IEnumerable<TbeExemption> exemptedRecords = null;

                switch (item.TbeCategory.Code)
                {
                    case LookupConstants.TbeCategory.IBFIM:
                        var results = ibfimRepository.Search(item.ICNumber).ToList();
                        if (item.IsFamily && !IsFamily(results))
                        {
                            yield return new ValidationMessage("", "The candidate \"{0}\" either did not passed or sat for the IBFIM exam in order to register", item.ICNumber);
                        }
                        if (item.IsGeneral && !IsGeneral(results))
                        {
                            yield return new ValidationMessage("", "The candidate \"{0}\" either did not passed or sat for the IBFIM exam in order to register", item.ICNumber);
                        }
                        break;
                    case LookupConstants.TbeCategory.MII:
                        var miiResults = miiRepository.Search(item.ICNumber);
                        var passGrades = (new[] { "A", "B", "C" });
                        var count = miiResults.Count(p => passGrades.Contains(p.Grade));
                        if (count == 0)
                        {
                            yield return new ValidationMessage("", "The candidate \"{0}\" either did not passed or sat for the MII exam in order to register", item.ICNumber);
                        }
                        break;
                    case LookupConstants.TbeCategory.ExemptedFPAM:
                        // CR Number 20200220-01 (Add checking with combination "TAKAFUL EXAM" and "EXEMPTION FOR"
                        // -----------------------------------------------------------------
                        exemptedRecords = tbeExemptionRepository.Search(item.ICNumber);
                        exemptedRecords = exemptedRecords.Where(i => i.LookupTakafulExemptedExam.Code == LookupConstants.TakafulExemptedExam.FPAM);
                        if (exemptedRecords.Count() == 0)
                        {
                            yield return new ValidationMessage("", "The IC number {0} is not exempted from Takaful Exam", item.ICNumber);
                        }
                        if ((item.IsFamily && !IsFamilyExempted(exemptedRecords)) || (item.IsGeneral && !IsGeneralExempted(exemptedRecords)))
                        {
                            yield return new ValidationMessage("", "This agent <{0}> is either not under the TBE Exemption Listing or wrongly selected Exemption", item.ICNumber);
                        }
                        break;
                    case LookupConstants.TbeCategory.ExemptedMFPC:
                        // CR Number 20200220-01 (Add checking with combination "TAKAFUL EXAM" and "EXEMPTION FOR"
                        // -----------------------------------------------------------------
                        exemptedRecords = tbeExemptionRepository.Search(item.ICNumber);
                        exemptedRecords = exemptedRecords.Where(i => i.LookupTakafulExemptedExam.Code == LookupConstants.TakafulExemptedExam.MFPC);
                        if (exemptedRecords.Count() == 0)
                        {
                            yield return new ValidationMessage("", "The IC number {0} is not exempted from Takaful Exam", item.ICNumber);
                        }
                        if ((item.IsFamily && !IsFamilyExempted(exemptedRecords)) || (item.IsGeneral && !IsGeneralExempted(exemptedRecords)))
                        {
                            yield return new ValidationMessage("", "This agent <{0}> is either not under the TBE Exemption Listing or wrongly selected Exemption", item.ICNumber);
                        }
                        break;
                    case LookupConstants.TbeCategory.ExemptedLTEQ08:
                        //var member = memberRepository.Get(item.ICNumber);
                        //if (member == null) {
                        //    yield return new ValidationMessage("", "The IC number {0} is not registered on or before 2008", item.ICNumber);
                        //    yield break;
                        //}
                        //var principals = memberRepository.GetPrincipals(member.ID).Count(p => p.AgencyNumber.Contains("MTA-A"));
                        //if (principals == 0) {
                        //    var histories = memberRepository.GetPrincipalHistories(member.ID).Count(p => p.AgencyNumber.Contains("MTA-A"));
                        //    if (histories == 0) {
                        //        yield return new ValidationMessage("", "The IC number {0} is not registered on or before 2008", item.ICNumber);
                        //    }
                        //}

                        var members = memberRepository.GetMembers(item.ICNumber).ToList();
                        if (members.Count == 0)
                        {
                            yield return new ValidationMessage("", "The IC number {0} is not registered on or before 2008", item.ICNumber);
                            yield break;
                        }
                        var corporateNomineMembers = members.Where(x => x.AgencyMembers.Any(am => am.DesignationID == 1)).ToList();

                        if (corporateNomineMembers.Count == 0)
                        {
                            yield return new ValidationMessage("", "The IC number {0} is not registered on or before 2008", item.ICNumber);
                            yield break;
                        }

                        var principals = new List<AgencyPrincipal>();
                        foreach (var member in members)
                            principals.AddRange(memberRepository.GetPrincipals(member.ID));

                        // CR Number 20190719-01 (All agents with TBE exception will have TBE exemption for life if resign or terminated. Affected (MTA-A, MTA-B, MTA-Q))
                        // -----------------------------------------------------------------
                        var terminationStatus = new string[] { LookupConstants.TerminationAction.Resign, LookupConstants.TerminationAction.Terminate };
                        var specificAgencyPrefix = new string[] { "MTA-A", "MTA-B", "MTA-Q", "MTAQ" };

                        Func<string, bool> checkPrefix = x =>
                        {
                            foreach (var prefix in specificAgencyPrefix)
                                if (x.Contains(prefix))
                                    return true;
                            return false;
                        };

                        var isExist = principals.Any(p => checkPrefix.Invoke(p.AgencyNumber));
                        var isExistAsCorporateNomine = principals.Any(p => checkPrefix.Invoke(p.AgencyNumber) && p.Member.AgencyMembers.Any(am => am.DesignationID == 1));
                        if (isExist && isExistAsCorporateNomine)
                            break;

                        var histories = new List<AgencyPrincipalHistory>();
                        foreach (var member in members)
                            histories.AddRange(memberRepository.GetPrincipalHistories(member.ID));

                        isExist = histories != null && histories.Any(h => h != null && checkPrefix.Invoke(h.AgencyNumber) && h.LookupTerminationAction != null && terminationStatus.Contains(h.LookupTerminationAction.Code));
                        isExistAsCorporateNomine = histories != null && histories.Any(h => h != null && checkPrefix.Invoke(h.AgencyNumber) && h.LookupTerminationAction != null && terminationStatus.Contains(h.LookupTerminationAction.Code)
                            && h.Member != null && h.Member.AgencyMembers != null && h.Member.AgencyMembers.Any(am => am != null && am.DesignationID == 1));

                        if (isExist && isExistAsCorporateNomine)
                            break;

                        //var cnt = principals.Count(p => p.AgencyNumber.Contains("MTA-A"));
                        //if (cnt == 0)
                        //{
                        //    /* Allow terminated General "MTA-A" Agent to proceed registration on General Agent - CR 16/10/2018 */
                        //    if (item.IsGeneral)
                        //    {                                
                        //        cnt = histories.Count(
                        //            p => p.AgencyNumber.Contains("MTA-A")
                        //            && p.LookupIntermediaryType.Code == LookupConstants.IntermediaryType.General 
                        //            && p.LookupTerminationAction.Code == LookupConstants.TerminationAction.Terminate);
                        //        if (cnt > 0) break;
                        //    }

                        //    cnt = histories.Count(p => p.AgencyNumber.Contains("MTA-A") && p.LookupTerminationAction.Code == LookupConstants.TerminationAction.Resign);
                        //    if (cnt == 0)
                        //    {
                        //        yield return new ValidationMessage("", "The IC number {0} is not registered on or before 2008.", item.ICNumber);
                        //    }
                        //}

                        // CR Number 20200220-01 (Add checking with combination "TAKAFUL EXAM" and "EXEMPTION FOR"
                        // -----------------------------------------------------------------
                        exemptedRecords = tbeExemptionRepository.Search(item.ICNumber);
                        exemptedRecords = exemptedRecords.Where(i => i.LookupTakafulExemptedExam.Code == LookupConstants.TakafulExemptedExam.Register2008OrBelow);
                        if ((item.IsFamily && !IsFamilyExempted(exemptedRecords)) || (item.IsGeneral && !IsGeneralExempted(exemptedRecords)))
                        {
                            yield return new ValidationMessage("", "This agent <{0}> is either not under the TBE Exemption Listing or wrongly selected Exemption", item.ICNumber);
                        }

                        yield return new ValidationMessage("", "The IC number {0} is not registered on or before 2008.", item.ICNumber);

                        break;
                    case LookupConstants.TbeCategory.SPECIAL:
                        exemptedRecords = tbeExemptionRepository.Search(item.ICNumber);
                        exemptedRecords = exemptedRecords.Where(i => i.LookupTakafulExemptedExam.Code == LookupConstants.TakafulExemptedExam.SPECIAL);
                        var counts = exemptedRecords.Count(t => t.LookupTakafulExemptedExam.Code.Equals(LookupConstants.TbeCategory.SPECIAL));
                        if (counts == 0)
                        {
                            yield return new ValidationMessage("", "The IC number {0} is not special-exempted from Takaful Exam", item.ICNumber);
                        }
                        // CR Number 20200220-01 (Add checking with combination "TAKAFUL EXAM" and "EXEMPTION FOR"
                        // -----------------------------------------------------------------
                        if ((item.IsFamily && !IsFamilyExempted(exemptedRecords)) || (item.IsGeneral && !IsGeneralExempted(exemptedRecords)))
                        {
                            yield return new ValidationMessage("", "This agent <{0}> is either not under the TBE Exemption Listing or wrongly selected Exemption", item.ICNumber);
                        }
                        break;
                    case LookupConstants.TbeCategory.TBEGE:
                        exemptedRecords = tbeExemptionRepository.Search(item.ICNumber);
                        exemptedRecords = exemptedRecords.Where(i => i.LookupTakafulExemptedExam.Code == LookupConstants.TakafulExemptedExam.TBEGE);
                        var countsC = exemptedRecords.Count(t => t.LookupTakafulExemptedExam.Code.Equals(LookupConstants.TbeCategory.TBEGE));
                        if (countsC == 0)
                        {
                            yield return new ValidationMessage("", "The IC number {0} is not exempted from Takaful Exam Guidelines", item.ICNumber);
                        }
                        // CR Number 20200220-01 (Add checking with combination "TAKAFUL EXAM" and "EXEMPTION FOR"
                        // -----------------------------------------------------------------
                        if ((item.IsFamily && !IsFamilyExempted(exemptedRecords)) || (item.IsGeneral && !IsGeneralExempted(exemptedRecords)))
                        {
                            yield return new ValidationMessage("", "This agent <{0}> is either not under the TBE Exemption Listing or wrongly selected Exemption", item.ICNumber);
                        }
                        break;
                    case LookupConstants.TbeCategory.EXP: //Exempted from Board
                        exemptedRecords = tbeExemptionRepository.Search(item.ICNumber);
                        exemptedRecords = exemptedRecords.Where(i => i.LookupTakafulExemptedExam.Code == LookupConstants.TakafulExemptedExam.EXP);
                        var countsB = exemptedRecords.Count(t => t.LookupTakafulExemptedExam.Code.Equals(LookupConstants.TbeCategory.EXP));
                        if (countsB == 0)
                        {
                            yield return new ValidationMessage("", "The IC number {0} is not exempted from MTA Board", item.ICNumber);
                        }
                        // CR Number 20200220-01 (Add checking with combination "TAKAFUL EXAM" and "EXEMPTION FOR"
                        // -----------------------------------------------------------------
                        if ((item.IsFamily && !IsFamilyExempted(exemptedRecords)) || (item.IsGeneral && !IsGeneralExempted(exemptedRecords)))
                        {
                            yield return new ValidationMessage("", "This agent <{0}> is either not under the TBE Exemption Listing or wrongly selected Exemption", item.ICNumber);
                        }
                        break;
                }
            }
        }

        bool IsGeneral(IEnumerable<IbfimResult> results)
        {
            return IsPassed(results, "A") && IsPassed(results, "B");
        }

        bool IsFamily(IEnumerable<IbfimResult> results)
        {
            return IsPassed(results, "A") && IsPassed(results, "C");
        }

        bool IsPassed(IEnumerable<IbfimResult> results, string part)
        {
            return results.Count(p => p.ExamType == part && p.Result.ToUpper() == "PASS") > 0;
        }

        bool IsFamilyExempted(IEnumerable<TbeExemption> result)
        {
            var isExempted = new string[] {
                LookupConstants.TakafulExemptionFor.FamilyTakafulOnly,
                LookupConstants.TakafulExemptionFor.GeneralFamilyTakaful
            };
            return result.Count(i => isExempted.Contains(i.LookupTakafulExemptionFor.Code)) > 0;
        }
        bool IsGeneralExempted(IEnumerable<TbeExemption> result)
        {
            var isExempted = new string[] {
                LookupConstants.TakafulExemptionFor.GeneralTakafulOnly,
                LookupConstants.TakafulExemptionFor.GeneralFamilyTakaful
            };
            return result.Count(i => isExempted.Contains(i.LookupTakafulExemptionFor.Code)) > 0;
        }

    }// class
}// namespace