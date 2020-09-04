export class PagedResponse<T> {
  values: T[];
  total: number;
  limit: number;
  offset: number;
}
