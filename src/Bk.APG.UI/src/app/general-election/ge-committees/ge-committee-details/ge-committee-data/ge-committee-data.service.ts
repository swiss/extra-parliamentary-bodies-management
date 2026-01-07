import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {CommitteeDetails} from '@api/CommitteeDetails';
import {GeneralElectionCommitteeUpdate} from '@api/GeneralElectionCommitteeUpdate';
import {map} from 'rxjs';

@Injectable()
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
}
