export interface PageResult<T> {
    pageNumber: number,
    pageSize: number,
    totalItems: number,
    totalPages: number,
    data: T[]
}