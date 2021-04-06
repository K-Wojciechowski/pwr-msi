export interface Page<T> {
    items: T[];
    page: number;
    max: number;
    itemCount: number;
}
