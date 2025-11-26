export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface ApiPagedResponse<T> {
  data: PagedResult<T>;
  isSuccess: boolean;
  message: string;
  errors: string[];
}
