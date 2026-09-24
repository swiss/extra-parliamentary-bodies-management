export interface WorklistTaskUpdate {
    id: string;
    worklistTaskType: string;
    worklistTaskState: string;
    description: string;
    assignedTo: string;
    dueDate: Date;
    created?: Date;
    canEdit?: boolean;
    canForward?: boolean;
    isBigDepartment?: boolean;
}
