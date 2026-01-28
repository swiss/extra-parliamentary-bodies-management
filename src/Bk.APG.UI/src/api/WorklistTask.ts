export interface WorklistTask {
    id: string;
    assignedTo: string;
    assignedBy: string;
    dueDate: Date;
    worklistTaskType: string;
    worklistTaskState: string;
    created?: Date;
    department?: string;
    office?: string;
    committee?: string;
    section?: string;
    isInactive?: boolean;
    isCompleted?: boolean;
    isOverdue?: boolean;
}
