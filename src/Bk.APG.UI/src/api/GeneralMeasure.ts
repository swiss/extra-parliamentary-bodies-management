export interface GeneralMeasure {
    departmentId: string;
    department: string;
    justificationLanguages?: string;
    justificationGenders?: string;
    isDepartmentTaskActive: boolean;
    isAdminTaskActive: boolean;
    canForwardToDepartment: boolean;
    canForwardToAdmin: boolean;
    canValidate: boolean;
}
