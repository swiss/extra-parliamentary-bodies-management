export interface PagedResult<T> {
    index: number;
    total: number;
    items: T[];
}
