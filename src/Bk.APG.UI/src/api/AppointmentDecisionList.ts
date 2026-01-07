export interface AppointmentDecisionList {
    id: string;
    appointmentDecisionDate: Date;
    appointmentDecisionType: string;
    appointmentDecisionLinkType: string;
    documentStorageId: string;
    text: string;
    linkText: string;
    link: string;
    fileName: string;
    modified: Date;
}
