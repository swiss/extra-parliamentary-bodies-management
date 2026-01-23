import {ReportType} from '../app/exports/ReportType';
import {CommitteeDetails} from './CommitteeDetails';
import {CommitteeType} from './CommitteeType';
import {Department} from './Department';
import {Office} from './Office';

export interface RequestsAndReportsFilterParameters {
    documentType?: ReportType | null;
    analysisDate1?: Date | null;
    analysisDate2?: Date | null;
    departments?: Department['id'][] | null;
    offices?: Office['id'][] | null;
    committeeTypes?: CommitteeType['id'][] | null;
    committees?: CommitteeDetails['id'][] | null;
    isGeneralElection?: boolean;
    committeesWithActiveMembership?: boolean;
    releasedCommittees?: boolean;
}
