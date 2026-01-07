export interface EntityAuditLog {
    auditUser: string;
    auditDate: Date;
    auditAction: string;
    auditDataBefore: EntityAuditLogData[];
    auditDataAfter: EntityAuditLogData[];
    entityType: string;
}

export interface EntityAuditLogData {
    columnName: string;
    data?: string;
}
