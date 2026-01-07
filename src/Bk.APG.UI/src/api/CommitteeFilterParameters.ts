import {CommitteeType} from './CommitteeType';
import {Department} from './Department';
import {Level} from './Level';
import {Office} from './Office';
import {Term} from './Term';

export interface CommitteeFilterParameters {
    freeText?: string;
    levels?: Level['id'][];
    departments?: Department['id'][];
    offices?: Office['id'][];
    committeeTypes?: CommitteeType['id'][];
    terms?: Term['id'][];
    isActive?: boolean[];
    isMarketOrientated?: boolean[];
    hasSupervisionDuty?: boolean[];
}
