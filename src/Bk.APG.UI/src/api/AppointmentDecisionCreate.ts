import {DocumentStorageCreateDto} from './DocumentStorageCreateDto';

export interface AppointmentDecisionCreate {
    committeeId: string;
    appointmentDecisionDate: Date;
    appointmentDecisionTypeId: string;
    appointmentDecisionLinkTypeId?: string;
    text?: string;
    link?: string;
    documents: DocumentStorageCreateDto[];
}
