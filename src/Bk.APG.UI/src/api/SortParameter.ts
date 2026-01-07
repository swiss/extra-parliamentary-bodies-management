export type SortDirection = 'asc' | 'desc' | '';

export interface SortParameter<T = string> {
    sort: T;
    direction: SortDirection;
}
