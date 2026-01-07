import {DocumentStorageUpdateDto} from './DocumentStorageUpdateDto';

export interface AppointmentDecisionUpdate {
    id: string;
    appointmentDecisionDate: Date;
    appointmentDecisionTypeId: string;
    appointmentDecisionLinkTypeId?: string;
    text?: string;
    link?: string;
    documents: DocumentStorageUpdateDto[];
}
