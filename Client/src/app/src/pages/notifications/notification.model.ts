export interface Notification {
  id: number;
  type: `success` | `error`;
  content: string | undefined;
}
