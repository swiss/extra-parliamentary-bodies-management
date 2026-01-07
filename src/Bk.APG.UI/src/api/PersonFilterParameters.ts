import {Canton} from './Canton';
import {Language} from './Language';

export interface PersonFilterParameters {
    freeText?: string;
    hasActiveMembership?: boolean[];
    cantons?: Canton['id'][];
    languages?: Language['id'][];
}
