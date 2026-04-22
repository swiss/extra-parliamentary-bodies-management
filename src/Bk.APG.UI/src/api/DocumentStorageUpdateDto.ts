export interface DocumentStorageUpdateDto {
    id: string;
    displayName: string;
    documentStorageId?: string;
    isOriginal: boolean;
    languageId: string;
    file: File;
}
