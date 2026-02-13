import {FormLettersSenderCreate} from './FormLettersSenderCreate';

export interface FormLettersSenderUpdate extends FormLettersSenderCreate {
    id: string;
    signatureFileName?: string;
    canEditDepartment: boolean;
}
