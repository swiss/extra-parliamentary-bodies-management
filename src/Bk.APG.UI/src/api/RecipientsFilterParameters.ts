import {CommitteeDetails} from './CommitteeDetails';
import {CommitteeType} from './CommitteeType';
import {Department} from './Department';
import {ElectionType} from './ElectionType';
import {FormLettersSenderList} from './FormLettersSenderList';
import {Language} from './Language';
import {Office} from './Office';

export interface RecipientsFilterParameters {
    departments?: Department['id'][] | null;
    offices?: Office['id'][] | null;
    committeeTypes?: CommitteeType['id'][] | null;
    committees?: CommitteeDetails['id'][] | null;
    correspondenceLanguages?: Language['id'][] | null;
    electionTypes?: ElectionType['id'][] | null;
    formLetterSender?: FormLettersSenderList['id'] | null;
    exportType?: string | null;
    exportFileType?: string | null;
}
