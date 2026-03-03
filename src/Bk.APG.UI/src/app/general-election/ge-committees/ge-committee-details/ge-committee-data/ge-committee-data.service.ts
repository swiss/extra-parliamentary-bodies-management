import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {CandidateListValidationResult} from '@api/CandidateListValidationResult';
import {CommitteeDetails} from '@api/CommitteeDetails';
import {EiamAssignment} from '@api/EiamAssignment';
import {GeneralElectionCommitteeUpdate} from '@api/GeneralElectionCommitteeUpdate';
import {ReadyForProposalForward} from '@api/ReadyForProposalForward';
import {map} from 'rxjs';

@Injectable({providedIn: 'root'})
export class GeneralElectionCommitteeDataService {
    constructor(private readonly http: HttpClient) {}

    updateGeneralElectionCommittee = (generalElectionCommittee: GeneralElectionCommitteeUpdate) =>
        this.http.put<CommitteeDetails>(`/api/general-election/committees/${generalElectionCommittee.committeeId}`, generalElectionCommittee);

    getGeneralElectionCommitteeForUpdate = (committeeId: string) =>
        this.http.get<GeneralElectionCommitteeUpdate>(`/api/general-election/committees/${committeeId}/update`).pipe(
            map(committee => ({
                ...committee,
                beginDate: new Date(committee.beginDate),
                endDate: committee.endDate ? new Date(committee.endDate) : undefined,
            }))
        );

    getAssignmentsForReadyForProposalForward = (committeeId: string) =>
        this.http.get<EiamAssignment[]>(`/api/general-election/committees/${committeeId}/ready-for-proposal/forward`);

    forwardReadyForProposal = (committeeId: string, readyForProposalForward: ReadyForProposalForward) =>
        this.http.post<void>(`/api/general-election/committees/${committeeId}/ready-for-proposal/forward`, readyForProposalForward);

    finalizeReadyForProposal = (committeeId: string) =>
        this.http.post<CandidateListValidationResult>(`/api/general-election/committees/${committeeId}/ready-for-proposal/finalize`, {});
}
