export interface BulkSaveDto<T> {
    added: T[];
    edited: T[];
    deleted: T[];
    validFrom?: string | null;
}
