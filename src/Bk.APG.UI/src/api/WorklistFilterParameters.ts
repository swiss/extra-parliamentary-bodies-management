import {CommitteeDetails} from './CommitteeDetails';
import {Department} from './Department';
import {Office} from './Office';
import {WorklistTaskState} from './WorklistTaskState';
import {WorklistTaskType} from './WorklistTaskType';

export interface WorklistFilterParameters {
    committee?: CommitteeDetails['description'];
    departments?: Department['id'][];
    offices?: Office['id'][];
    worklistTaskTypes?: WorklistTaskType['id'][];
    worklistTaskStates?: WorklistTaskState['id'][];
    assignedBy?: string;
    assignedTo?: string;
    createdFrom?: Date;
    createdTo?: Date;
    dueDateFrom?: Date;
    dueDateTo?: Date;
}
