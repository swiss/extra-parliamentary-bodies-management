export interface WorklistTaskCreate {
    worklistTaskTypeId: string;
    description: string;
    assignedToId: string;
    dueDate: Date;
}
