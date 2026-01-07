import {AddressUpdate} from './AddressUpdate';
import {InterestDetails} from './InterestDetails';
import {MembershipDetails} from './MembershipDetails';
import {Occupation} from './Occupation';

export interface PersonUpdate {
    id: string;
    salutationId?: string;
    salutationText?: string;
    surname: string;
    givenName: string;
    birthYear: number;
    maskAddress: boolean;
    privateAddress?: AddressUpdate;
    officeAddress?: AddressUpdate;
    languageId: string;
    correspondenceLanguageId: string;
    title?: string;
    occupation?: string;
    genderId: string;
    employer?: string;
    federalDuty: boolean;
    federalAssembly: boolean;
    legislaturePeriodIds: string[];
    noInterest: boolean;
    noEmployment: boolean;
    interests?: InterestDetails[];
    memberships?: MembershipDetails[];
    hasInterests?: boolean;
    isMissingJustificationFederalAssembly: boolean;
    councilId?: string;
    officeId?: string;
    hasActiveMembership: boolean;
    canDelete: boolean;
    occupations?: Occupation[];
    needsAttentionOccupation?: boolean;
    rowVersion: number;
}
