import {CommitteeType} from './CommitteeType';
import {Department} from './Department';
import {Level} from './Level';
import {Office} from './Office';

export interface GeneralElectionCommitteeFilterParameters {
    freeText?: string;
    levels?: Level['id'][];
    departments?: Department['id'][];
    offices?: Office['id'][];
    committeeTypes?: CommitteeType['id'][];
    isMarketOrientated?: boolean[];
    hasSupervisionDuty?: boolean[];
    isNew?: boolean[];
    vacancies?: boolean[];
    statusProposal?: boolean[];
    modifiedBy?: string;
}
